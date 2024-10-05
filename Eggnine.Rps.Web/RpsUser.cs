//  ©️ 2024 by RF At EggNine All Rights Reserved

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Eggnine.Rps.Common;
using Eggnine.Rps.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Eggnine.Rps.Web.Exceptions;
using Eggnine.Rps.Web.Extensions;

namespace Eggnine.Rps.Web;

[PrimaryKey(nameof(Id))]
public class RpsUser : ClaimsPrincipal, IRpsPlayer
{
    private readonly IRpsEngine _engine;
    public RpsUser(Guid id, bool hasAcceptedCookies, IRpsEngine engine) : 
        base (new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim("id", id.ToString())], CookieAuthenticationDefaults.AuthenticationScheme)))
    {
        _engine = engine;
        HasAcceptedCookies = hasAcceptedCookies;
    }
    public Guid Id => this.GetId();
    public string? Name {get;set;}
    public long Score => _engine.GetScoreAsync(this).GetAwaiter().GetResult();
    public bool HasAcceptedCookies {get;set;}
    public string? EncryptedPassphrase {get;set;}

    public async Task<bool> VerifyEncryptionAsync(string passphrase, CancellationToken cancellationToken) 
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passphrase);
        if(EncryptedPassphrase == null)
        {
            return false;
        }
        return await passphrase.VerifyEncryptionAsync(EncryptedPassphrase, cancellationToken);
    }
}