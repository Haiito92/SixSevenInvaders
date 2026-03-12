using UnityEngine;

public static class VfxDebug
{
    public static bool BlockRipple = false;
    public static bool BlockFullScreenPixel = false;
    public static bool BlockPlayerEffects = false;
    public static bool BlockEnemiesEffects = false;
    public static bool BlockScoreEffects = false;
    public static bool BlockSoundEffects = false;

    public static void ToggleRipple()
    {
        BlockRipple = !BlockRipple;
    }
    
    public static void ToggleFullScreenPixel()
    {
        BlockFullScreenPixel  = !BlockFullScreenPixel;
    }
    
    public static void TogglePlayerEffects()
    {
        BlockPlayerEffects  = !BlockPlayerEffects;
    }
    
    public static void ToggleEnemiesEffects()
    {
        BlockEnemiesEffects  = !BlockEnemiesEffects;
    }

    public static void ToggleScoreEffects()
    {
        BlockScoreEffects  = !BlockScoreEffects;
    }
    
    public static void ToggleSoundEffects()
    {
        BlockSoundEffects  = !BlockSoundEffects;
    }
    
    public static void ToggleAllEffects()
    {
        if (BlockRipple || BlockFullScreenPixel || BlockPlayerEffects || 
            BlockEnemiesEffects || BlockScoreEffects || BlockSoundEffects)
        {
            UnblockAllEffects();
            return;
        }
        
        BlockAllEffects();
    }
    
    public static void BlockAllEffects()
    {
        BlockRipple = true;
        BlockFullScreenPixel = true;
        BlockPlayerEffects = true;
        BlockEnemiesEffects = true;
        BlockScoreEffects = true;
        BlockSoundEffects = true;
    }
    
    public static void UnblockAllEffects()
    {
        BlockRipple = false;
        BlockFullScreenPixel = false;
        BlockPlayerEffects = false;
        BlockEnemiesEffects = false;
        BlockScoreEffects = false;
        BlockSoundEffects = false;
    }
}
