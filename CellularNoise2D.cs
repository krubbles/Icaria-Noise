using System.Runtime.CompilerServices;
using System;
namespace Icaria.Engine.Procedural
{
    public static partial class IcariaNoise
    {
        static readonly NoisePeriod _noPeriod = new NoisePeriod(10, 10);

        /// <summary>A noise function of random polygonal cells resembling a voronoi diagram. </summary>
        public static unsafe CellularResults CellularNoise(float x, float y, int seed = 0)
        {
            int ix = x > 0 ? (int)x : (int)x - 1;
            int iy = y > 0 ? (int)y : (int)y - 1;
            float fx = x - ix;
            float fy = y - iy;

            ix += seed * Const.SeedPrime;

            int cx = ix * Const.XPrime1;
            int rx = (cx + Const.XPrime1) >> Const.PeriodShift;
            int lx = (cx - Const.XPrime1) >> Const.PeriodShift;
            cx >>= Const.PeriodShift;
            int cy = iy * Const.YPrime1;
            int uy = (cy + Const.YPrime1) >> Const.PeriodShift;
            int ly = (cy - Const.YPrime1) >> Const.PeriodShift;
            cy >>= Const.PeriodShift;

            return SearchNeighborhood(fx, fy,
                Hash(lx, ly), Hash(cx, ly), Hash(rx, ly),
                Hash(lx, cy), Hash(cx, cy), Hash(rx, cy),
                Hash(lx, uy), Hash(cx, uy), Hash(rx, uy));
        }

