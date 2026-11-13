using System;

namespace Wolfgang.Extensions.Mail.ShadowWorkloads;

/// <summary>
/// A small deterministic generator (SplitMix64) for the synthetic mail traffic. It is used instead of
/// <see cref="Random"/> because the traffic must be identical for a given seed on every run and on every
/// runtime, so two library versions measure the same input. It is not for security use.
/// </summary>
internal sealed class SeededRandom
{

    private ulong _state;



    internal SeededRandom(int seed)
    {
        _state = (ulong)seed;
    }



    /// <summary>A non-negative integer.</summary>
    internal int Next()
    {
        return (int)(NextUInt64() >> 33);
    }



    /// <summary>An integer in [0, <paramref name="maxValue"/>).</summary>
    internal int Next(int maxValue)
    {
        return (int)(NextUInt64() % (ulong)maxValue);
    }



    /// <summary>An integer in [<paramref name="minValue"/>, <paramref name="maxValue"/>).</summary>
    internal int Next(int minValue, int maxValue)
    {
        return minValue + Next(maxValue - minValue);
    }



    internal void NextBytes(byte[] buffer)
    {
        for (var i = 0; i < buffer.Length; i++)
        {
            buffer[i] = (byte)(NextUInt64() >> 56);
        }
    }



    private ulong NextUInt64()
    {
        _state += 0x9E3779B97F4A7C15UL;
        var z = _state;
        z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
        z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
        return z ^ (z >> 31);
    }
}
