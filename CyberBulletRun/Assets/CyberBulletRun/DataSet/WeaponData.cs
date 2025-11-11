using System;
using System.Collections.Generic;

namespace CyberBulletRun.DataSet {
    [Serializable]
    public class WeaponData {
        public int Id;
        public string Name;
        public float Speed;
        public float Damage;
        public float Spread;
        public int Ammo;
        public float Radius;
        public float RadiusSpeed;
    }
}