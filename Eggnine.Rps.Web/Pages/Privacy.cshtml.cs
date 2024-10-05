//  ©️ 2024 by RF At EggNine All Rights Reserved
using System;
using System.Threading;
using System.Threading.Tasks;
using Eggnine.Rps.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Eggnine.Rps.Web.Pages;
public class Privacy : PageModel
{
    private readonly ILogger _logger;
    private readonly IUserRepository _users;

    public Privacy(ILogger<Index> logger, IUserRepository Users)
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
        _logger.LogTrace("Entering {ClassName}.{MethodName}", nameof(Privacy), nameof(OnGetAsync));
        RpsUser = await GetUserFromCookieAsync(cancellationToken);
        return Page();
    }
}