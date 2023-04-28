using System.Runtime.CompilerServices;
using System;
namespace Icaria.Engine.Procedural
{
    public static partial class IcariaNoise
    {
        static readonly NoisePeriod _noPeriod = new NoisePeriod(10, 10);
        public static unsafe (float d0, float d1, float r) WorleyNoise(float x, float y, int seed = 0)
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
        public static unsafe (float d0, float d1, float r) WorleyNoisePeriodic(float x, float y, in NoisePeriod period, int seed = 0)
        {
            // See comments in GradientNoisePeriodic(). differences are documented.
            int ix = x > 0 ? (int)x : (int)x - 1;
            int iy = y > 0 ? (int)y : (int)y - 1;
            float fx = x - ix;
            float fy = y - iy;

            ix += seed * Const.SeedPrime;

            // letters stand for left/right/center/upper/lower
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
        static unsafe (float d0, float d1, float r) SearchNeighborhood(float fx, float fy, int llh, int lch, int lrh, int clh, int cch, int crh, int ulh, int uch, int urh)
        {
            // intermediate variables
            int xHash, yHash;
            float dx, dy, sqDist, temp;
            // output variables
            int r = 0;
            float d0 = 1, d1 = 1;

            fy += 2f;

            // bottom row
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
            d0 = MathF.Sqrt(d0);
            d1 = MathF.Sqrt(d1);
            r = ((r * Const.ZPrime1) & Const.PortionAndMask) | Const.PortionOrMask;
            float rFloat = *(float*)&r - 1f;
            return (d0, d1, r);
        }
    }
}