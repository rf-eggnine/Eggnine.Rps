//  ©️ 2024 by RF At EggNine All Rights Reserved
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eggnine.Rps.Core;
using Eggnine.Rps.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Extensions;

namespace Eggnine.Rps.Web.Pages;
public class Home : PageModel
{
    private readonly Random _random;
    private readonly string[] _actions = ["rock","paper","scissors","random"];
    private readonly ILogger _logger;
    private readonly IUserRepository _users;
    private readonly IRpsEngine _engine;

    public Home(ILogger<Index> logger, IUserRepository Users, IRpsEngine engine)
    {
        _logger = logger;
        _users = Users;
        _engine = engine;
        _random = new Random();
    }

    public RpsUser? RpsUser {get;set;}

    public async Task<RpsUser?> GetUserFromCookieAsync(CancellationToken cancellationToken = default)
    {
        return await HttpContext.GetUserFromCookieAsync(_users, _logger, cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(bool acceptedCookies = false, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Entering {MethodName}", nameof(OnGetAsync));
        RpsUser = await _users.SignInUserAsync(HttpContext, acceptedCookies, cancellationToken);
        if(acceptedCookies)
        {
            HttpContext.Response.Headers.Append(Constants.HeaderAcceptedCookies, "true");
            return RedirectToPage("home");
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string action, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("Entering {MethodName}", nameof(OnPostAsync));
        if(RpsUser is null)
        {
            RpsUser = await GetUserFromCookieAsync(cancellationToken);
        }
        if(RpsUser is RpsUser rpsUser && Enum.TryParse(action, out RpsAction rpsAction) && rpsAction.Validate())
        {
            _logger.LogDebug("User took action {Action}", rpsAction);
            if(rpsAction.Equals(RpsAction.Random))
            {
                rpsAction = (RpsAction)(_random.Next() % ((int)RpsAction.Random) + 1);
                _logger.LogDebug("Random action was {Action}", rpsAction);
            }
            await _engine.ActAsync(rpsUser, rpsAction, cancellationToken);
            return await OnGetAsync(false, cancellationToken);
        }
        if(RpsUser is null)
        {
            _logger.LogWarning("Invalid user");
        }
        else
        {
            _logger.LogWarning("Invalid action {Action}", action);
        }
        return await OnGetAsync(false, cancellationToken);
    }
}