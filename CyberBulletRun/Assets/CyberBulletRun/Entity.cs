using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using System;
using CyberBulletRun.Managers.UIManager;
using UnityEngine;
using UniRx;

namespace CyberBulletRun 
{
    internal sealed class Entity : BaseDisposable
    {
        public struct Ctx
        {
            public Data Data;
        }

        private Loading.Entity _loading;
        private Menu.Entity _menu;
        private BaseDisposable _lastWindow;
        private readonly Ctx _ctx;
        private UIManager _uiManager;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;
        }

        public async UniTask AsyncProcess() {
            
            _uiManager = new UIManager(new UIManager.Ctx {
                Data = new Managers.Data {
                    LoadingData = _ctx.Data.LoadingData,
                    MenuData = _ctx.Data.MenuData,
                    GameData = _ctx.Data.GameData,
                    ShopData = _ctx.Data.ShopData,
                    OptionsData = _ctx.Data.OptionsData,
                    }
            });

            _uiManager.ShowWindow.Execute(_ctx.Data.LoadingData.ScreenName);
        }
    }
}
