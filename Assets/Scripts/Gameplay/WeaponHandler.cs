using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Klonk.Platforming;
using Klonk.TileEntity;
using Klonk.TileEntity.Data;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Klonk
{
    [Serializable]
    public class Weapon
    {
        public int TileDataIndex;
    }

    public class WeaponHandler : MonoBehaviour
    {
        public static WeaponHandler Instance;

        [SerializeField] private Weapon[] _weapons;

        private int _currentWeaponIndex;
        private Weapon _currentWeapon;

        private void Awake()
        {
            Instance = this;
            _currentWeaponIndex = 0;
            _currentWeapon = _weapons[_currentWeaponIndex];
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKey(KeyCode.LeftControl))
            {
                UseWeapon();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                ScrollWeapon(-1);
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
            {
                ScrollWeapon(1);
            }
        }

        public void ScrollWeapon(int direction)
        {
            _currentWeaponIndex += direction;
            if (_currentWeaponIndex < 0)
            {
                _currentWeaponIndex = _weapons.Length - 1;
            }
            else if (_currentWeaponIndex > _weapons.Length - 1)
            {
                _currentWeaponIndex = 0;
            }

            _currentWeapon = _weapons[_currentWeaponIndex];
        }

        private void UseWeapon()
        {
            int direction = PlayerMovement.Instance.GetHorizontalDirection();

            Vector3 size = PlayerMovement.Instance.Rigidbody.BoxCollider.size;
            Vector3 charPos = PlayerMovement.Instance.transform.position;
            int safeX = Mathf.FloorToInt(charPos.x + (direction * ((size.x + 4) / 2)));
            int safeY = Mathf.FloorToInt(charPos.y) - 2; //Mathf.FloorToInt(charPos.y + (direction.y * ((size.y + 2) / 2)));

            if (!TileEntityHandler.Instance.IsInBounds(safeX, safeY))
            {
                return;
            }

            if (TileEntityHandler.Instance.TryGetTileEntityAtPosition(safeX, safeY, out _))
            {
                return;
            }

            Vector2Int pos = new(safeX, safeY);

            TileData tileToSpawn = TileEntityHandler.Instance.EntityData.GetTileDataByIndex(_currentWeapon.TileDataIndex);

            var tileEntity = new Klonk.TileEntity.TileEntity(pos, tileToSpawn.LiquidType, tileToSpawn.SolidType, tileToSpawn.IsSpawnSource);

            TileEntityHandler.Instance.TryAddTileToPosition(tileEntity, pos);
        }
    }
}
