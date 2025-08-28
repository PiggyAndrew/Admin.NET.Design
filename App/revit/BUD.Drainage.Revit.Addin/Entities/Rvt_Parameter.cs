using Autodesk.Revit.DB;
using BUD.Tuna.Revit.Extensions.Extensions;
using DSD.Toolkit.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Drainage.Revit.Addin.Entities;

class Rvt_Parameter<T> : ObservableObject
{
    private T? _value;

    private Rvt_Parameter(Parameter parameter)
    {
        IsReadOnly = parameter.IsReadOnly;
        RvtObject = parameter;
    }

    public Parameter RvtObject { get; }

    public bool IsReadOnly { get; }

    public T? Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }

    public static Rvt_Parameter<double> IsDouble(Parameter parameter)
    {
        var result = new Rvt_Parameter<double>(parameter);
        result.Value = parameter.AsDouble().ConvertToMillimeters(); ;

        return result;
    }
}