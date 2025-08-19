using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using Shared.Requests;
using System;
using CyberBulletRun.Options.View;
using Shared.UI;
using UniRx;
using UnityEngine;

namespace CyberBulletRun.Options 
{
    public sealed class Entity : BaseDisposable
    {
        public struct Ctx
        {
            public Data Data;
            public ReactiveCommand<string> ShowWindow;
            public ReactiveCommand<string> HideWindow;
        }

        private IWindow _window;
        private readonly Ctx _ctx;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;
        }

        public async UniTask Init() {
            var asset = await Cacher.GetBundleAsync("main", _ctx.Data.ScreenName);
            var go = GameObject.Instantiate(asset as GameObject);
            _window = go.GetComponent<IWindow>();
            _window.SetOnHide(async () => {
                await Hide();
            });
        }

        public void ShowImmediate() => _window.ShowImmediate();
        public void HideImmediate() {
            _window.HideImmediate();
            _ctx.HideWindow?.Execute(_ctx.Data.ScreenName);
        }

        public async UniTask Show() => await _window.Show();
        public async UniTask Hide() {
            await _window.Hide();
            _ctx.HideWindow?.Execute(_ctx.Data.ScreenName);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            _window.Release();
        }
    }
}