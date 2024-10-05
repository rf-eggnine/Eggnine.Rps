//  ©️ 2024 by RF At EggNine All Rights Reserved
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NReco.Logging.File;
using System;
using System.Threading.Tasks;
using Eggnine.Rps.Web.Extensions;
using Microsoft.Extensions.Logging;

namespace Eggnine.Rps.Web
{
    public class Program
    {
        public static async Task Main(string[] args) => await Startup(args).RunAsync();

        public static WebApplication Startup(string[] args, WebApplicationBuilder? builder = null)
        {
            builder ??= WebApplication.CreateBuilder(args);

            builder.Services.AddAntiforgery();
            builder.Services.AddAuthentication().AddCookie();
            builder.Services.AddAuthorization();
            builder.Services.AddRpsCore();
            builder.Services.AddRpsUsers();
            builder.Services.AddRazorPages();
            builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddFile("rps.log", append:true));
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => (!(context.User as RpsUser)?.HasAcceptedCookies) ?? true;
                options.ConsentCookie.Domain = "eggnine.com";
                options.ConsentCookie.Name = "CookieConsent";
            });
            builder.WebHost.ConfigureKestrel(options => 
                        options.AddServerHeader = false);

            if(builder.Environment.IsDevelopment())
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
            }
            else
            {
                builder.Services.AddHsts(options =>
                {
                    options.Preload = true;
                    options.IncludeSubDomains = true;
                    options.MaxAge = TimeSpan.FromDays(60);
                });
            }
            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                CheckConsentNeeded = context => 
                {
                    if(bool.TryParse(context.Request.Headers[Constants.HeaderAcceptedCookies], out bool headerAcceptedCookies)
                        && headerAcceptedCookies)
                    {
                        app.Logger.LogDebug("header acceptedCookies true");
                        return true;
                    }
                    if(bool.TryParse(context.Request.Query[Constants.QueryStringKeyAcceptedCookies], out bool acceptCookies)
                        && acceptCookies)
                    {
                        app.Logger.LogDebug("query string acceptedCookies true");
                        return true;
                    }
                    if(context.Request.Cookies.ContainsKey(Constants.UserSessionCookieKey))
                    {
                        app.Logger.LogDebug("cookie already set");
                        return true;
                    }
                    app.Logger.LogDebug("cookes not accepted");
                    return false;
                }
            });
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAntiforgery();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            return app;
        }
    }
}