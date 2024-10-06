//  ©️ 2024 by RF At EggNine All Rights Reserved
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Eggnine.Rps.Web;
public interface IUserRepository : IRepository<RpsUser>
{
    public Task<RpsUser?> SignInUserAsync(HttpContext httpContext, bool acceptedCookies = false, 
        CancellationToken cancellationToken = default);
    public Task<RpsUser?> SignInUserAsync(HttpContext httpContext, 
        Guid id, CancellationToken cancellationToken = default);
}