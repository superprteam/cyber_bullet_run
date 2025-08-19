using System;
using UnityEngine;

namespace CyberBulletRun 
{
    [Serializable]
    internal struct Data
    {
        [SerializeField] private Loading.Data _loadingData;
        [SerializeField] private Menu.Data _menuData;
        [SerializeField] private Game.Data _gameData;
        [SerializeField] private Shop.Data _shopData;
        [SerializeField] private Options.Data _optionsData;

        public readonly Loading.Data LoadingData => _loadingData;
        public readonly Menu.Data MenuData => _menuData;
        public readonly Game.Data GameData => _gameData;
        public readonly Shop.Data ShopData => _shopData;
        public readonly Options.Data OptionsData => _optionsData;
    }
}


