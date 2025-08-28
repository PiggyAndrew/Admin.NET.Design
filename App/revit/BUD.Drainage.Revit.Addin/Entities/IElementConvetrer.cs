using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Drainage.Revit.Addin.Entities;

internal interface IElementConvetrer<TElement>
{
    public IElementWrapper ConvertTo(TElement element);

    public TElement ConvertBack(IElementWrapper element);
}
