using System;
using CyberBulletRun.DataSet;
using Cysharp.Threading.Tasks;

namespace CyberBulletRun.Managers.UIManager {
    [Serializable]
    public struct Data {
        public Loading.Data LoadingData;
        public Menu.Data MenuData;
        public Game.Data GameData;
        public Shop.Data ShopData;
        public Options.Data OptionsData;
        public DataSet.Data DataLoaded;
        public Func<string, UniTask<ItemStatus>> LoadItemStatus;
        public Func<string, ItemStatus, UniTask> SetItemStatus;
        public Func<UniTask<int>> GetCurrentWeapon;
        public Func<UniTask<int>> GetCurrentSkin;
        public Func<int, UniTask> SetCurrentWeapon;
        public Func<int, UniTask> SetCurrentSkin;
    }
}
