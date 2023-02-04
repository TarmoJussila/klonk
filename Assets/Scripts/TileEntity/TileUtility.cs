using UnityEngine;

namespace Klonk.TileEntity
{
    public static class TileUtility
    {
        public const int TILES_PER_UNIT = 1;
        public const float TILE_SIZE = 1f / TILES_PER_UNIT;

        public static Vector2Int WorldToTileCoordinates(Vector3 worldCoordinates)
        {
            Vector3 coordinates = worldCoordinates * TILES_PER_UNIT;
            return new Vector2Int((int)coordinates.x, (int)coordinates.y);
        }

        public static Vector3 TileToWorldCoordinates(Vector2Int tileCoordinates)
        {
            Vector2Int transformed = tileCoordinates * (int)TILE_SIZE;
            return new Vector3(transformed.x, transformed.y, default);
        }

        public static void DestroyInArea(Vector2Int center, int radius)
        {
            int centerX = center.x;
            int centerY = center.y;
            
            for (int x = centerX - radius; x < centerX + radius; x++)
            {
                for (int y = centerY - radius; y < centerY + radius; y++)
                {
                    if ((Mathf.Pow(x - centerX, 2) + Mathf.Pow(y - centerY, 2)) >= Mathf.Pow(radius, 2))
                    {
                        continue;
                    }

                    if (TileEntityHandler.Instance.TryGetTileEntityAtPosition(x, y, out var tile))
                    {
                        tile.ReduceHealth(100);
                    }
                }
            }
        }

        public static void LiquifyInArea(Vector2Int center, int radius)
        {
            int centerX = center.x;
            int centerY = center.y;
            
            for (int x = centerX - radius; x < centerX + radius; x++)
            {
                for (int y = centerY - radius; y < centerY + radius; y++)
                {
                    if ((Mathf.Pow(x - centerX, 2) + Mathf.Pow(y - centerY, 2)) >= Mathf.Pow(radius, 2))
                    {
                        continue;
                    }
                    
                    if (TileEntityHandler.Instance.TryGetTileEntityAtPosition(x, y, out var tile))
                    {
                        tile.SetLiquid(LiquidType.Water);    
                    }
                }
            }
        }
    }
}
