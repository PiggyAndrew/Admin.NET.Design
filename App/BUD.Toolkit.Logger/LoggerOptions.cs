using System;

namespace DSD.Toolkit.Logger;

/// <summary>
/// Configuration options for logger
/// </summary>
public class LoggerOptions
{
    /// <summary>
    /// Enable debug output
    /// </summary>
    public bool EnableDebug { get; set; } = true;

    /// <summary>
    /// Enable file logging
    /// </summary>
    public bool EnableFileLog { get; set; } = true;

    /// <summary>
    /// Log file directory path
    /// </summary>
    public string LogDirectory { get; set; }

    /// <summary>
    /// Log file name pattern
    /// </summary>
    public string FileNamePattern { get; set; } = "dsd_log.txt";
} 