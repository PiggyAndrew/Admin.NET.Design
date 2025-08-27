using DSD.Toolkit.Logger.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Toolkit.Logger;

public static class LoggerExtensions
{
    /// <summary>
    /// Create a information log message
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="message"></param>
    public static void Information(this ILogger logger, object message) => logger.Log(new LogMessage(message.ToString(), LogLevel.Information));

    /// <summary>
    /// Create a error log message
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="message"></param>
    public static void Error(this ILogger logger, string message) => logger.Log(new LogMessage(message, LogLevel.Error));

    /// <summary>
    /// Create a wrinning log message
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="message"></param>
    public static void Warning(this ILogger logger, string message) => logger.Log(new LogMessage(message, LogLevel.Warning));

    /// <summary>
    /// Create an error log message
    /// </summary>
    /// <param name="logger">logger host</param>
    /// <param name="exception">exception on the context</param>
    public static void Exception(this ILogger logger, Exception exception) => logger.Log(new ExceptionLogMessage(exception));

}
