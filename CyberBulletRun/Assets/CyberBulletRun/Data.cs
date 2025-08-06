using System;
using UnityEngine;

namespace CyberBulletRun 
{
    [Serializable]
    internal struct Data
    {
        [SerializeField] private Loading.Data _loadingData;
        [SerializeField] private Menu.Data _menuData;

        public readonly Loading.Data LoadingData => _loadingData;
        public readonly Menu.Data MenuData => _menuData;
    }
}


