using System;
using CyberBulletRun.DataSet;
using Cysharp.Threading.Tasks;
using UniRx;

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
        public ReactiveProperty<WeaponData> CurrentWeapon;
        public ReactiveProperty<SkinData> CurrentSkin;
    }
}
