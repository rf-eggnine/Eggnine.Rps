//  ©️ 2024 by RF At EggNine All Rights Reserved
using System;
using System.Threading;
using System.Threading.Tasks;
using Eggnine.Rps.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Eggnine.Rps.Web.Pages;
public class Index : PageModel
{
    private readonly ILogger _logger;
    private readonly IUserRepository _users;

    public Index(ILogger<Index> logger, IUserRepository Users)
    {
        _logger = logger;
        _users = Users;
    }

    public RpsUser? RpsUser {get;set;}

    public async Task<RpsUser?> GetUserFromCookieAsync(CancellationToken cancellationToken = default)
    {
        return await HttpContext.GetUserFromCookieAsync(_users, _logger, cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(bool acceptedCookies = false, CancellationToken cancellationToken = default)
    {
        RpsUser = await GetUserFromCookieAsync(cancellationToken);
        _logger.LogDebug("Path {Path}", HttpContext.Request.Path);
        if(RpsUser is not null && RpsUser.HasAcceptedCookies && acceptedCookies)
        {
            return RedirectPreserveMethod($"Home?{Constants.QueryStringKeyAcceptedCookies}=true");
        }
        return Page();
    }
}