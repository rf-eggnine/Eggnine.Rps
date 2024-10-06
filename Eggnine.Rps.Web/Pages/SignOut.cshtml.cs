//  ©️ 2024 by RF At EggNine All Rights Reserved
using Eggnine.Rps.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Collections.Generic;
using Eggnine.Rps.Web.Extensions;

namespace Eggnine.Rps.Web.Pages;
public class SignOut : PageModel
{
    private readonly ILogger _logger;
    private readonly IUserRepository _users;

    public SignOut(ILogger<SignUp> logger, IUserRepository Users)
    {
        _logger = logger;
        _users = Users;
    }

    public IList<IValidation> Validations {get;set;} = new List<IValidation>();
    
    public async Task<RpsUser?> GetUserFromCookieAsync(CancellationToken cancellationToken = default)
    {
        return await HttpContext.GetUserFromCookieAsync(_users, _logger, cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        _logger.LogTrace("Entering {MethodName}", nameof(OnGetAsync));
        await HttpContext.SignOutAsync();
        HttpContext.Response.Cookies.Delete(Constants.UserSessionCookieKey);
        return RedirectToPage("/Index");
    }
}