using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using Shared.Requests;
using System;
using CyberBulletRun.Options.View;
using Shared.UI;
using UnityEngine;

namespace CyberBulletRun.Options 
{
    public sealed class Entity : BaseDisposable
    {
        public struct Ctx
        {
            public Data Data;
        }

        private IWindow _window;
        private readonly Ctx _ctx;

        private Action _onHide;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;
        }

        public async UniTask Init(Action hideCallback) {
            _onHide = hideCallback;
            var asset = await Cacher.GetBundleAsync("main", _ctx.Data.ScreenName);
            var go = GameObject.Instantiate(asset as GameObject);
            _window = go.GetComponent<IWindow>();
            _window.SetOnHideCallback(_onHide);
        }

        public void ShowImmediate() => _window.ShowImmediate();
        public void HideImmediate() {
            _window.HideImmediate();
            _onHide?.Invoke();
        }

        public async UniTask Show() => await _window.Show();
        public async UniTask Hide() {
            await _window.Hide();
            _onHide?.Invoke();
        }

        protected override void OnDispose()
        {
            _window.Release();
        }
    }
}