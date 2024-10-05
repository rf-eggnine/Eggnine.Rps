//  ©️ 2024 by RF At EggNine All Rights Reserved
using System;
using System.Linq;
using System.Security.Claims;

namespace Eggnine.Rps.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetId(this ClaimsPrincipal user)
    {
        string? id = user.Claims.FirstOrDefault(c => c.Type.Equals("id"))?.Value;
        return id == null ? Guid.Empty : new Guid(id);
    }
}