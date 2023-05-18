using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class BackupService : BackgroundService
{
    private readonly string sourceFilePath;
    private readonly string backupFolderPath;

    public BackupService()
    {
        sourceFilePath = "shots.db"; // Replace with the actual path of the shots.db file
        backupFolderPath = "backups"; // Replace with the desired backup folder path
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Perform the backup operation
            await BackupFile();

            // Wait for 30 seconds before the next backup
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task BackupFile()
    {
        // Create the backup folder if it doesn't exist
        Directory.CreateDirectory(backupFolderPath);

        try
        {
            var backupFileName = $"backup_{DateTime.Now:yyyyMMddHHmmss}.db";
            var backupFilePath = Path.Combine(backupFolderPath, backupFileName);

            using (var sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
            using (var destinationStream = new FileStream(backupFilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await sourceStream.CopyToAsync(destinationStream);
            }

            Console.WriteLine($"Backup created: {backupFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating backup: {ex.Message}");
        }
    }
}