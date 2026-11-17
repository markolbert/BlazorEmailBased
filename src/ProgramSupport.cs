using System.Runtime.InteropServices;
using System.Text;
using J4JSoftware.BlazorEmailBased.Middleware;
using J4JSoftware.BlazorEmailBased.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace J4JSoftware.BlazorEmailBased;

public static class ProgramSupport
{
    public static void PreBuild()
    {
        // added to explicitly include required code pages under linux
        if( RuntimeInformation.IsOSPlatform( OSPlatform.Linux ) )
            Encoding.RegisterProvider( CodePagesEncodingProvider.Instance );
    }

    public static WebApplicationBuilder AddServices<TUser, TDbContext>(
        this WebApplicationBuilder builder,
        string keyPath,
        string appName,
        bool enableEfLogging = false
    )
        where TUser : IdentityUser<int>, IAdministrator
        where TDbContext : DbContext
    {
        // Add this to persist keys to a special directory
        // under linux.
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            builder.Services.AddDataProtection()
                   .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
                   .SetApplicationName(appName);
        }

        var siteSettingsSection = builder.Configuration.GetSection(SiteSettings.SectionName);
        builder.Services.Configure<SiteSettings>( siteSettingsSection );
        builder.Services.AddHostedService<SiteSettingsChangeListener<TUser, TDbContext>>();

        var siteSettings = siteSettingsSection.Get<SiteSettings>()
            ?? throw new InvalidOperationException($"Missing configuration for {SiteSettings.SectionName}");

        // Add services to the container.
        builder.Services.AddRazorComponents()
               .AddInteractiveServerComponents();

        builder.Services.AddScoped<IEmailService, GraphEmailService>();
        builder.Services.AddScoped(typeof(IHtmlEmailTemplateService), typeof(HtmlEmailTemplateService));

        builder.Services.AddDbContextFactory<TDbContext>( options =>
        {
            options.UseSqlite( siteSettings.DefaultConnection );

            if( !enableEfLogging )
                return;

            options.LogTo( Console.WriteLine, [DbLoggerCategory.Database.Command.Name], LogLevel.Information )
                   .EnableSensitiveDataLogging();
        } );

        builder.Services.AddCascadingAuthenticationState();

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(AdministratorRequirement.PolicyName,
                               policy =>
                                   policy.RequireAuthenticatedUser()
                                         .AddRequirements(new AdministratorRequirement()));

            options.AddPolicy(ConditionalAdminRequirement.PolicyName,
                               p =>
                                   p.RequireAuthenticatedUser()
                                    .AddRequirements(new ConditionalAdminRequirement()));
        });

        builder.Services.AddScoped<IAuthorizationHandler, AdministratorAuthorizationHandler<TUser>>();
        builder.Services.AddScoped<IAuthorizationHandler, ConditionalAdminHandler>();

        builder.Services.AddIdentityCore<TUser>(options => { options.SignIn.RequireConfirmedAccount = true; })
               .AddRoles<IdentityRole<int>>()
               .AddEntityFrameworkStores<TDbContext>()
               .AddDefaultTokenProviders()
               .AddTokenProvider<PasswordlessLoginTokenProvider<TUser>>(
                    PasswordlessLoginTokenProvider<TUser>.ProviderName)
               .AddSignInManager();

        builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
               .AddIdentityCookies();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(siteSettings.LoginLinkLifetimeMinutes);
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.LoginPath = "/";
            options.AccessDeniedPath = "/Static/unauthorized.html";

            options.Events.OnRedirectToLogin = ctx =>
            {
                if (IsBlazorSignalROrApi(ctx.HttpContext))
                    ctx.HttpContext.Response.StatusCode = 401;
                else ctx.HttpContext.Response.Redirect(ctx.RedirectUri);

                return Task.CompletedTask;
            };

            options.Events.OnRedirectToAccessDenied = ctx =>
            {
                if (IsBlazorSignalROrApi(ctx.HttpContext))
                    ctx.HttpContext.Response.StatusCode = 403;
                else ctx.HttpContext.Response.Redirect(ctx.RedirectUri);

                return Task.CompletedTask;
            };
        });

        builder.Services.AddHttpContextAccessor();

        // only needed to run under debian
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownIPNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }

        builder.Services.AddScoped<ILoginTokenService<TUser>, LoginTokenService<TUser>>();
        builder.Services.AddScoped<IDeliveryEmailService<TUser>, DeliveryEmailService<TUser>>();

        return builder;
    }

    public static WebApplication ConfigureWebApplication(this WebApplicationBuilder builder)
    {
        var app = builder.Build();

        // using a reverse proxy on debian trixie obviates the
        // need to have asp.net handle https redirection
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            app.UseForwardedHeaders(); // FIRST

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", true);
            app.UseHsts();
        }

        // DO NOT UseHttpsRedirection() on Linux behind proxy
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            app.UseHttpsRedirection();

        // Keep MapStaticAssets for your CSS/JS frameworks
        app.MapStaticAssets();

        // Fall back to standard disk serving for your custom HTML pages
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(builder.Environment.ContentRootPath, "Static")),
            RequestPath = "/Static"
        });

        app.UseMiddleware<MaintenanceMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();

        return app;
    }

    private static bool IsBlazorSignalROrApi( HttpContext ctx )
    {
        if( ctx.Request.Path.StartsWithSegments( "/_blazor" ) )
            return true;

        if( string.Equals( ctx.Request.Headers[ "X-Requested-With" ],
                           "XMLHttpRequest",
                           StringComparison.OrdinalIgnoreCase ) )
            return true;

        if( ctx.Request.Headers.Accept.ToString().Contains( "application/json", StringComparison.OrdinalIgnoreCase ) )
            return true;

        return false;
    }
}
