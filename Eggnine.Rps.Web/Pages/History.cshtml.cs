//  ©️ 2024 by RF At EggNine All Rights Reserved
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using Eggnine.Rps.Core;
using Eggnine.Rps.Web.Extensions;

namespace Eggnine.Rps.Web.Pages;
public class History : PageModel
{
    private readonly ILogger _logger;
    private readonly IUserRepository _users;
    private readonly IRpsEngine _engine;

    public History(ILogger<SignIn> logger, IUserRepository Users, IRpsEngine engine)
    {
        _logger = logger;
        _users = Users;
        _engine = engine;
    }
    
    public async Task<RpsUser?> GetUserFromCookieAsync(CancellationToken cancellationToken = default)
    {
        return await HttpContext.GetUserFromCookieAsync(_users, _logger, cancellationToken);
    }

    public long Turn => _engine.GetTurnAsync().GetAwaiter().GetResult();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        await _users.SignInUserAsync(HttpContext, false, cancellationToken);
        return Page();
    }

    public async Task<long> ActionsAsync(RpsAction action, long turn, CancellationToken cancellationToken = default)
        => await _engine.GetActionsOnTurnAsync(turn, action, cancellationToken);
    
    public async Task<RpsAction> ActionAsync(RpsUser? user, CancellationToken cancellationToken = default)
    {
        if(user is null)
        {
            return RpsAction.None;
        }
        return await _engine.GetActionOnTurnAsync(await _engine.GetTurnAsync(cancellationToken), user, cancellationToken);
    }
}