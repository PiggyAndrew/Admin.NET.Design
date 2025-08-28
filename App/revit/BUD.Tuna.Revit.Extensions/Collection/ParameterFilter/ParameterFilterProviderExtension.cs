using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Tuna.Revit.Extensions.Collection.ParameterFilter
{
    internal static class ParameterFilterProviderExtension
    {
        public static ParameterFilterProvider GetElements(this Document document)
        {

            return new ParameterFilterProvider(document);
        }
    }
}
