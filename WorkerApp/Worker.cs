using Microsoft.Data.SqlClient;

namespace WorkerApp;

public class Worker(ILogger<Worker> logger, IConfiguration configuration) : BackgroundService
{
    private int iteration = 0;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (logger.BeginScope(new Dictionary<string, object>
                   {
                       { "RequestId", Guid.NewGuid() },
                       { "CorrelationId", Guid.NewGuid() }
                   }))
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                if(iteration % 2 == 0)
                {
                    await PerformSqlLookupAsync();
                }
                iteration++;

                await Task.Delay(10000, stoppingToken);
            }
        }
    }

    private async Task PerformSqlLookupAsync()
    {
        await using var connection = new SqlConnection(configuration["SQL_CONNECTION_STRING"]);
        await connection.OpenAsync();
        await using var command = new SqlCommand("SELECT TOP 5 name AS ObjectName, type_desc AS ObjectType, create_date FROM sys.objects ORDER BY NEWID();", connection);
        await using var reader = await command.ExecuteReaderAsync();
    }
}