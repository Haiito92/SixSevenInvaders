using UnityEngine;

public static class VfxDebug
{
    public static bool BlockRipple = false;
    public static bool BlockFullScreenPixel = false;
    public static bool BlockPlayerEffects = false;
    public static bool BlockEnemiesEffects = false;

    public static void BlockAllEffects()
    {
        BlockRipple = true;
        BlockFullScreenPixel = true;
        BlockPlayerEffects = true;
        BlockEnemiesEffects = true;
    }
    
    public static void UnblockAllEffects()
    {
        BlockRipple = false;
        BlockFullScreenPixel = false;
        BlockPlayerEffects = false;
        BlockEnemiesEffects = false;
    }
}
