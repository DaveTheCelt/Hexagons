using System;
namespace Hexagons
{
    public readonly struct Hex : IEquatable<Hex>
    {
        private readonly int _q;
        private readonly int _r;
        public int X => _q;
        public int Y => _r;

        public Hex(int q, int r)
        {
            _q = q;
            _r = r;
        }
        public override string ToString() => "[" + _q + "," + _r + "]";
        public override int GetHashCode() => _q + 3 * _r;
        public override bool Equals(object obj) => obj is Hex hex && Equals(hex);
        public bool Equals(Hex other) => other._q == _q && other._r == _r;

        public static bool operator ==(in Hex a, in Hex b) => a.Equals(b);
        public static bool operator !=(in Hex a, in Hex b) => !(a == b);
        public static Hex operator +(in Hex a, in Hex b) => new(a._q + b._q, a._r + b._r);
        public static Hex operator -(in Hex a, in Hex b) => new(a._q - b._q, a._r - b._r);
        public static Hex operator /(in Hex a, in Hex b) => new(a._q / b._q, a._r / b._r);
        public static Hex operator *(in Hex a, in Hex b) => new(a._q * b._q, a._r * b._r);
    }
}