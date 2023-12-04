using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StajyerTakipSistemi.Web.Models;

public class BackgroundWorkerService : BackgroundService
{
    private readonly ILogger<BackgroundWorkerService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Service started.");

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<StajyerTakipSistemiDbContext>();

                var interns = await dbContext.SInterns.ToListAsync();

                foreach (var intern in interns)
                {
                    var yesterday = DateTime.UtcNow.AddDays(-1).Date;
                    if (yesterday.DayOfWeek != DayOfWeek.Saturday && yesterday.DayOfWeek != DayOfWeek.Sunday)
                    {
                        var hasReportForYesterday = await dbContext.SDailyReports
                            .AnyAsync(r => r.InternId == intern.Id && DateTime.UnixEpoch.AddSeconds(r.UnixTime).Date == yesterday);

                        if (!hasReportForYesterday)
                        {
                            var hasAbsenceEntryForYesterday = await dbContext.SAbsenceInformations
                                .AnyAsync(r => r.InternId == intern.Id && r.AbsenceDate == yesterday);

                            if (!hasAbsenceEntryForYesterday)
                            {
                                var absenceInfo = new SAbsenceInformation { InternId = intern.Id, AbsenceDate = yesterday };
                                dbContext.SAbsenceInformations.Add(absenceInfo);
                                await dbContext.SaveChangesAsync();
                            }
                        }
                    }
                    if (intern.EndDate.Date==yesterday.Date)
                    {
                        intern.AccessStatus = "Not active";
                        dbContext.Update(intern);
                        await dbContext.SaveChangesAsync();
                    }
                    
                    
                }
            }

            await Task.Delay(86400000, stoppingToken); // 24 hours delay
        }
    }
}