        /// <summary>A periodic noise function of random polygonal cells resembling a voronoi diagram. </summary>
        public static unsafe CellularResults CellularNoisePeriodic(float x, float y, in NoisePeriod period, int seed = 0)
        {
            // See comments in GradientNoisePeriodic(). differences are documented.
            int ix = x > 0 ? (int)x : (int)x - 1;
            int iy = y > 0 ? (int)y : (int)y - 1;
            float fx = x - ix;
            float fy = y - iy;

            ix += seed * Const.SeedPrime;

            // r: right c: center l: left/lower u: upper
            // worley uses 3x3 as supposed to gradient using 2x2
            int cx = ix * period.xf;
            int rx = (cx + period.xf) >> Const.PeriodShift;
            int lx = (cx - period.xf) >> Const.PeriodShift;
            cx >>= Const.PeriodShift;

            int cy = iy * period.yf;
            int uy = (cy + period.yf) >> Const.PeriodShift;
            int ly = (cy - period.yf) >> Const.PeriodShift;
            cy >>= Const.PeriodShift;

            return SearchNeighborhood(fx, fy,
                Hash(lx, ly), Hash(cx, ly), Hash(rx, ly),
                Hash(lx, cy), Hash(cx, cy), Hash(rx, cy),
                Hash(lx, uy), Hash(cx, uy), Hash(rx, uy));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe CellularResults SearchNeighborhood(float fx, float fy, int llh, int lch, int lrh, int clh, int cch, int crh, int ulh, int uch, int urh)
        {
            // intermediate variables
            int xHash, yHash;
            float dx, dy, sqDist, temp;
            // output variables
            int r = 0;
            float d0 = 1, d1 = 1;

            fy += 2f;

            // bottom row
            // the WorleyAndMask and WorleyOrMask set the sign bit to zero so the number is poitive and 
            // set the exponent to 1 so that the value is between 1 and 2. This is why the offsets to fx / fy
            // range from 0 to 2 instead of -1 to 1. 
            xHash = (llh & Const.WorleyAndMask) | Const.WorleyOrMask;
            yHash = xHash << 13;
            dx = fx - *(float*)&xHash + 2f;
            dy = fy - *(float*)&yHash;
            sqDist = dx * dx + dy * dy;
            r = sqDist < d0 ? llh : r;
            d1 = sqDist < d1 ? sqDist : d1; // min
            temp = d0 > d1 ? d0 : d1; // max
            d0 = d0 < d1 ? d0 : d1;
            d1 = temp;

            xHash = (lch & Const.WorleyAndMask) | Const.WorleyOrMask;
            yHash = xHash << 13;
            dx = fx - *(float*)&xHash + 1f;
            dy = fy - *(float*)&yHash;
            sqDist = dx * dx + dy * dy;
            r = sqDist < d0 ? lch : r;
            d1 = sqDist < d1 ? sqDist : d1; // min
            temp = d0 > d1 ? d0 : d1; // max
            d0 = d0 < d1 ? d0 : d1; // min
            d1 = temp;

            xHash = (lrh & Const.WorleyAndMask) | Const.WorleyOrMask;
            yHash = xHash << 13;
            dx = fx - *(float*)&xHash + 0f;
            dy = fy - *(float*)&yHash;
            sqDist = dx * dx + dy * dy;
            r = sqDist < d0 ? lrh : r;
            d1 = sqDist < d1 ? sqDist : d1; // min
            temp = d0 > d1 ? d0 : d1; // max
            d0 = d0 < d1 ? d0 : d1;
            d1 = temp;

            fy -= 1f;

            // middle row
            xHash = (clh & Const.WorleyAndMask) | Const.WorleyOrMask;
            yHash = xHash << 13;
            dx = fx - *(float*)&xHash + 2f;
            dy = fy - *(float*)&yHash;
            sqDist = dx * dx + dy * dy;
            r = sqDist < d0 ? clh : r;
            d1 = sqDist < d1 ? sqDist : d1; // min
            temp = d0 > d1 ? d0 : d1; // max
            d0 = d0 < d1 ? d0 : d1;
            d1 = temp;

            xHash = (cch & Const.WorleyAndMask) | Const.WorleyOrMask;
            yHash = xHash << 13;
            dx = fx - *(float*)&xHash + 1f;
            dy = fy - *(float*)&yHash;
            sqDist = dx * dx + dy * dy;
            r = sqDist < d0 ? cch : r;
            d1 = sqDist < d1 ? sqDist : d1; // min
            temp = d0 > d1 ? d0 : d1; // max
            d0 = d0 < d1 ? d0 : d1; // min
            d1 = temp;

            xHash = (crh & Const.WorleyAndMask) | Const.WorleyOrMask;
            yHash = xHash << 13;
            dx = fx - *(float*)&xHash + 0f;
            dy = fy - *(float*)&yHash;
            sqDist = dx * dx + dy * dy;
            r = sqDist < d0 ? crh : r;
            d1 = sqDist < d1 ? sqDist : d1; // min
            temp = d0 > d1 ? d0 : d1; // max
            d0 = d0 < d1 ? d0 : d1;
            d1 = temp;

            fy -= 1f;

            // top row
            xHash = (ulh & Const.WorleyAndMask) | Const.WorleyOrMask;
            yHash = xHash << 13;
            dx = fx - *(float*)&xHash + 2f;
            dy = fy - *(float*)&yHash;
            sqDist = dx * dx + dy * dy;
            r = sqDist < d0 ? ulh : r;
            d1 = sqDist < d1 ? sqDist : d1; // min
            temp = d0 > d1 ? d0 : d1; // max
            d0 = d0 < d1 ? d0 : d1;
            d1 = temp;

            xHash = (uch & Const.WorleyAndMask) | Const.WorleyOrMask;
            yHash = xHash << 13;
            dx = fx - *(float*)&xHash + 1f;
            dy = fy - *(float*)&yHash;
            sqDist = dx * dx + dy * dy;
            r = sqDist < d0 ? uch : r;
            d1 = sqDist < d1 ? sqDist : d1; // min
            temp = d0 > d1 ? d0 : d1; // max
            d0 = d0 < d1 ? d0 : d1; // min
            d1 = temp;

            xHash = (urh & Const.WorleyAndMask) | Const.WorleyOrMask;
            yHash = xHash << 13;
            dx = fx - *(float*)&xHash + 0f;
            dy = fy - *(float*)&yHash;
            sqDist = dx * dx + dy * dy;
            r = sqDist < d0 ? urh : r;
            d1 = sqDist < d1 ? sqDist : d1; // min
            temp = d0 > d1 ? d0 : d1; // max
            d0 = d0 < d1 ? d0 : d1;

            d1 = temp;
#if UNITY_2017_1_OR_NEWER
            d0 = UnityEngine.Mathf.Sqrt(d0);
            d1 = UnityEngine.Mathf.Sqrt(d1);
#else
            d0 = MathF.Sqrt(d0);
            d1 = MathF.Sqrt(d1);
#endif
            r = ((r * Const.ZPrime1) & Const.PortionAndMask) | Const.PortionOrMask;
            float rFloat = *(float*)&r - 1f;
            return new CellularResults(d0, d1, rFloat);
        }
    }
    /// <summary>The results of a Cellular Noise evaluation. </summary>
    public readonly struct CellularResults
    {
        /// <summary> The distance to the closest cell center.</summary>
        public readonly float d0;
        /// <summary> The distance to the second-closest cell center.</summary>
        public readonly float d1;
        /// <summary> A random 0 - 1 value for each cell. </summary>
        public readonly float r;

        public CellularResults(float d0, float d1, float r)
        {
            this.d0 = d0;
            this.d1 = d1;
            this.r = r;
        }
    }
}
