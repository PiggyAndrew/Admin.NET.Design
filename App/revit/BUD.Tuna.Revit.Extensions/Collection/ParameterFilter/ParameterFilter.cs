using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Tuna.Revit.Extensions.Collection.ParameterFilter;

internal class ParameterFilter
{
    protected ParameterFilter()
    {

    }

    public static ParameterFilter OfCategory()
    {
        return new ParameterFilter();
    }

    public static ParameterFilter OfType()
    {
        return new ParameterFilter();
    }

    public ParameterFilterRule Parameter(ElementId parameterId)
    {
        return new ParameterFilterRule(this);
    }
}
