using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using System;
using UnityEngine;

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

        public Entity(Ctx ctx)
        {
            _ctx = ctx;
        }

        public async UniTask AsyncProcess()
        {
            _loading = new Loading.Entity(new Loading.Entity.Ctx
            {
                Data = _ctx.Data.LoadingData,
            }).AddTo(this);
            
            await _loading.Init();
            
            await _loading.Show();

            _menu = await ShowMainMenu(OnButtonClick);
        }
        
        private async UniTask<Menu.Entity> ShowMainMenu(Action<string> clickCallback) 
        {
            var mainScreen = new Menu.Entity(new Menu.Entity.Ctx 
            {
                Data = _ctx.Data.MenuData,
            }).AddTo(this);
            
            await mainScreen.Init(clickCallback);
            await mainScreen.Show();

            await _loading.Hide();

            return mainScreen;
        }
        
        private void OnButtonClick(string name) {
            Debug.Log("Main menu OnButtonClick " + name);
            ShowWindow(name,OnHideWindow);
        }
        
        private async void ShowWindow(string windowName, Action hideCallback) 
        {
            switch (windowName) {
                case "options":
                    var optionsScreen = new Options.Entity(new Options.Entity.Ctx {
                        Data = _ctx.Data.OptionsData,
                    }).AddTo(this);
                    _lastWindow = optionsScreen;
                    await optionsScreen.Init(hideCallback);
                    await optionsScreen.Show();
                    break;
            }
        }
        
        private async void OnHideWindow() {
            await _menu.Show();
            _lastWindow?.Dispose();
        }
        
    }
}
