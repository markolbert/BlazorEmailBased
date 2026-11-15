using J4JSoftware.BlazorEmailBased.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace J4JSoftware.BlazorEmailBased;

public sealed class SiteSettingsChangeListener<TUser, TDbContext>(
    IOptionsMonitor<SiteSettings> settings,
    IDbContextFactory<TDbContext> dbContextFactory
    ) : IHostedService, IDisposable
    where TUser : IdentityUser<int>, IAdministrator
    where TDbContext : DbContext
{
    private IDisposable? _subscription;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var dbContext = dbContextFactory.CreateDbContext();
        var userSet = dbContext.Set<TUser>();

        string[] adminEmails;

        if (userSet == null)
            adminEmails = [];
        else
        {
            if (userSet.Any(u => u.IsAdministrator && u.Email != null))
                adminEmails = userSet.Where(u => u.IsAdministrator).Select(u => u.Email!).ToArray();
            else adminEmails = [];
        }

        settings.CurrentValue.OnSettingsChanged(adminEmails);

        _subscription = settings.OnChange(value => value.OnSettingsChanged(adminEmails));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _subscription?.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose() => _subscription?.Dispose();
}

