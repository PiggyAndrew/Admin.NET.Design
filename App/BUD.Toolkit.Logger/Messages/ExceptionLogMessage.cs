using DSD.Toolkit.Logger.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Toolkit.Logger;

public class ExceptionLogMessage : LogMessage
{
    public ExceptionLogMessage(Exception exception)
    {
        this.LogLevel = LogLevel.Error;
        Message = $"{exception.GetType()}:\t{exception.Message}\n===Exception Detail===\n{exception.StackTrace}";
    }
}
