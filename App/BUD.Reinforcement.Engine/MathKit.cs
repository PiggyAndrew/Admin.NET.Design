using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Reinforcement.Engine;

public static class MathKit
{
    /// <summary>
    /// Predicts the new y value for a given x value ,
    /// Uses linear regression to predict a new y value based on known x and y data points
    /// </summary>
    /// <param name="key">The x value for which the prediction is needed</param>
    /// <param name="xValues">Array of known x values</param>
    /// <param name="yValues">Array of known y values</param>
    /// <returns>The predicted y value</returns>
    /// <exception cref="ArgumentException">Thrown if xValues and yValues have different lengths or are empty</exception>
    public static double Forecast(double key, double[] xValues, double[] yValues)
    {
        double xLength = xValues.Length;
        double yLength = yValues.Length;

        if (xLength != yLength || xLength == 0)
        {
            throw new ArgumentException("xValues and yValues must be non-empty arrays of the same length");
        }

        double x = xValues.Average();
        double y = yValues.Average();


        var bounds = yValues.Select((value, i) => new { Value = value, Index = i })
            .Aggregate(new { Top = 0.0, Bottom = 0.0 }, (acc, cur) =>
            {
                int index = cur.Index;
                double diffValue = xValues[index] - x;

                return new
                {
                    Top = acc.Top + diffValue * (yValues[index] - y),
                    Bottom = acc.Bottom + Math.Pow(diffValue, 2)
                };
            });

        var level = bounds.Top / bounds.Bottom;
        return y - level * x + level * key;
    }

    public static double RadiansToDegrees(double radians)
    {
        return radians / Math.PI * 180;
    }

    public static double DegreesToRadians(double degrees)
    {
        return degrees / 180 * Math.PI;
    }

    public static double Cot(double angle)
    {
        return 1 / Math.Tan(angle);
    }

    public static double Area(double diameter)
    {
        return Math.PI * Math.Pow(diameter / 2, 2);
    }
}
