using UnityEngine;
namespace Hexagons.Demo
{
    public sealed class HexTile : MonoBehaviour
    {
        private HexMap _map;
        private Hex _hex;

        [SerializeField]
        private float _dt;
        [SerializeField]
        private Color _colorA = Color.white;
        [SerializeField]
        private Color _colorB = Color.white;

        private Material _material;

        private void Awake()
        {
            _map = GetComponentInParent<HexMap>();
            _map.OnMouseTileChange += Refresh;
            _material = GetComponent<MeshRenderer>().material;
            enabled = false;
        }
        public void Set(Hex hex) => _hex = hex;
        private void Refresh(Hex hex)
        {
            _colorA = _colorB;
            _dt = 0;
            if (hex == _hex)
                _colorB = Color.green;
            else if (IsNeighbour(hex, out int index))
                if (index < 6)
                    _colorB = Color.cyan;
                else
                    _colorB = Color.yellow;
            else
                _colorB = Color.white;

            _material.SetColor("_ColorA", _colorA);
            _material.SetColor("_ColorB", _colorB);
            enabled = true;
        }

        private bool IsNeighbour(in Hex a, out int index)
        {
            for (index = 0; index < 12; index++)
            {
                Hex neighbour = HexTool.GetNeighbour(_hex, index);
                if (neighbour == a)
                    return true;
            }
            return false;
        }

        private void Update()
        {
            _dt += Time.deltaTime;
            float t = _dt / _map.FadeTime;
            _material.SetFloat("_TValue", t);
            enabled = t < 1;
        }
        private void OnDestroy() => _map.OnMouseTileChange -= Refresh;
    }
}