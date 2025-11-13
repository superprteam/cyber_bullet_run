using System;
using CyberBulletRun.DataSet;
using UnityEngine;

namespace CyberBulletRun.Game {
    public struct CharacterData {
        public int HP;
        public int SkinId;
        public int WeaponId;
        public WeaponData Weapon;
        public bool IsEnemy;
    }
}
