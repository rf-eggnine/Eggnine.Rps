//  ©️ 2024 by RF At EggNine All Rights Reserved
using Eggnine.Rps.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authentication;
using System.Threading;
using System.Collections.Generic;
using Eggnine.Rps.Core;
using Eggnine.Rps.Web.Extensions;

namespace Eggnine.Rps.Web.Pages;
public class SignUp : PageModel
{
    private readonly ILogger _logger;
    private readonly IUserRepository _users;
    private readonly IRpsEngine _engine;

    public SignUp(ILogger<SignUp> logger, IUserRepository Users, IRpsEngine engine)
    {
        _logger = logger;
        _users = Users;
        _engine = engine;
    }

    public IList<IValidation> Validations {get;set;} = new List<IValidation>();
    
    public async Task<RpsUser?> GetUserFromCookieAsync(CancellationToken cancellationToken = default)
    {
        return await HttpContext.GetUserFromCookieAsync(_users, _logger, cancellationToken);
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        await _users.SignInUserAsync(HttpContext, false, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string name, string passphrase, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(passphrase);
        Validations.Clear();
        string encrypted = await passphrase.EncryptAsync(cancellationToken);
        RpsUser? rpsUser = await HttpContext.GetUserFromCookieAsync(_users, _logger, cancellationToken);
        if(rpsUser is null)
        {
            _logger.LogInformation("No user currently connected, logging in new user");
            return Page();
        }
        if(!await _users.UpdateAsync(new RpsUser(rpsUser.Id, rpsUser.HasAcceptedCookies, _engine)
            {
                Name = name,
                EncryptedPassphrase = encrypted,
            }))
        {
            _logger.LogInformation("Update user failed");
            Validations.Add(new UserAlreadyExistsValidation());
            return Page();
        }
        return RedirectToPage("home");
    }
}