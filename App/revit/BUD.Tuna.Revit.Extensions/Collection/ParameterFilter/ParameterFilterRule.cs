using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Tuna.Revit.Extensions.Collection.ParameterFilter;

internal class ParameterFilterRule
{
    private ParameterFilter filter;

    public ParameterFilterRule(ParameterFilter parameterFilter)
    {

    }

    public ParameterFilter EqualsTo(string value)
    {
        return filter;
    }

    public ParameterFilter EqualsTo(int value)
    {
        return filter;
    }

    public ParameterFilter EqualsTo(double value)
    {
        return filter;

    }

    public ParameterFilter EqualsTo(ElementId value)
    {
        return filter;
    }
}
