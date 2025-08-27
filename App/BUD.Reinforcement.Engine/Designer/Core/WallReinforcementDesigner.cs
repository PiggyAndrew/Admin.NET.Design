/*
 * 
 *  Steps:
 *      Dimensions
 *      Loadings
 *      Design Moment and Shear
 *      Bending Reinforcement
 *      Shear Reinforcement
 *      Deflection Checking
 *      Checking for Axial Loadig
 * 
 * 
 */

using DSD.Reinforcement.Engine.Components;
using DSD.Reinforcement.Engine.Components.Extensions;
using DSD.Reinforcement.Engine.Designer.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine.Designer.Core;

public class WallReinforcementDesigner : ReinforcementDesigner<Wall>
{
    private bool _isLongSpan;
    private double fyk;

    public WallReinforcementDesigner(Wall wall, bool isLongSpan, params Load[] loadings) : base(wall, loadings)
    {
        LongSpan = Math.Max(wall.Length, wall.Height) / 1000;
        ShortSpan = Math.Min(wall.Length, wall.Height) / 1000;

        fyk = isLongSpan ? StructuralComponent.LongSpanRebarArguments.Fyk : StructuralComponent.ShortSpanRebarArguments.Fyk;
        ProviderArea = isLongSpan ? StructuralComponent.LongSpanRebarArguments.GetRebarArea() : StructuralComponent.ShortSpanRebarArguments.GetRebarArea();
        EffectiveDepth = StructuralComponent.GetEffectiveDepth(isLongSpan);

        StepCommands.Add(new StepCommand(() => DeflectionChecking(SupportType.Continous)));
        StepCommands.Add(new StepCommand(CheckingForAxialLoading));

        _isLongSpan = isLongSpan;
    }

    /// <summary>
    /// Unit D Load
    /// </summary>
    protected double UDL { get; set; } = 0.0;

    /// <summary>
    /// Trigental Load
    /// </summary>
    protected double Tri { get; set; } = 0.0;

    /// <summary>
    /// Total moment
    /// </summary>
    public double Moment { get; set; }

    /// <summary>
    /// Total shear
    /// </summary>
    public double Shear { get; set; }

    /// <summary>
    /// Provided area of reinforcement 
    /// <para>As,𝒑𝒓𝒐𝒗 </para>
    /// </summary>
    public double ProviderArea { get; set; }

    public override Step Loading()
    {
        Step step = base.Loading();

        var n1 = 0.0;
        var n2 = 0.0;

        foreach (var item in Loadings)
        {
            switch (item.LoadType)
            {
                case LoadType.UniformlyDistributedLoad:
                    n1 += item.Weight;
                    break;
                case LoadType.TriangularLoad:
                    n2 += item.Weight;
                    break;
            }
        }

        UDL = n1 * Properties.Settings.Default.FactorOfImposedLoad;
        Tri = n2 * Properties.Settings.Default.FactorOfDeadLoad;

        step.Add($"Factored UDL, n1 = {UDL} KN/m²");
        step.Add($"Factored Tri. Load, n2 = {Tri} KN/m²");

        return step.Success("Done.");
    }

    public override Step DesignMoment()
    {
        Step step = base.DesignMoment();

        var udlCoefficient = new ForceCoefficient.BendingMoment.UniformlyDistributedLoad();

        double bsx = MathKit.Forecast(LongSpan / ShortSpan, udlCoefficient.Ratio, udlCoefficient.Values_x);

        double msx1 = bsx * UDL * Math.Pow(ShortSpan, 2);

        var triCoefficient = _isLongSpan ?
            ForceCoefficient.BendingMoment.TriangularLoad.GetLongSpanTriangularLoad() :
            ForceCoefficient.BendingMoment.TriangularLoad.GetShortSpanTriangularLoad();
        double amx = MathKit.Forecast(ShortSpan / LongSpan,
           triCoefficient.Ratio,
           triCoefficient.Values_x);

        double msx2 = amx * Tri * Math.Pow(LongSpan, 2);

        Moment = Math.Max(msx1, msx2);


        step.Add($"Maximum moment Bsx = {bsx:0.000}");
        step.Add($"Msx: {bsx:0.000} * {UDL} * {ShortSpan:0.000}² = {msx1:0.000}");
        step.Add($"Amx = {amx:0.000}");
        step.Add($"Msx: {bsx:0.000} * {Tri} * {LongSpan}² = {msx2:0.000}");

        return step.Success($"Therefore , Moment = {Moment:0.000}");
    }

    public override Step DesignShear()
    {
        Step step = base.DesignShear();
        var udlCoefficient = new ForceCoefficient.Shear.UniformlyDistributedLoad();
        double bsx = MathKit.Forecast(LongSpan / ShortSpan,
           udlCoefficient.Ratio,
           udlCoefficient.Values_x);

        double vsx1 = bsx * UDL * ShortSpan;

        var triCoefficient = _isLongSpan ?
           ForceCoefficient.Shear.TriangularLoad.GetLongSpanTriangularLoad() :
           ForceCoefficient.Shear.TriangularLoad.GetShortSpanTriangularLoad();

        double amx = MathKit.Forecast(ShortSpan / LongSpan,
            triCoefficient.Ratio,
            triCoefficient.Values_x);

        double vsx2 = amx * Tri * LongSpan;

        Shear = Math.Max(vsx1, vsx2);


        step.Add($"Maximum shear Bsx = {bsx:0.000}");
        step.Add($"Vsx: {bsx:0.000} * {UDL} * {ShortSpan:0.000} = {vsx1:0.000}");
        step.Add($"Amx = {amx:0.000}");
        step.Add($"Vsx: {amx:0.000} * {Tri} * {LongSpan:0.000} = {vsx2:0.000}");

        return step.Success($"Therefore, Shear = {Shear:0.000}");
    }

