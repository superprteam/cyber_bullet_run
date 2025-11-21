using System;
using Cysharp.Threading.Tasks;
using Shared.Disposable;
using UnityEngine;
using UniRx;

namespace CyberBulletRun.Managers.UIManager {
    public class UIManager : IDisposable {
        
        public struct Ctx
        {
            public Data Data;
        }
        
        public ReactiveCommand<string> ShowWindow;
        public ReactiveCommand<string> HideWindow;


        private Menu.Entity _mainScreen;
        
        private CompositeDisposable _disposables;
        private BaseDisposable _lastWindow;
        private Ctx _ctx;
        
        public UIManager(Ctx ctx) {
            _ctx = ctx;
            
            _disposables = new CompositeDisposable();
            
            ShowWindow = new ReactiveCommand<string>().AddTo(_disposables);
            HideWindow = new ReactiveCommand<string>().AddTo(_disposables);
            
            ShowWindow.Subscribe(ShowWindowCommand);
            HideWindow.Subscribe(HideWindowCommand);
        }
        
        private async void ShowWindowCommand(string windowName) {
            Debug.Log("Show window " + windowName);
            if (windowName.Equals(_ctx.Data.LoadingData.ScreenName)) {
                var loading = new Loading.Entity(new Loading.Entity.Ctx
                {
                    Data = _ctx.Data.LoadingData,
                    ShowWindow = ShowWindow,
                    HideWindow = HideWindow,
                }).AddTo(_disposables);
                
                _lastWindow = loading;
                    
                await loading.Init();
                await loading.Show();
                await loading.Hide();
            } else if (windowName.Equals(_ctx.Data.GameData.ScreenName)) {
                _ctx.Data.GameData.DataLoaded = _ctx.Data.DataLoaded;
                var gameScreen = new Game.Entity(new Game.Entity.Ctx {
                    Data = _ctx.Data.GameData,
                    ShowWindow = ShowWindow,
                    HideWindow = HideWindow,
                }).AddTo(_disposables);
                
                _lastWindow = gameScreen;
                
                await gameScreen.Init();
                await _mainScreen.Hide();
                await gameScreen.Show();
            } else if (windowName.Equals(_ctx.Data.ShopData.ScreenName)) {
                var shopScreen = new Shop.Entity(new Shop.Entity.Ctx {
                    Data = _ctx.Data.ShopData,
                    ShowWindow = ShowWindow,
                    HideWindow = HideWindow,
                }).AddTo(_disposables);
                
                _lastWindow = shopScreen;
                
                await shopScreen.Init();
                await shopScreen.Show();
            } else if (windowName.Equals(_ctx.Data.OptionsData.ScreenName)) {
                var optionsScreen = new Options.Entity(new Options.Entity.Ctx {
                    Data = _ctx.Data.OptionsData,
                    ShowWindow = ShowWindow,
                    HideWindow = HideWindow,
                }).AddTo(_disposables);
                
                _lastWindow = optionsScreen;
                
                await optionsScreen.Init();
                await optionsScreen.Show();
            } else if (windowName.Equals(_ctx.Data.MenuData.ScreenName)) {
                if (_mainScreen == null) {
                    _mainScreen = new Menu.Entity(new Menu.Entity.Ctx {
                        Data = _ctx.Data.MenuData,
                        ShowWindow = ShowWindow,
                        HideWindow = HideWindow,
                        GameScreenName = _ctx.Data.GameData.ScreenName,
                        ShopScreenName = _ctx.Data.ShopData.ScreenName,
                        OptionsScreenName = _ctx.Data.OptionsData.ScreenName,
                    }).AddTo(_disposables);

                    await _mainScreen.Init();
                }

                await _mainScreen.Show();
            }
        }

        private async void HideWindowCommand(string windowName) {
            Debug.Log("Hide window " + windowName);
            if (windowName.Equals(_ctx.Data.LoadingData.ScreenName)) {
                ShowWindow?.Execute(_ctx.Data.MenuData.ScreenName);
                if (_lastWindow != null) {
                    _disposables.Remove(_lastWindow);
                    _lastWindow?.Dispose();
                    _lastWindow = null;
                }
            }
            if (windowName.Equals(_ctx.Data.GameData.ScreenName)) {
                ShowWindow?.Execute(_ctx.Data.MenuData.ScreenName);
                if (_lastWindow != null) {
                    _disposables.Remove(_lastWindow);
                    _lastWindow?.Dispose();
                    _lastWindow = null;
                }
            }
            if (windowName.Equals(_ctx.Data.ShopData.ScreenName)) {
                ShowWindow?.Execute(_ctx.Data.MenuData.ScreenName);
                if (_lastWindow != null) {
                    _disposables.Remove(_lastWindow);
                    _lastWindow?.Dispose();
                    _lastWindow = null;
                }
            }
            if (windowName.Equals(_ctx.Data.OptionsData.ScreenName)) {
                ShowWindow?.Execute(_ctx.Data.MenuData.ScreenName);
                if (_lastWindow != null) {
                    _disposables.Remove(_lastWindow);
                    _lastWindow?.Dispose();
                    _lastWindow = null;
                }
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }        
    }
}
