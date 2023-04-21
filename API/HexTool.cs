using System;
using System.Collections.Generic;

namespace Hexagons
{
    public readonly struct HexTool
    {
        private readonly float _tileSize;
        private readonly HexagonOrientation _layout;
        public float Size => _tileSize;
        public HexagonOrientation Layout => _layout;

        public HexTool(in float tileSize, in HexagonOrientation layout = HexagonOrientation.POINTY)
        {
            if (tileSize <= 0)
                throw new Exception("Size must be greater than zero. Current size is " + tileSize);
            _layout = layout;
            _tileSize = tileSize;
        }
        public void CreateLayout(in MapShape shape, int size, ref HashSet<Hex> result)
        {
            result.Clear();
            switch (shape)
            {
                case MapShape.Hexagon:
                    for (int q = -size; q <= size; q++)
                    {
                        int r1 = Math.Max(-size, -q - size);
                        int r2 = Math.Min(size, -q + size);
                        for (int r = r1; r <= r2; r++)
                            if (_layout == HexagonOrientation.POINTY)
                                result.Add(new(q, r));
                            else
                                result.Add(new(r, q));
                    }
                    break;
                case MapShape.Rhombus:
                    for (int r = -size / 2; r < size / 2; r++)
                        for (int q = -size / 2; q < size / 2; q++)
                            if (_layout == HexagonOrientation.POINTY)
                                result.Add(new(q, r));
                            else
                                result.Add(new(r, q));
                    break;
                case MapShape.Diamond:
                    for (int r = -size / 2; r < size / 2; r++)
                        for (int q = -size / 2; q < size / 2; q++)
                            if (_layout == HexagonOrientation.POINTY)
                                result.Add(new(q, -q - r));
                            else
                                result.Add(new(-q - r, q));
                    break;
                case MapShape.Triangle:
                    for (int q = 0; q < size; q++)
                        for (int r = 0; r < size - q; r++)
                            if (_layout == HexagonOrientation.POINTY)
                                result.Add(new(q, r));
                            else
                                result.Add(new(r, q));
                    break;
                case MapShape.Rectangle:
                    {
                        for (int r = -size / 2; r < size / 2; r++)
                        {
                            int offset = r >> 1;
                            for (int q = (-size / 2) - offset; q < (size / 2) - offset; q++)
                                if (_layout == HexagonOrientation.POINTY)
                                    result.Add(new(q, r));
                                else
                                    result.Add(new(r, q));
                        }
                    }
                    break;
            }
        }
        public Hex WorldToHex(in float x, in float y)
        {
            float col, row;
            switch (_layout)
            {
                case HexagonOrientation.POINTY:
                    col = (float)((Math.Sqrt(3) / 3f * x - 1 / 3f * y) / _tileSize);
                    row = (2 / 3f * y) / _tileSize;
                    break;
                case HexagonOrientation.FLAT:
                    col = (2 / 3f * x) / _tileSize;
                    row = (float)((-1 / 3f * x + Math.Sqrt(3) / 3f * y) / _tileSize);
                    break;
                default:
                    throw new Exception("Incorrect layout set. Currently set to " + _layout);
            }

            Round(col, row, out int q, out int r);
            return new(q, r);
        }
        public void ToWorld(in Hex hex, out float x, out float y)
        {
            switch (_layout)
            {
                case HexagonOrientation.POINTY:
                    x = _tileSize * (float)(Math.Sqrt(3) * hex.X + Math.Sqrt(3) / 2 * hex.Y);
                    y = _tileSize * 3 / 2f * hex.Y;
                    break;
                case HexagonOrientation.FLAT:
                    x = _tileSize * (3 / 2f * hex.X);
                    y = _tileSize * (float)(Math.Sqrt(3) / 2 * hex.X + Math.Sqrt(3) * hex.Y);
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
        public static Hex GetNeighbour(in Hex hex, in int neighbourIndex)
        {

            switch (neighbourIndex)
            {
                case 0: // north
                    return new(hex.X, hex.Y + 1);
                case 1: // north east
                    return new(hex.X + 1, hex.Y);
                case 2: // south east
                    return new(hex.X + 1, hex.Y - 1);
                case 3: // south
                    return new(hex.X, hex.Y - 1);
                case 4: // south west
                    return new(hex.X - 1, hex.Y);
                case 5: // north west 
                    return new(hex.X - 1, hex.Y + 1);
                case 6: //north north east
                    return new(hex.X + 1, hex.Y + 1);
                case 7: //east
                    return new(hex.X + 2, hex.Y - 1);
                case 8: //south south east
                    return new(hex.X + 1, hex.Y - 2);
                case 9: //south south west
                    return new(hex.X - 1, hex.Y - 1);
                case 10: //west
                    return new(hex.X - 2, hex.Y + 1);
                case 11: //north north west
                    return new(hex.X - 1, hex.Y + 2);
                default:
                    return hex;
            }
        }
    }
}

