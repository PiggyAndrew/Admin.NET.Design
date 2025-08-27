using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Drainage.Revit.Addin.Converter;

internal class StatusColors
{
    public static System.Drawing.Color Failed { get; } = System.Drawing.Color.FromArgb(255, 77, 79);

    public static System.Drawing.Color Waiting { get; } = System.Drawing.Color.FromArgb(250, 173, 20);

    public static System.Drawing.Color Done { get; } = System.Drawing.Color.FromArgb(82, 196, 26);

    public static System.Drawing.Color Running { get; } = System.Drawing.Color.FromArgb(162, 110, 244);
}
