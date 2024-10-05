//  ©️ 2024 by RF At EggNine All Rights Reserved
using Microsoft.Extensions.DependencyInjection;
using Eggnine.Rps.Core;

namespace Eggnine.Rps.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRpsCore(this IServiceCollection services)
        {
            services.AddSingleton<IRpsEngine,RpsEngine>();
            return services;
        }
        public static IServiceCollection AddRpsUsers(this IServiceCollection services)
        {
            services.AddSingleton<IUserRepository, UserRepository>();
            return services;
        }
    }
}