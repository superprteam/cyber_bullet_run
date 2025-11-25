using System;
using CyberBulletRun.DataSet;
using UnityEngine;

namespace CyberBulletRun.Game
{
    public struct Shot {
        public Vector3 StartPos;
        public Vector3 Direction;
        public WeaponData Weapon;
    }
}
