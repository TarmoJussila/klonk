using System;
using UnityEngine;
using Klonk.TileEntity;
using UnityEngine.Serialization;

namespace Klonk.Rendering
{
    public class WorldRenderer : MonoBehaviour
    {
        public static WorldRenderer Instance { get; private set; }

        public Camera Camera { get; private set; }
        public int Width => _width;
        public int Height => _height;
        public int TextureResDivider { get; private set; } = 4;

        [SerializeField] private float _moveSpeed = 10;
        [SerializeField] private Material _material;
        [SerializeField] private Color _skyColor;
        [SerializeField] private Color _outOfBoundsColor;
        [SerializeField] private Color _caveColor;
        [SerializeField] private Camera _normalCamera;
        [FormerlySerializedAs("player")]
        [SerializeField]
        private Transform _player;

        private Texture2D _texture;
        private int _lastScreenWidth, _lastScreenHeight;
        private int _width, _height;
        private readonly int _minDivider = 4;
        private readonly int _maxDivider = 12;

        private void Awake()
        {
            Instance = this;

            Camera = GetComponent<Camera>();

            _material.EnableKeyword("_WorldTex");
            _material.EnableKeyword("_CameraPos");
            _material.EnableKeyword("_WorldResolution");

            ResetTexture();
        }

        private void LateUpdate()
        {
            CheckAspect();
            Vector3 targetPosition = _player.position + Vector3.down * Height / 2 + Vector3.left * Width / 2;
            transform.position = Vector3.Lerp(transform.position, targetPosition, _moveSpeed * Time.deltaTime);

            _normalCamera.transform.position = new Vector3(
                transform.position.x + Width / 2f,
                transform.position.y + Height / 2f,
                _normalCamera.transform.position.z
            );
            _normalCamera.orthographicSize = Height / 2f;

            Vector3 position = transform.position;

            for (int textureX = 0; textureX < Width; textureX++)
            {
                for (int textureY = 0; textureY < Height; textureY++)
                {
                    Color c = Color.magenta;

                    int coordsX = Mathf.FloorToInt(position.x) + textureX;
                    int coordsY = Mathf.FloorToInt(position.y) + textureY;

                    if (!TileEntityHandler.Instance.IsInBounds(coordsX, coordsY))
                    {
                        c = coordsY >= TileEntityHandler.Instance.TileEntities.GetLength(1) ? _skyColor : _outOfBoundsColor;
                    }
                    else if (!TileEntityHandler.Instance.TryGetTileEntityAtPosition(coordsX, coordsY, out TileEntity.TileEntity tile))
                    {
                        if (coordsY > TileEntityHandler.Instance.GenerationData.GenerationHeight - TileEntityHandler.GroundStart)
                        {
                            c = _skyColor;
                        }
                        else
                        {
                            c = _caveColor;
                        }
                    }
                    else
                    {
                        c = tile.TileColor;
                    }

                    _texture.SetPixel(textureX, textureY, c);
                }
            }

            _texture.Apply();

            _material.SetTexture("_WorldTex", _texture);
            _material.SetVector("_CameraPos", position);
        }

        private void CheckAspect(bool force = false)
        {
            if (_lastScreenWidth != Screen.width || _lastScreenHeight != Screen.height || force)
            {
                _lastScreenWidth = Screen.width;
                _lastScreenHeight = Screen.height;
                _width = Screen.width / TextureResDivider;
                _height = Screen.height / TextureResDivider;

                ResetTexture();
            }
        }

        [ContextMenu("Zoom In")]
        public void ZoomIn()
        {
            ChangeZoom(1);
        }

        [ContextMenu("Zoom Out")]
        public void ZoomOut()
        {
            ChangeZoom(-1);
        }

        private void ChangeZoom(int direction)
        {
            TextureResDivider = Math.Clamp(TextureResDivider + direction, _minDivider, _maxDivider);
            CheckAspect(true);
        }

        private void ResetTexture()
        {
            _texture = new Texture2D(_width, _height, TextureFormat.ARGB32, false);
            _texture.wrapMode = TextureWrapMode.Clamp;

            _material.SetVector("_WorldResolution", new Vector2(_width, _height));
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, _material);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector2(transform.position.x + _width / 2, transform.position.y + _height / 2), new Vector2(_width, _height));
        }
    }
}
