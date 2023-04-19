using System;
using System.Collections.Generic;

namespace Hexagons
{
    public readonly struct Hexagons
    {
        private static readonly Hex[] Neighbours_Axial =
        {
            new(1,0), new(1,-1), new(0,-1),
            new(-1,0), new(-1,1), new(0,1)
        };
        private static readonly Hex[] Neighbours_Row =
        {
            new(1,0), new(1,-1), new(0,-1),
            new(-1,0), new(0,1),new(1,1)
        };
        private static readonly Hex[] Neighbours_Colums =
        {
            new(1,1), new(1,0), new(0,-1),
            new(-1,0), new(-1,1), new(0,1)
        };

        private readonly float _size;
        private readonly HexagonLayout _layout;
        public float Size => _size;
        public HexagonLayout Layout => _layout;

        public Hexagons(in float size, in HexagonLayout layout = HexagonLayout.POINTY)
        {
            if (size <= 0)
                throw new Exception("Size must be greater than zero. Current size is " + size);
            _layout = layout;
            _size = size;
        }

        public Hex ToHex(in float x, in float y)
        {
            void ToRow(in int col, in int row, out int q, out int r)
            {
                q = col + (row - (row & 1)) / 2;
                r = row;
            }
            void ToColumn(in int col, in int row, out int q, out int r)
            {
                q = col;
                r = row + (col + (col & 1)) / 2;
            }

            float col, row;
            switch (_layout)
            {
                case HexagonLayout.POINTY:
                case HexagonLayout.ROWS:
                    col = (float)(Math.Sqrt(3 / 3f * x - 1 / 3f * y) / _size);
                    row = (2 / 3f * y) / _size;
                    break;
                case HexagonLayout.FLAT:
                case HexagonLayout.COLUMNS:
                    col = (2 / 3f * x) / _size;
                    row = (float)((-1 / 3f * x + Math.Sqrt(3) / 3 * y) / _size);
                    break;
                default:
                    throw new Exception("Incorrect layout set. Currently set to " + _layout);
            }

            Round(col, row, out int q, out int r);

            if (_layout == HexagonLayout.ROWS)
                ToRow(q, r, out q, out r);
            else if (_layout == HexagonLayout.COLUMNS)
                ToColumn(q, r, out q, out r);
            return new(q, r);
        }
        public void ToWorld(in Hex hex, out float x, out float y)
        {
            switch (_layout)
            {
                case HexagonLayout.POINTY:
                    x = _size * (float)(Math.Sqrt(3) * hex.X + Math.Sqrt(3) / 2 * hex.Y);
                    y = _size * 3 / 2f * hex.Y;
                    break;
                case HexagonLayout.FLAT:
                    x = _size * (3 / 2f * hex.X);
                    y = _size * (float)(Math.Sqrt(3) / 2 * hex.X + Math.Sqrt(3) * hex.Y);
                    break;
                case HexagonLayout.ROWS:
                    x = _size * (float)(Math.Sqrt(3) * (hex.X + 0.5 * (hex.Y & 1)));
                    y = _size * 3 / 2f * hex.Y;
                    break;
                case HexagonLayout.COLUMNS:
                    x = _size * 3 / 2f * hex.X;
                    y = _size * (float)(Math.Sqrt(3) * (hex.Y + 0.5 * (hex.X & 1)));
                    break;
                default:
                    throw new Exception("Incorrect layout set. Currently set to " + _layout);
            }
        }
        public float DistanceSqr(in Hex a, in Hex b)
        {
            ToWorld(a, out var x1, out var y1);
            ToWorld(b, out var x2, out var y2);
            var dx = x2 - x1;
            var dy = y2 - y1;
            return dx * dx + dy * dy;
        }
        public float Distance(in Hex a, in Hex b) => (float)Math.Sqrt(DistanceSqr(a, b));

        private void ToRow(in Hex a, out Hex row)
        {
            if (_layout != HexagonLayout.POINTY)
                throw new Exception("Can only convert pointy axial hexagons to row offset hexagons!");

            var q = a.X + (a.Y - (a.Y & 1)) / 2;
            var r = a.Y;
            row = new(q, r);
        }
        private void ToColumn(in Hex a, out Hex col)
        {
            if (_layout != HexagonLayout.FLAT)
                throw new Exception("Can only convert flat axial hexagons to column offset hexagons!");

            var q = a.X;
            var r = a.Y + (a.X - (a.X & 1)) / 2;
            col = new(q, r);
        }
        private void ToAxial(in Hex a, out Hex axial)
        {
            if (_layout == HexagonLayout.ROWS)
            {
                var q = a.X - (a.Y - (a.Y & 1)) / 2;
                var r = a.Y;
                axial = new(q, r);
            }
            else if (_layout == HexagonLayout.COLUMNS)
            {
                var q = a.X;
                var r = a.Y - (a.X - (a.X & 1)) / 2;
                axial = new(q, r);
            }
            else
                throw new Exception("Can only convert row or column hexagons to an axial hexagon!");
        }
        private static void Round(in float col, in float row, out int q, out int r)
        {
            q = (int)Math.Round(col);
            r = (int)Math.Round(row);
            var s = -q - r;

            var qdif = Math.Abs(q - col);
            var rdif = Math.Abs(r - row);
            var sDif = Math.Abs(s + col - row);

            if (qdif > rdif && qdif > sDif)
                q = -r - s;
            else if (rdif > sDif)
                r = -q - s;
        }

        public IReadOnlyCollection<Hex> Neighbours()
        {
            switch (_layout)
            {
                case HexagonLayout.COLUMNS:
                    return Neighbours_Colums;
                case HexagonLayout.ROWS:
                    return Neighbours_Row;
                case HexagonLayout.FLAT:
                case HexagonLayout.POINTY:
                    return Neighbours_Axial;

                default:
                    throw new Exception("Incorrect layout set. Currently set to " + _layout);
            }
        }
    }
}
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
    public static Hex operator +(in Hex a, in Hex b) => new(a._q + b._q, a._r + b._r);
    public static Hex operator -(in Hex a, in Hex b) => new(a._q - b._q, a._r - b._r);
    public static Hex operator /(in Hex a, in Hex b) => new(a._q / b._q, a._r / b._r);
    public static Hex operator *(in Hex a, in Hex b) => new(a._q * b._q, a._r * b._r);
}