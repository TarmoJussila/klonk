using Klonk.TileEntity.Data;
using UnityEngine;

namespace Klonk.TileEntity
{
    public enum SolidType { None, Rock, Sand }
    public enum LiquidType { None, Water, Acid }
    
    public class TileEntity
    {
        public Vector2Int Position { get; private set; }
        public SolidType SolidType { get; private set; }
        public LiquidType LiquidType { get; private set; }
        public bool IsSolid { get { return SolidType != SolidType.None; } }
        public bool IsLiquid { get { return LiquidType != LiquidType.None; } }
        public float Gravity { get; private set; }
        public Vector2 Velocity { get; private set; }
        public int Health { get; private set; }
        public int Potency { get; private set; }
        public Color TileColor { get { return _tileColor; } }

        public TileData TileData { get; private set; }

        public int LastUpdateFrame { get; private set; } = -1;

        private Color _tileColor;
        private int _lastSpawnSourceCount;
        
        public TileEntity(Vector2Int position, LiquidType liquidType, SolidType solidType, bool isSpawnSource = false)
        {
            TileData = TileEntityHandler.Instance.EntityData.GetTileDataForType(solidType, liquidType, isSpawnSource);
            Position = position;
            SolidType = solidType;
            LiquidType = liquidType;
            Gravity = TileData.Gravity;
            Health = TileData.Health;
            Potency = TileData.Potency;
            _tileColor = TileData.ColorPalette[Random.Range(0, TileData.ColorPalette.Length)];
        }

        public Vector2Int UpdateEntity(int updateFrame)
        {
            if (IsLiquid || TileData.IsSpawnSource)
            {
                if (TileData.IsSpawnSource)
                {
                    if (_lastSpawnSourceCount >= TileData.SpawnSpeed)
                    {
                        _lastSpawnSourceCount = 0;
                    }
                    else
                    {
                        _lastSpawnSourceCount++;
                        return Position;
                    }
                }

                if (!TileEntityHandler.Instance.TryGetTileEntityAtPosition(new Vector2Int(Mathf.Clamp(Position.x, default, TileEntityHandler.Instance.GenerationData.GenerationWidth - 1), Mathf.Max(Position.y - 1, default)), out TileEntity otherTile1))
                {
                    var position = new Vector2Int(Mathf.Clamp(Position.x, default, TileEntityHandler.Instance.GenerationData.GenerationWidth - 1), Mathf.Max(Position.y - 1, default));
                    if (!TileData.IsSpawnSource)
                    {
                        Position = position;
                    }
                    return position;
                }
                int direction = GetRandomDirection();
                if (!TileEntityHandler.Instance.TryGetTileEntityAtPosition(new Vector2Int(Mathf.Clamp(Position.x + direction, default, TileEntityHandler.Instance.GenerationData.GenerationWidth - 1), Mathf.Max(Position.y, default)), out TileEntity otherTile2))
                {
                    var position = new Vector2Int(Mathf.Clamp(Position.x + direction, default, TileEntityHandler.Instance.GenerationData.GenerationWidth - 1), Mathf.Max(Position.y, default));
                    if (!TileData.IsSpawnSource)
                    {
                        Position = position;
                    }
                    return position;
                }

                TileEntity otherTile = otherTile1 ?? otherTile2;
                if (otherTile != null)
                {
                    if (LiquidType == LiquidType.Acid && otherTile.LiquidType != LiquidType.Acid && !otherTile.TileData.IsAcidResistant)
                    {
                        if (Potency > 0)
                        {
                            otherTile.ReduceHealth();
                            Potency--;
                        }
                    }
                }
            }
            LastUpdateFrame = updateFrame;
            return Position;
        }

        public int ReduceHealth(int amount = 1)
        {
            return Health -= amount;
        }

        private int GetRandomDirection()
        {
            return Random.Range(0, 2) == 0 ? -1 : 1;
        }

        public void SetLiquid(LiquidType type)
        {
            LiquidType = type;
            SolidType = SolidType.None;
        }
    }
}