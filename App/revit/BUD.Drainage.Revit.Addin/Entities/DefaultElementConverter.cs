using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Drainage.Revit.Addin.Entities;

internal class DefaultElementConverter : IElementConvetrer<Element>
{
    public Element ConvertBack(IElementWrapper element)
    {
        var wrapper = element as IElementWrapper<Element>;
        Element originalElement = wrapper!.RvtObject;




        return originalElement;
    }

    public IElementWrapper ConvertTo(Element element)
    {
        throw new NotImplementedException();
    }
}
