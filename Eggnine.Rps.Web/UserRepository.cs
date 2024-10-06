//  ©️ 2024 by RF At EggNine All Rights Reserved
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Eggnine.Rps.Core;
using Eggnine.Rps.Web.Extensions;

namespace Eggnine.Rps.Web;

internal class UserRepository : IUserRepository
{
    private readonly SemaphoreSlim _semaphore = new(1);
    private readonly IList<RpsUser> _users = new List<RpsUser>();
    private readonly ILogger _logger;
    private readonly IRpsEngine _engine;

    public UserRepository(IRpsEngine engine, ILogger<UserRepository> logger)
    {
        _engine = engine;
        _logger = logger;
    }
    
    public async Task<RpsUser?> SignInUserAsync(HttpContext httpContext, bool acceptedCookies = false, 
        CancellationToken cancellationToken = default)
    {

        try
        {
            RpsUser? rpsUser = await httpContext.GetUserFromCookieAsync(this, _logger, cancellationToken);
            if(rpsUser is null)
            {
                rpsUser = new RpsUser(Guid.NewGuid(), acceptedCookies, _engine);
                rpsUser.HasAcceptedCookies = acceptedCookies;
                if(await AddAsync(rpsUser, cancellationToken))
                {
                    _logger.LogInformation("User added with id {Id}", rpsUser.Id);
                    await httpContext.AuthenticateAsync();
                    await Task.Run(async () => await httpContext.SignInAsync(rpsUser), cancellationToken);
                    _logger.LogInformation("User signed in");
                    httpContext.Response.Cookies.Append(Constants.UserSessionCookieKey, rpsUser.GetId().ToString());
                    _logger.LogInformation("set cookie with value {id}", rpsUser.GetId());
                    return rpsUser;
                }
                _logger.LogWarning("User not signed in with id {Id}", rpsUser.Id);
                return null;
            }
            _logger.LogInformation("Initial user was not null");
            return rpsUser;
        }
        finally
        {
            _logger.LogTrace("Exiting {MethodName}", nameof(SignInUserAsync));
        }
    }
    public async Task<RpsUser?> SignInUserAsync(HttpContext httpContext, Guid id, 
        CancellationToken cancellationToken = default)
    {
        await httpContext.SignOutAsync();
        RpsUser? rpsUser = await GetAsync(u => u.Id.Equals(id));
        if(rpsUser is not null)
        {
            _logger.LogInformation("User found with id {Id}", rpsUser.Id);
            await httpContext.AuthenticateAsync();
            await Task.Run(async () => await httpContext.SignInAsync(rpsUser), cancellationToken);
            _logger.LogInformation("User signed in");
            httpContext.Response.Cookies.Append(Constants.UserSessionCookieKey, rpsUser.GetId().ToString());
            _logger.LogInformation("set cookie with value {id}", rpsUser.GetId());
            return rpsUser;
        }
        return null;
    }

    public async Task<bool> AddAsync(RpsUser rpsUser, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (_users.Any(u => u.Id.Equals(rpsUser.Id) 
                || (u.Name?.Equals(rpsUser.Name, StringComparison.CurrentCultureIgnoreCase) ?? true)))
            {
                _logger.LogWarning("User not stored because of conflict with id {Id}", rpsUser.Id);
                return false;
            }
            _users.Add(rpsUser);
            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<RpsUser?> GetAsync(Func<RpsUser, bool> query, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => _users.FirstOrDefault(query), cancellationToken);
    }

    public async Task<bool> UpdateAsync(RpsUser rpsUser, CancellationToken cancellationToken = default)
    {
        RpsUser? user = await GetAsync(u => u.Id.Equals(rpsUser.Id), cancellationToken);
        if(user is null)
        {
            _logger.LogWarning("Could not update user because User is null");
            return false;
        }
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            RpsUser? conflictingUser = await GetAsync(u => 
                u.Name?.Equals(rpsUser.Name, StringComparison.CurrentCultureIgnoreCase) ?? false, cancellationToken);
            if(conflictingUser is not null)
            {
                _logger.LogWarning("Could not update user because of conflicting user");
                return false;
            }
            user.Name = rpsUser.Name;
            user.EncryptedPassphrase = rpsUser.EncryptedPassphrase;
            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}