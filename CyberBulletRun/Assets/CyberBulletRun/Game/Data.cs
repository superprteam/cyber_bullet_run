using System;
using UnityEngine;

namespace CyberBulletRun.Game
{
    [Serializable]
    public struct Data
    {
        [SerializeField] private string _screenName;

        public readonly string ScreenName => _screenName;
    }
}
