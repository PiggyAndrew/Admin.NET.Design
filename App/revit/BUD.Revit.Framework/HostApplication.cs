using Autodesk.Revit.UI;
using DSD.Toolkit.Logger;
using DSD.Toolkit.MVVM.DialogServices;
using BUD.Tuna.Revit.Extensions.ExternalEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUD.Revit.Framework;

public class HostApplication : IDisposable
{
    public static HostApplication Current { get; internal set; } = default!;

    internal static HostApplication CreatHostAppllication(UIControlledApplication application)
    {
        Current = new HostApplication()
        {
            ApplicationContext = new HostApplicationContext(application),
        };

        return Current;
    }

    public HostApplication()
    {
        ExternalEventService = new ExternalEventService();
        DialogRegistry = new DialogService();
    }

    public HostApplicationContext ApplicationContext { get; set; } = default!;

    public ILogger Logger { get; internal set; } = default!;

    public IDialogService DialogService { get; internal set; } = default!;

    public IExternalEventService ExternalEventService { get; }

    public IDialogRegistry DialogRegistry { get; }

    public void Dispose()
    {
        if (Logger is Logger logger)
        {
            logger.Dispose();
        }
    }
}
