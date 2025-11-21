using System;
using CyberBulletRun.DataSet;
using UnityEngine;

namespace CyberBulletRun.Game {
    public struct CharacterDataRealtime {
        public int HP;
        public WeaponData Weapon;
        public bool IsEnemy;
        public CharacterData CharacterData;
        
        public CharacterDataRealtime(CharacterData characterData, WeaponData weapon, bool isEnemy) {
            HP = characterData.HP;
            Weapon = weapon;
            CharacterData = characterData;
            IsEnemy = isEnemy;
        }
        
    }
}
