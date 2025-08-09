using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using Shared.Requests;
using System;
using CyberBulletRun.Options.View;
using UnityEngine;

namespace CyberBulletRun.Options 
{
    public sealed class Entity : BaseDisposable
    {
        public struct Ctx
        {
            public Data Data;
        }

        private IScreen _screen;
        private readonly Ctx _ctx;

        private Action _onHide;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;
        }

        public async UniTask Init(Action hideCallback) {
            _onHide = hideCallback;
            var asset = await Cacher.GetBundle("main", _ctx.Data.ScreenName);
            var go = GameObject.Instantiate(asset as GameObject);
            _screen = go.GetComponent<IScreen>();
            _screen.Init(_onHide);
        }

        public void ShowImmediate() => _screen.ShowImmediate();
        public void HideImmediate() {
            _screen.HideImmediate();
            _onHide?.Invoke();
        }

        public async UniTask Show() => await _screen.Show();
        public async UniTask Hide() {
            await _screen.Hide();
            _onHide?.Invoke();
        }

        protected override void OnDispose()
        {
            _screen.Release();
        }
    }
}