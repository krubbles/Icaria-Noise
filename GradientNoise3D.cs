using System.Runtime.CompilerServices;
namespace Icaria.Engine.Procedural
{
    public partial class IcariaNoise
    {
        /// <summary>High-quality version of GradientNoise() that returns a rotated
        /// slice of 3D gradient noise to remove grid alignment artifacts.</summary>
        [MethodImpl(512)] // aggressive optimization on supported runtimes
        public static float GradientNoiseHQ(float x, float y, int seed = 0)
        {
            // rotation from https://noiseposti.ng/posts/2022-01-16-The-Perlin-Problem-Breaking-The-Cycle.html
            float xz = x;
            float s2 = xz * -0.21132487f;
            float yy = y * 0.5773502692f;
            x += (s2 + yy);
            float z = (s2 + yy);
            y = xz * -0.57735027f + yy;

            // GradientNoise3D() won't get inlined automatically so its manually inlined here.
            // seems to improve preformance by around 5 to 10%
            int ix = x > 0 ? (int)x : (int)x - 1;
            int iy = y > 0 ? (int)y : (int)y - 1;
            int iz = z > 0 ? (int)z : (int)z - 1;
            float fx = x - ix;
            float fy = y - iy;
            float fz = z - iz;

            ix += seed * Const.SeedPrime;

            ix += Const.Offset;
            iy += Const.Offset;
            iz += Const.Offset;
            int p1 = ix * Const.XPrime1 + iy * Const.YPrime1 + iz * Const.ZPrime1;
            int p2 = ix * Const.XPrime2 + iy * Const.YPrime2 + iz * Const.ZPrime2;
            int llHash = p1 * p2;
            int lrHash = (p1 + Const.XPrime1) * (p2 + Const.XPrime2);
            int ulHash = (p1 + Const.YPrime1) * (p2 + Const.YPrime2);
            int urHash = (p1 + Const.XPlusYPrime1) * (p2 + Const.XPlusYPrime2);
            float zLowBlend = InterpolateGradients3D(llHash, lrHash, ulHash, urHash, fx, fy, fz);
            llHash = (p1 + Const.ZPrime1) * (p2 + Const.ZPrime2);
            lrHash = (p1 + Const.XPlusZPrime1) * (p2 + Const.XPlusZPrime2);
            ulHash = (p1 + Const.YPlusZPrime1) * (p2 + Const.YPlusZPrime2);
            urHash = (p1 + Const.XPlusYPlusZPrime1) * (p2 + Const.XPlusYPlusZPrime2);
            float zHighBlend = InterpolateGradients3D(llHash, lrHash, ulHash, urHash, fx, fy, fz - 1);
            float sz = fz * fz * (3 - 2 * fz);
            return zLowBlend + (zHighBlend - zLowBlend) * sz;
        }

        /// <summary> 3D -1 to 1 gradient noise function. Analagous to Perlin Noise. </summary>
        [MethodImpl(512)] // aggressive optimization on supported runtimes
        public static float GradientNoise3D(float x, float y, float z, int seed = 0)
        {
            // see comments in GradientNoise()
            int ix = x > 0 ? (int)x : (int)x - 1;
            int iy = y > 0 ? (int)y : (int)y - 1;
            int iz = z > 0 ? (int)z : (int)z - 1;
            float fx = x - ix;
            float fy = y - iy;
            float fz = z - iz;

            ix += seed * Const.SeedPrime;

            ix += Const.Offset;
            iy += Const.Offset;
            iz += Const.Offset;
            int p1 = ix * Const.XPrime1 + iy * Const.YPrime1 + iz * Const.ZPrime1;
            int p2 = ix * Const.XPrime2 + iy * Const.YPrime2 + iz * Const.ZPrime2;
            int llHash = p1 * p2;
            int lrHash = (p1 + Const.XPrime1) * (p2 + Const.XPrime2);
            int ulHash = (p1 + Const.YPrime1) * (p2 + Const.YPrime2);
            int urHash = (p1 + Const.XPlusYPrime1) * (p2 + Const.XPlusYPrime2);
            float zLowBlend = InterpolateGradients3D(llHash, lrHash, ulHash, urHash, fx, fy, fz);
            llHash = (p1 + Const.ZPrime1) * (p2 + Const.ZPrime2);
            lrHash = (p1 + Const.XPlusZPrime1) * (p2 + Const.XPlusZPrime2);
            ulHash = (p1 + Const.YPlusZPrime1) * (p2 + Const.YPlusZPrime2);
            urHash = (p1 + Const.XPlusYPlusZPrime1) * (p2 + Const.XPlusYPlusZPrime2);
            float zHighBlend = InterpolateGradients3D(llHash, lrHash, ulHash, urHash, fx, fy, fz - 1);
            float sz = fz * fz * (3 - 2 * fz);
            return zLowBlend + (zHighBlend - zLowBlend) * sz;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe float InterpolateGradients3D(int llHash, int lrHash, int ulHash, int urHash, float fx, float fy, float fz)
        {
            // see comments in InterpolateGradients2D()
            int xHash, yHash, zHash;
            xHash = (llHash & Const.GradAndMask) | Const.GradOrMask;
            yHash = xHash << Const.GradShift1;
            zHash = xHash << Const.GradShift2;
            float llGrad = fx * *(float*)&xHash + fy * *(float*)&yHash + fz * *(float*)&zHash; // dot-product
            xHash = (lrHash & Const.GradAndMask) | Const.GradOrMask;
            yHash = xHash << Const.GradShift1;
            zHash = xHash << Const.GradShift2;
            float lrGrad = (fx - 1) * *(float*)&xHash + fy * *(float*)&yHash + fz * *(float*)&zHash;
            xHash = (ulHash & Const.GradAndMask) | Const.GradOrMask;
            yHash = xHash << Const.GradShift1;
            zHash = xHash << Const.GradShift2;
            float ulGrad = fx * *(float*)&xHash + (fy - 1) * *(float*)&yHash + fz * *(float*)&zHash; // dot-product
            xHash = (urHash & Const.GradAndMask) | Const.GradOrMask;
            yHash = xHash << Const.GradShift1;
            zHash = xHash << Const.GradShift2;
            float urGrad = (fx - 1) * *(float*)&xHash + (fy - 1) * *(float*)&yHash + fz * *(float*)&zHash;
            float sx = fx * fx * (3 - 2 * fx);
            float sy = fy * fy * (3 - 2 * fy);
            float lowerBlend = llGrad + (lrGrad - llGrad) * sx;
            float upperBlend = ulGrad + (urGrad - ulGrad) * sx;
            return lowerBlend + (upperBlend - lowerBlend) * sy;
        }
    }
}