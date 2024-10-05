//  ©️ 2024 by RF At EggNine All Rights Reserved
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Eggnine.Rps.Web.Extensions;

public static class HttpContextExtensions
{
    public static async Task<RpsUser?> GetUserFromCookieAsync(this HttpContext httpContext, IUserRepository users,
        ILogger logger, CancellationToken cancellationToken = default)
    {
        string? idString = await Task.Run(() => 
            httpContext.Request.Cookies.FirstOrDefault(c => c.Key.Equals(Constants.UserSessionCookieKey)).Value);
        if(idString is null)
        {
            logger.LogWarning("User id not found in cookies");
            return null;
        }
        Guid id = new Guid(idString);
        logger.LogDebug("Retrieved id {Id} from cookie", id);
        return await users.GetAsync(u => u.GetId().Equals(id), cancellationToken);
    }
}