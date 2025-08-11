using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using Shared.Requests;
using System;
using System.Collections.Generic;
using CyberBulletRun.Menu.View;
using Shared.UI;
using UnityEngine;
using Screen = CyberBulletRun.Menu.View.Screen;

namespace CyberBulletRun.Menu 
{
    public sealed class Entity : BaseDisposable
    {
        public struct Ctx
        {
            public Data Data;
        }

        private IWindow _window;
        private readonly Ctx _ctx;
        private Action<string> _onButtonClick;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;
        }

        public async UniTask Init(Action<string> OnButtonClick) {
            _onButtonClick = OnButtonClick;
            var asset = await Cacher.GetBundleAsync("main", _ctx.Data.ScreenName);
            var go = GameObject.Instantiate(asset as GameObject);
            _window = go.GetComponent<IWindow>();
            ((Screen)_window).Init(_onButtonClick);
        }

        public async UniTask Show() => await _window.Show();
        public void HideImmediate() => _window.HideImmediate();

        protected override void OnDispose()
        {
            base.OnDispose();
            HideImmediate();
            _window.Release();
        }
    }
}

