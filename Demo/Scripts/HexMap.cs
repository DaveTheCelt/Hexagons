using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hexagons.Demo
{
    public class HexMap : MonoBehaviour
    {
        [SerializeField]
        private Transform _tileContainer;

        [SerializeField]
        private GameObject _preFab;

        [SerializeField, Range(1, 2)]
        float _gridSize;

        [SerializeField]
        HexagonOrientation _orientation;

        [SerializeField, Range(1, 10)]
        private int _size;

        [SerializeField, Range(.25f, 2)]
        private float _fadeTime = .5f;

        [SerializeField]
        private MapShape _shape;

        [SerializeField, Header("Refresh settings")]
        private bool _refresh;

        HexTool _hexTool;
        HashSet<Hex> _map = new();
        private Hex _mouseHex;

        public float FadeTime => _fadeTime;
        public Action<Hex> OnMouseTileChange { get; set; }

        private void Awake() => Refresh();
        private void Update()
        {
            SetMouseTile();
            if (_refresh)
            {
                Refresh();
                _refresh = !_refresh;
            }
        }
        private void Refresh()
        {
            ClearTiles();
            _hexTool = new(_gridSize, _orientation);
            _hexTool.CreateLayout(_shape, _size, ref _map);
            CreateTiles();
        }

        private void CreateTiles()
        {
            foreach (var hex in _map)
            {
                var obj = Instantiate(_preFab, _tileContainer);
                obj.transform.eulerAngles = new(0, _hexTool.Layout == HexagonOrientation.POINTY ? 0 : 90, 0);
                obj.transform.localScale = new Vector3(_gridSize, 1, _gridSize) * 0.95f;
                _hexTool.ToWorld(hex, out float x, out float z);
                obj.transform.position = new(x, 0, z);
                var tile = obj.GetComponent<HexTile>();
                tile.Set(hex);
            }
        }

        private void ClearTiles()
        {
            if (_tileContainer != null)
            {
                foreach (Transform child in _tileContainer)
                    Destroy(child.gameObject);
            }
        }
        private void SetMouseTile()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float dist))
            {
                var p = ray.GetPoint(dist);
                var hex = _hexTool.WorldToHex(p.x, p.z);

                if (hex != _mouseHex)
                {
                    _mouseHex = hex;
                    OnMouseTileChange?.Invoke(_mouseHex);
                }
            }
        }

        private void OnDrawGizmos()
        {
            _hexTool.ToWorld(_mouseHex, out float x, out float z);
            var p = new Vector3(x, 0, z);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(p, .25f);
        }
    }
}