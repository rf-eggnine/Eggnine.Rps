//  ©️ 2024 by RF At EggNine All Rights Reserved
using System;

namespace Eggnine.Rps.Web.Exceptions;
public class UserAlreadyNamedException : Exception
{
    public UserAlreadyNamedException() : base("User already named")
    {
        
    }
}