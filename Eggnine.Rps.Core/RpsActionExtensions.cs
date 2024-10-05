//  ©️ 2024 by RF At EggNine All Rights Reserved

namespace Eggnine.Rps.Core;
public static class RpsActionExtensions
{
    public static bool Validate(this RpsAction action, bool allowRandom = true)
    {
        return action switch
        {
            RpsAction.Rock or RpsAction.Paper or RpsAction.Scissors => true,
            RpsAction.Random => allowRandom,
            _ => false,
        };
    }
}