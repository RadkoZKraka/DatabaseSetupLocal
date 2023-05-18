namespace DatabaseSetupLocal.Library;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class LoggingService : BackgroundService
{
    private string logFolder;
    private string logFilePath;

    public LoggingService()
    {
        UpdateLogFilePath();
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Update the log folder and file path
            UpdateLogFilePath();
            await AppendToLogFile("eldo");
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private void UpdateLogFilePath()
    {
        var currentLogFolder = "logs"; // Replace with the desired log folder path
        Directory.CreateDirectory(currentLogFolder);

        var currentDate = DateTime.Now.ToString("yyyyMMdd"); // Format the current date as yyyyMMdd
        var currentLogFilePath = Path.Combine(currentLogFolder, $"{currentDate}.txt");

        if (logFilePath != currentLogFilePath)
        {
            // Create a new log file for the current date if it doesn't exist
            if (!File.Exists(currentLogFilePath))
            {
                File.Create(currentLogFilePath).Dispose();
            }

            logFolder = currentLogFolder;
            logFilePath = currentLogFilePath;
        }
    }

    public async Task AppendToLogFile(string message)
    {
        try
        {
            
            await using (var writer = new StreamWriter(logFilePath, true))
            {
                await writer.WriteLineAsync(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to log file: {ex.Message}");
        }
    }
}
