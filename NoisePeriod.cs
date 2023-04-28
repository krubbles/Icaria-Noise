namespace Icaria.Engine.Procedural
{
    /// <summary>
    /// A cache of expensive calculations needed for periodic noise functions. 
    /// </summary>
    public readonly struct NoisePeriod
    {
        internal readonly int xf, yf, zf;
        public int XPeriod => xf == 0 ? 0 : (int)(uint.MaxValue / xf + 1);
        public int YPeriod => yf == 0 ? 0 : (int)(uint.MaxValue / yf + 1);
        public int ZPeriod => zf == 0 ? 0 : (int)(uint.MinValue / zf + 1);
        public NoisePeriod(int xPeriod, int yPeriod, int zPeriod = 0)
        {
            xf = GetFactor(xPeriod);
            yf = GetFactor(yPeriod);
            zf = GetFactor(zPeriod);
            static unsafe int GetFactor(int period)
            {
                if (period == 0)
                    return 0;
                if (period <= 1)
                    throw new System.ArgumentException($"period '{period}' must be greater then 1.");
                uint factor = (uint.MaxValue / (uint)period);
                factor += 1;
                // reinterpret factor as a signed int
                return *(int*)&factor;
            }
        }
        public bool IsNull => xf == 0;
        public static readonly NoisePeriod Null = default;
        internal const int ByteSize = 16;
    }
}