using System;

namespace CyberBulletRun.Managers.UIManager {
    [Serializable]
    public struct Data {
        public Loading.Data LoadingData;
        public Menu.Data MenuData;
        public Game.Data GameData;
        public Shop.Data ShopData;
        public Options.Data OptionsData;
    }
}
