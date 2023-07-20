# Icaria-Noise
A highly-optimized C# noise-function library. Compile with Burst if using in Unity, as Mono is very slow. 
Include namespace Icaria.Engine.Procedural, call noise functions from static class IcariaNoise. 
Commented to show optimizations over standard implementations.

Preformance w/ RyuJIT (tested on AMD Ryzen 9 6900HS, average per sample over 10 billion samples)
- IcariaNoise.GradientNoise(x, y): 6.3ns
- IcariaNoise.GradientNoisePeriodic(x, y, period): 9.5ns
- IcariaNoise.GradientNoiseVec2(x, y): 11.5ns
- IcariaNoise.GradientNoiseHQ(x, y): 14.9ns
- IcariaNoise.GradientNoise3D(x, y): 13.4ns
