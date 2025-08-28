using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSD.Toolkit.Logger;
using DSD.Toolkit.Logger.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BUD.Revit.Framework;

public abstract class CommandBase : IExternalCommand, IExternalCommandAvailability
{
    protected CommandContext CommandContext { get; private set; } = default!;

    public abstract CommandResult Execute();
    //public abstract  Task<CommandResult> ExecuteAsync();

    [DebuggerStepThrough]
    public  Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        Type commandType = GetType();

        StringBuilder log = new StringBuilder();
        log.AppendLine($"Invoke {commandType}");
        log.AppendLine("--- Command Start ---");
        LogLevel logLevel = LogLevel.Information;


        CommandResult result = new CommandResult();
        try
        {
            CommandContext = new CommandContext(commandData.Application);
            CommandContext.Attach();

            result = Execute();

            if (!string.IsNullOrEmpty(result.Message))
            {
                message = result.Message;
            }

            log.AppendLine($"Message: {result.Message}");
        }
        catch (Exception ex)
        {
            ExceptionLogMessage exceptionLogMessage = new ExceptionLogMessage(ex);
            log.AppendLine($"Message: {exceptionLogMessage.Message}");
            logLevel = exceptionLogMessage.LogLevel;

            message = ex.Message;
            result.ResultStatus = Result.Failed;
        }
        finally
        {
            CommandContext.Dispose();
        }


        log.AppendLine($"Result: {result.ResultStatus}");
        log.AppendLine("--- Command End ---");

        HostApplication.Current.Logger.Log(new LogMessage($"{log}", logLevel));
        return result.ResultStatus;
    }

    [DebuggerStepThrough]
    public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
    {
        CommandContext = new CommandContext(applicationData);

        return CanExecute();
    }

    public virtual bool CanExecute()
    {
        return CommandContext.IsVaildDocument;
    }

    protected CommandResult Succeeded()
    {
        return new CommandResult()
        {
            ResultStatus = Result.Succeeded,
        };
    }

    protected CommandResult Succeeded(string message)
    {
        return new CommandResult()
        {
            ResultStatus = Result.Succeeded,
            Message = message
        };
    }

    protected CommandResult Failed()
    {
        return new CommandResult()
        {
            ResultStatus = Result.Failed,
        };
    }

    protected CommandResult Failed(string message)
    {
        HostApplication.Current.Logger.Error(message);
        return new CommandResult()
        {
            ResultStatus = Result.Failed,
            Message = message
        };
    }

    protected CommandResult Cancelled()
    {
        return new CommandResult()
        {
            ResultStatus = Result.Cancelled,
        };
    }

    protected CommandResult Cancelled(string message)
    {
        return new CommandResult()
        {
            ResultStatus = Result.Cancelled,
            Message = message
        };
    }
}
