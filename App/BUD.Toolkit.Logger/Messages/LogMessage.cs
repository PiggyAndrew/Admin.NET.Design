using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Toolkit.Logger.Messages;

public class LogMessage
{
    public LogMessage() { }

    public LogMessage(string message)
    {
        Message = message;
    }

    public LogMessage(string message, LogLevel logLevel)
    {
        Message = message;
        LogLevel = logLevel;
    }

    public DateTime CreateTime { get; set; } = DateTime.Now;

    public LogLevel LogLevel { get; set; }

    public string Message { get; set; }

    public override string ToString()
    {
        return $"> {CreateTime:g}\t[{LogLevel}] : {Message}";
    }
}