    public override Step BendingReinforcement()
    {
        Step step = base.BendingReinforcement();

        double b = StructuralComponent.ConcreteArguments.Breadth / 1000;
        double fck = StructuralComponent.ConcreteArguments.Fck;

        double k = Moment / (fck * b * Math.Pow(EffectiveDepth / 1000, 2)) / 1000;

        step.Add($"K: {Moment:0.000}/({fck}*{b}*{EffectiveDepth}²)/1000 = {k:0.000}");

        if (k >= 0.167)
        {
            step.Add("Therefore, redesign la!!!");
            return step;
        }

        step.Add($"Therefore, no compression reinforcement is required\n");

        double z = 0.5 + Math.Pow(0.25 - k / 1.134, 0.5);

        if (z > 0.95)
        {
            step.Add($"z = {z:0.000}d > 0.95d");
            z = 0.95;
        }
        else
        {
            step.Add($"z = {z:0.000}d < 0.95d");
        }
        step.Add($"   = {z:0.000}d");
        step.Add($"   = {z:0.000}*{EffectiveDepth}*1000");

        z = z * EffectiveDepth;
        step.Add($"Therefore, z = {z:0.000} mm\n");


        double xu = (EffectiveDepth / 1000 - z) / 0.4;
        if (xu / EffectiveDepth >= 0.45)
        {
            step.Add("design again!!");
            return step;
        }

        double requirdAs = Moment / (0.87 * fyk * z) * Math.Pow(1000, 2);
        step.Add($"As,requird = {requirdAs:0.000} mm²");

        double fctm = 3.2;
        double minAsRatio = 0.26 * fctm / fyk * 100;
        step.Add($"As,min \t= {minAsRatio:0.000}%*b*d");

        if (IsRequirement(minAsRatio))
        {
            double minAs = minAsRatio / 100 * b * 1000 * EffectiveDepth;
            requirdAs = Math.Max(requirdAs, minAs);
            step.Add($"\t= {minAsRatio:0.000}*{b}*{EffectiveDepth / 1000}");
            step.Add($"\t= {minAs:0.000}");
        }

        step.Add($"Thus, As,requird = {requirdAs:0.000} mm²\n");

        step.Add($"As,provider = {ProviderArea:0.000}");
        if (ProviderArea <= requirdAs)
        {
            return step.Fail("design again!!");
        }

        double steelRatio = ProviderArea / StructuralComponent.Thickness / b / 1000;

        step.Add($"Steel Ratio = {ProviderArea:0.00}/{StructuralComponent.Thickness}/{b}/1000");
        step.Add($"\t= {steelRatio * 100:0.000}%");

        return IsRequirement(steelRatio * 100) ? step.Success("Ok") : step.Fail("design again!!!");
    }

    protected override Step DeflectionChecking(SupportType supportType)
    {
        return base.DeflectionChecking(supportType);
    }

    public override Step ShearReinforcement()
    {
        Step step = base.ShearReinforcement();
        double d = EffectiveDepth;

        double fck = StructuralComponent.ConcreteArguments.Fck;
        double k = 1 + Math.Sqrt(200 / d);

        double r = ProviderArea / (1000 * d);

        double vrdc = 0.12 * Math.Min(k, 2) * 1000 * d * Math.Pow(100 * r * fck, 1.0 / 3.0) / 1000;

        step.Add($"where k = {k:0.000}  , d = {d}");
        step.Add($"Vrdc = {vrdc:0.000} compare with {Shear:0.000}");

        if (Shear < vrdc)
        {
            step.Success("Shear Reinforcement is not required");
            return step;
        }

        step.Add("Shear Reinforcement is required");

        var angle = MathKit.RadiansToDegrees(0.5 * Math.Asin(Shear * 1000 / (0.18 * 1000 * d * (1 - fck / 250d) * fck)));
        step.Add($"{angle}");
        var adopt = angle > 22 ? (angle < 45 ? angle : 45) : 22;
        step.Add($"Therefore, adopt θ = {adopt}");


        double radians = MathKit.DegreesToRadians(adopt);

        double vraMax = 0.36 * 1000 * d * (1 - fck / 250) * fck / (MathKit.Cot(radians) + Math.Tan(radians)) / 1000;
        step.Add($"{vraMax} KN");

        double asw = Shear / (0.78 * d * 500 * MathKit.Cot(radians)) * 1000;
        step.Add($"{asw} mm²/mm");


        double Asws = MathKit.Area(12) * 1000 / 300 / 300;
        step.Add($"{Asws} mm²/mm");
        return Asws > asw ? step.Success("Ok") : step.Fail("design again!!!");
    }

    protected Step CheckingForAxialLoading()
    {
        //double selfWeightOfWall = 25 * LongSpan * ShortSpan * this.StructuralComponent.Thickness;

        //double selfWeightOfTopCover=25*

        return new Step("CheckingForAxialLoading");
    }
}
