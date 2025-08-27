/*
 * 
 *  Step:
 *      Dimensions
 *      Loadings
 *      Design Moment and Shear
 *      Bending Reinforcement
 *      Shear Reinforcement
 *      Deflection Checking
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

public class SlabReinforcementDesigner : ReinforcementDesigner<Slab>
{
    public SlabReinforcementDesigner(Slab Slab, params Load[] loadings) : base(Slab, loadings)
    {
        LongSpan = Math.Max(Slab.Length, Slab.Width) / 1000;
        ShortSpan = Math.Min(Slab.Length, Slab.Width) / 1000;

        StepCommands.Add(new StepCommand(() => DeflectionChecking(SupportType.Continous)));
        EffectiveDepth = StructuralComponent.GetEffectiveDepth(true);
    }

    public double TotalPressure { get; set; }

    public double Moment { get; set; }

    public double Shear { get; set; }

    public double ProviderArea { get; set; }

    public override Step BendingReinforcement()
    {
        Step step = base.BendingReinforcement();
        double fcu = this.StructuralComponent.ConcreteArguments.Fck;
        double fy = this.StructuralComponent.LongSpanRebarArguments.Fyk;
        double cover = this.StructuralComponent.ConcreteArguments.Cover;
        double breath = this.StructuralComponent.ConcreteArguments.Breadth;
        double diameter = this.StructuralComponent.LongSpanRebarArguments.Diameter;

        step.Add($"fcu = {fcu} MPa");
        step.Add($"fy = {fy} MPa");
        step.Add($"cover = {cover} mm");
        step.Add($"b = {breath} mm");
        step.Add($"d = {diameter} mm Diameter");

        step.Add($"d = {EffectiveDepth}");
        double k = K(Moment, fcu, breath / 1000, EffectiveDepth / 1000);
        step.Add($"K = {k}");

        if (k < 0.167)
        {
            step.Add("Therefore, no compression reinforcement required");
        }
        else
        {
            return step.Fail("Therefore, design again!!");
        }

        double z = LeverArm(k);
        step.Add($"z = {z}");

        z = (z > 0.95 ? 0.95 : z) * EffectiveDepth;
        step.Add($"Thereform, z = {z}");

        double xu = (EffectiveDepth - z) / 0.4;
        if (xu / EffectiveDepth >= 0.45)
        {
            step.Add("design again!!");
            return step;
        }

        double requirdAs = Moment / (0.87 * fy * z) * Math.Pow(1000, 2);
        step.Add($"As,requird = {requirdAs:0.000} mm²");

        double fctm = 3.2;
        double minAsRatio = 0.26 * fctm / fy * 100;
        step.Add($"As,min \t= {minAsRatio:0.000}%*b*d");

        if (IsRequirement(minAsRatio))
        {
            double minAs = minAsRatio / 100 * breath * EffectiveDepth;
            requirdAs = Math.Max(requirdAs, minAs);
            step.Add($"\t= {minAsRatio:0.000}*{breath}*{EffectiveDepth}");
            step.Add($"\t= {minAs:0.000}");
        }

        step.Add($"Thus, As,requird = {requirdAs:0.000} mm²\n");
        ProviderArea = StructuralComponent.LongSpanRebarArguments.GetRebarArea();
        step.Add($"As,provider = {ProviderArea:0.000}");
        if (ProviderArea <= requirdAs)
        {
            return step.Fail("design again!!");
        }

        double steelRatio = ProviderArea / StructuralComponent.Thickness / breath;

        step.Add($"Steel Ratio = {ProviderArea:0.00}/{StructuralComponent.Thickness}/{breath}");
        step.Add($"\t= {steelRatio * 100:0.000}%");

        return IsRequirement(steelRatio * 100) ? step.Success("Ok") : step.Fail("design again!!!");
    }

    public override Step DesignMoment()
    {
        Step step = base.DesignMoment();
        double bsx = MathKit.Forecast(LongSpan / ShortSpan,
            [1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.75, 2],
            [0.055, 0.065, 0.074, 0.081, 0.087, 0.092, 0.103, 0.111]);

        double bsy = 0.056;

        double shortSpanMoment = Math.Pow(ShortSpan, 2) * bsx * TotalPressure;
        double longSpanMoment = Math.Pow(ShortSpan, 2) * bsy * TotalPressure;


        step.Add($"Bsx = {bsx:0.000}");
        step.Add($"Bsy = {bsy}");

        step.Add($"Short span Moment = {shortSpanMoment:0.000}");
        step.Add($"Long span Moment = {longSpanMoment:0.000}");

        Moment = Math.Max(shortSpanMoment, longSpanMoment);



        return step.Success($"Dsesign Moment = {Moment:0.00}");
    }

    public override Step DesignShear()
    {
        Step step = base.DesignShear();
        double bvx = MathKit.Forecast(LongSpan / ShortSpan,
            [1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.75, 2],
            [0.33, 0.36, 0.39, 0.41, 0.43, 0.45, 0.48, 0.5]);

        double bvy = 0.330;

        double vsx = bvx * TotalPressure * ShortSpan;
        double vsy = bvy * TotalPressure * ShortSpan;

        Shear = Math.Max(vsx, vsy);

        return step.Success($"Design Shear = {Shear:N3}");
    }

    public override Step Loading()
    {
        Step step = new Step("Loading");

        var deadLoad = 0.0;
        var liveLoad = 0.0;

        foreach (var item in Loadings)
        {
            switch (item.LoadType)
            {
                case LoadType.UniformlyDistributedLoad:
                    deadLoad += item.Weight;
                    break;
                case LoadType.TriangularLoad:
                    liveLoad += item.Weight;
                    break;
            }
        }

        TotalPressure = (deadLoad * Properties.Settings.Default.FactorOfDeadLoad + liveLoad * Properties.Settings.Default.FactorOfLiveLoad) / (ShortSpan * LongSpan);
        return step.Success($"Factored Total Pressure = {TotalPressure:N3}");
    }

    public override Step ShearReinforcement()
    {
        Step step = base.ShearReinforcement();
        double d = EffectiveDepth;

        double fck = StructuralComponent.ConcreteArguments.Fck;
        double k = 1 + Math.Sqrt(200 / d);

        double r = ProviderArea / (1000 * d);

        double vrdc = 0.12 * Math.Min(k, 2) * 1000 * d * Math.Pow(100 * r * fck, 1.0 / 3.0) / 1000;

        step.Add($"where k = {k:N3}  , d = {d}");
        step.Add($"Vrdc = {vrdc:N3} compare with {Shear:N3}");

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
}
