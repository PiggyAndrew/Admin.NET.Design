using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Revit.Framework;

public class CommandResult
{
    public Result ResultStatus { get; set; }

    public string Message { get; set; }
}
