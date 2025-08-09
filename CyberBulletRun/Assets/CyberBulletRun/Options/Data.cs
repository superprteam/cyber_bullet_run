using System;
using UnityEngine;

namespace CyberBulletRun.Options
{
    [Serializable]
    public struct Data
    {
        [SerializeField] private string _screenName;

        public readonly string ScreenName => _screenName;
    }
}
