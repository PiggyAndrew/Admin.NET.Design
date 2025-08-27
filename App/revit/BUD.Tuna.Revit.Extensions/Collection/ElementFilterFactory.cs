using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Tuna.Revit.Extensions.Collection;

public static class ElementFilterFactory
{
    public static LogicalAndFilter LogicalAnd(params ElementFilter[] elementFilters)
    {
        return new LogicalAndFilter(elementFilters);
    }

    //public static ElementParameterFilter ParameterEqualsTo(ElementId parameterId, string value)
    //{
    //    return new ElementParameterFilter(ParameterFilterRuleFactory.CreateEqualsRule(parameterId, value));
    //}

    public static ElementParameterFilter ParameterEqualsTo(ElementId parameterId, int value)
    {
        return new ElementParameterFilter(ParameterFilterRuleFactory.CreateEqualsRule(parameterId, value));
    }

    public static ElementParameterFilter ParameterNotEqualsTo(ElementId parameterId, int value)
    {
        return new ElementParameterFilter(ParameterFilterRuleFactory.CreateNotEqualsRule(parameterId, value));
    }

    public static ElementParameterFilter ParameterNotEqualsTo(ElementId parameterId, ElementId value)
    {
        return new ElementParameterFilter(ParameterFilterRuleFactory.CreateNotEqualsRule(parameterId, value));
    }

    public static BoundingBoxIntersectsFilter IntersectsWith(Outline outline)
    {
        return new BoundingBoxIntersectsFilter(outline);
    }

    public static BoundingBoxIsInsideFilter InsideTheBoundingBox(BoundingBoxXYZ boundingBox)
    {
        return new BoundingBoxIsInsideFilter(new Outline(boundingBox.Min, boundingBox.Max));
    }
}
