using CyberBulletRun.Loading.View;
using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using Shared.Requests;
using Shared.UI;
using UnityEngine;

namespace CyberBulletRun.Loading
{
    public sealed class Entity : BaseDisposable
    {
        public struct Ctx
        {
            public Data Data;
        }

        private IWindow _window;
        private Ctx _ctx;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;
        }

        public async UniTask Init() 
        {
            var asset = await Cacher.GetBundleAsync("main", _ctx.Data.ScreenName);
            var go = GameObject.Instantiate(asset as GameObject);
            _window = go.GetComponent<IWindow>();
        }

        public void ShowImmediate() => _window.ShowImmediate();
        public void HideImmediate() => _window.HideImmediate();

        public async UniTask Show() => await _window.Show();
        public async UniTask Hide() => await _window.Hide();
        
        protected override void OnDispose()
        {
            base.OnDispose();
            _window.Release();
        }
    }
}
