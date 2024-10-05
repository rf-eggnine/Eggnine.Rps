//  ©️ 2024 by RF At EggNine All Rights Reserved
using System;

namespace Eggnine.Rps.Core
{
    /// <summary>
    /// An Rps Player
    /// </summary>
    public interface IRpsPlayer
    {
        /// <summary>
        /// The players Id
        /// </summary>
        /// <returns></returns>
        Guid Id {get;}
    }
}