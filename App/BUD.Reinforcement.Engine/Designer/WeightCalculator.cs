using DSD.Reinforcement.Engine.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine.Designer;

/// <summary>
/// Loadings calculator
/// </summary>
public class WeightCalculator
{
    /// <summary>
    /// Calculate the pressure of soil
    /// </summary>
    /// <param name="depth">Depth at wall toe</param>
    /// <returns>The pressure of soil</returns>
    public static double CalculatePressureOfSoil(double depth)
    {
        /// <summary>
        /// Calculate the coefficient of earth pressure at rest 
        /// </summary>
        /// <param name="angle">Angle of resistance for soil</param>
        /// <returns>The coefficient of earth pressure at rest </returns>
        static double CalculateEarthPressureAtRest(double angle) => 1 - Math.Sin(angle * Math.PI / 180);


        double ko = CalculateEarthPressureAtRest(Properties.Settings.Default.VolumetricWeightOfSoil);
        double volumetricWeight = Properties.Settings.Default.VolumetricWeightOfSoil - Properties.Settings.Default.VolumetricWeightOfWater;
        return ko * volumetricWeight * depth / 1000;
    }

    /// <summary>
    /// Calculate the pressure of water
    /// </summary>
    /// <param name="depth">Depth at wall toe</param>
    /// <returns>The pressure of water</returns>
    public static double CalculatePressureOfWater(double depth) => Properties.Settings.Default.VolumetricWeightOfWater * depth / 1000;

    /// <summary>
    /// Calculate pressure of surcharge
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static double CalculatePressureOfSurcharge(double value) => value * 0.500;
}
