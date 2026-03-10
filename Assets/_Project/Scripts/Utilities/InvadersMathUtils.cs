using System;
using UnityEngine;

public static class InvadersMathUtils
{
    public static UInt64 Min(UInt64 a, UInt64 b)
    {
        return a <= b ? a : b;
    }
    
    public static UInt64 Max(UInt64 a, UInt64 b)
    {
        return a >= b ? a : b;
    }
    
    public static UInt64 Clamp(UInt64 value, UInt64 min, UInt64 max)
    {
        return Max(Min(value, max), min);
    }
}
