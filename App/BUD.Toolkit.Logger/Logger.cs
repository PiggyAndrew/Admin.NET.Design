using DSD.Toolkit.Logger.Messages;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DSD.Toolkit.Logger;

public class Logger : ILogger, IDisposable
{
    private readonly ConcurrentQueue<LogMessage> _messageQueue;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Task _processQueueTask;
    private readonly AutoResetEvent _messageEnqueuedEvent;

    public static ILogger InitializeLogger(Action<LoggerOptions> handle)
    {
        LoggerOptions options = new LoggerOptions();
        handle(options);

        DirectoryInfo directoryInfo = Directory.CreateDirectory(Path.Combine(options.LogDirectory, "log"));
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        string fileName = options.FileNamePattern
            .Replace("{date}", DateTime.Now.ToString("yyyyMMdd"))
            .Replace("{time}", DateTime.Now.ToString("HHmmss"))
            .Replace("{guid}", Guid.NewGuid().ToString("N").Substring(0, 8));

        var loggerFilePath = Path.Combine(directoryInfo.FullName, $"DSD_{fileName}");

        return new Logger(loggerFilePath);
    }

    private Logger(string loggerFilePath)
    {
        LogPath = loggerFilePath;
        _messageQueue = new ConcurrentQueue<LogMessage>();
        _cancellationTokenSource = new CancellationTokenSource();
        _messageEnqueuedEvent = new AutoResetEvent(false);

     
        _processQueueTask = Task.Run(ProcessQueueAsync);
    }

    private string LogPath { get; }

    public void Log(LogMessage message)
    {
        _messageQueue.Enqueue(message);
        _messageEnqueuedEvent.Set(); 
    }

    private async Task ProcessQueueAsync()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                await Task.Run(() => _messageEnqueuedEvent.WaitOne(1000));

                await ProcessQueuedMessages();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error processing log queue: {ex.Message}");
            }
        }
    }

    private async Task ProcessQueuedMessages()
    {
        if (_messageQueue.IsEmpty)
        {
            return;
        }  

        try
        {
            using (StreamWriter writer = new StreamWriter(File.Open(LogPath, FileMode.Append, FileAccess.Write, FileShare.Read)))
            {
                while (_messageQueue.TryDequeue(out LogMessage message))
                {
                    await writer.WriteLineAsync(message.ToString());
                }
                await writer.FlushAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error writing to log file: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _messageEnqueuedEvent.Set(); 
        
        try
        {
            _processQueueTask.Wait(1000); 
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during logger disposal: {ex.Message}");
        }

        _cancellationTokenSource.Dispose();
        _messageEnqueuedEvent.Dispose();
    }
}
