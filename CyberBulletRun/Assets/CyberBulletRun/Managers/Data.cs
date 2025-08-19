using System;
using UnityEngine;

namespace CyberBulletRun.Managers {
    [Serializable]
    public struct Data {
        public Loading.Data LoadingData;
        public Menu.Data MenuData;
        public Game.Data GameData;
        public Shop.Data ShopData;
        public Options.Data OptionsData;

    }
}
