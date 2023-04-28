# Icaria-Noise
A highly-optimized C# noise-function library. Compile with Burst if using in Unity, as Mono is very slow. 
Include namespace Icaria.Engine.Procedural, call noise functions from static class IcariaNoise.

Preformance in Roslyn (tested on AMD Ryzen 9 6900HS)
- IcariaNoise.GradientNoise(x, y): 6ns
- IcariaNoise.GradientNoisePeriodic(x, y, period): 10ns
- IcariaNoise.WorleyNoise(x, y): 60ns
- IcariaNoise.WorleyNoisePeriodic(x, y): 70ns

Preformance in Burst (tested on AMD Ryzen 9 6900HS)
- IcariaNoise.GradientNoise(x, y): 5ns (3.5x faster then Mathf.PerlinNoise)
- IcariaNoise.GradientNoisePeriodic(x, y, period): 6ns
- IcariaNoise.WorleyNoise(x, y): 21ns
- IcariaNoise.WorleyNoisePeriodic(x, y, period): 21ns
