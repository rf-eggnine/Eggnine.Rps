//  ©️ 2024 by RF At EggNine All Rights Reserved
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Authentication;
using Eggnine.Rps.Core;
using Eggnine.Rps.Web.Extensions;

namespace Eggnine.Rps.Web.Pages;
public class SignIn : PageModel
{
    private readonly ILogger _logger;
    private readonly IUserRepository _users;
    private readonly IRpsEngine _engine;

    public SignIn(ILogger<SignIn> logger, IUserRepository Users, IRpsEngine engine)
    {
        _logger = logger;
        _users = Users;
        _engine = engine;
    }
    
    public async Task<RpsUser?> GetUserFromCookieAsync(CancellationToken cancellationToken = default)
    {
        return await HttpContext.GetUserFromCookieAsync(_users, _logger, cancellationToken);
    }

    public IList<IValidation> Validations {get;set;} = new List<IValidation>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        await _users.SignInUserAsync(HttpContext, false, cancellationToken);
        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync(string name, string passphrase, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(passphrase);
        Validations.Clear();
        RpsUser? rpsUser = await _users.GetAsync(u => name.Equals(u.Name), cancellationToken);
        if(rpsUser is null)
        {
            Validations.Add(new UserNotFoundValidation());
            return Page();
        }
        if(!await rpsUser.VerifyEncryptionAsync(passphrase, cancellationToken))
        {
            Validations.Add(new UserNotFoundValidation());
            return Page();
        }
        await _users.SignInUserAsync(HttpContext, rpsUser.Id, cancellationToken);
        return RedirectToPage("home");
    }
}