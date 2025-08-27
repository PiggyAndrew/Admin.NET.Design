/*
 * 
 * 荷载（Load）计算的安全系数取值：
 * 1、静荷载/自身荷载 Dead Load（DL）：1.35
 * 2、活荷载 
 */


using DSD.Reinforcement.Engine.Components;
using DSD.Reinforcement.Engine.Designer.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace DSD.Reinforcement.Engine.Designer.Core;

public abstract class ReinforcementDesigner<T> : IReinforcementDesigner, ICalculator<CalculationResult> where T : IStructuralComponent
{
    public ReinforcementDesigner(T structuralComponent, params Load[] loadings)
    {
        StructuralComponent = structuralComponent;
        Loadings = loadings;

        StepCommands.Add(new StepCommand(Dimensions));
        StepCommands.Add(new StepCommand(Loading));
        StepCommands.Add(new StepCommand(DesignMoment));
        StepCommands.Add(new StepCommand(DesignShear));
        StepCommands.Add(new StepCommand(BendingReinforcement));
        StepCommands.Add(new StepCommand(ShearReinforcement));
    }

    protected List<ICalculator<Step>> StepCommands { get; } = new List<ICalculator<Step>>();

    /// <summary>
    /// The component force model
    /// </summary>
    /// <remarks>
    /// if <see cref="Height"/> / <see cref="Length"/> > 2 ,the component is <see cref="ForceType.OneWaySlab"/> otherwise , <see cref="ForceType.TwoWaySlab"/>
    /// </remarks>
    public ForceType ForceType { get; set; }

    /// <summary>
    /// Long span
    /// </summary>
    protected double LongSpan { get; set; }

    /// <summary>
    /// Short span
    /// </summary>
    protected double ShortSpan { get; set; }

    /// <summary>
    /// Effective depth of the tension reinforcement 
    /// <para>The symbol d</para>
    /// </summary>
    public double EffectiveDepth { get; set; }

    /// <summary>
    /// Loading of the structural component
    /// </summary>
    protected Load[] Loadings { get; set; }

    /// <summary>
    /// Designer host structural component
    /// </summary>
    protected T StructuralComponent { get; set; }

    public virtual Step Dimensions()
    {
        Step step = new Step("Dimensions");

        double ratio = RatioCalculate();

        ForceType = ratio > 2 ? ForceType.OneWaySlab : ForceType.TwoWaySlab;

        step.Add($"ly = {LongSpan:N3}");
        step.Add($"lx = {ShortSpan:N3}");
        step.Add($"ly/lx = {ratio:N3}");

        return step.Success($"Therefore, design the component as a {(int)ForceType} way Slab");

        /// <summary>
        /// The result of <see cref="Height"/> divde <see cref="Length"/>
        /// </summary>
        /// <returns></returns>
        double RatioCalculate() => LongSpan / ShortSpan;
    }

    public virtual Step Loading()
    {
        return new Step("Loading");
    }

    public virtual Step DesignMoment()
    {
        return new Step("DesignMoment");
    }

    public virtual Step DesignShear()
    {
        return new Step("DesignShear");
    }

    public virtual Step BendingReinforcement()
    {
        return new Step("BendingReinforcement");
    }

    public virtual Step ShearReinforcement()
    {
        return new Step("ShearReinforcement");
    }

    protected virtual Step DeflectionChecking(SupportType supportType)
    {
        Step step = new Step("Deflection Checking");

        double ratio = 0;

        switch (supportType)
        {
            case SupportType.SimplySupported:
                ratio = 20;
                break;
            case SupportType.Continous:
                ratio = 26;
                break;
            case SupportType.Cantilever:
                ratio = 7;
                break;
        }

        double actualRatio = LongSpan * 1000 / EffectiveDepth;

        step.Add($"{ratio}");
        step.Add($"Actual Span-Depth Ratio {actualRatio}  = {LongSpan * 1000} / {EffectiveDepth}");

        return actualRatio >= ratio ? step.Fail($"Actual ratio = {actualRatio} >= {ratio}, Design again") : step.Success($"Actual ratio = {actualRatio} < {ratio}, Ok");
    }

    public CalculationResult Calculate()
    {
        CalculationResult calculationResult = new CalculationResult();
        int count = StepCommands.Count;
        Step[] steps = new Step[count];
        for (int i = 0; i < count; i++)
        {
            ICalculator<Step> command = StepCommands[i];

            Step step = command.Calculate();
            steps[i] = step;

            if (step.NodeStatus == NodeStatus.Failed)
            {
                calculationResult.NodeStatus = step.NodeStatus;
            }
        }

        calculationResult.Steps = steps;
        return calculationResult;
    }

    /// <summary>
    /// A measure of the relative compressive stress in a member in flexure
    /// </summary>
    /// <param name="moment"></param>
    /// <param name="fck"></param>
    /// <param name="b"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    protected double K(double moment, double fck, double b, double d)
    {
        return moment / (fck * b * Math.Pow(d, 2)) / 1000;
    }

    /// <summary>
    /// z
    /// </summary>
    /// <param name="k"></param>
    /// <returns></returns>
    protected double LeverArm(double k)
    {
        return 0.5 + Math.Pow(0.25 - k / 1.134, 0.5);
    }

    protected bool IsRequirement(double ratio) => ratio > Properties.Settings.Default.MinSteelRatio && ratio < Properties.Settings.Default.MaxSteelRatio;
}
