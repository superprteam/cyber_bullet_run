using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using Shared.Requests;
using System;
using System.Collections.Generic;
using CyberBulletRun.Menu.View;
using UnityEngine;

namespace CyberBulletRun.Menu 
{
    public sealed class Entity : BaseDisposable
    {
        public struct Ctx
        {
            public Data Data;
        }

        private IScreen _screen;
        private readonly Ctx _ctx;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;
        }

        public async UniTask Init()
        {
            var asset = await Cacher.GetBundle("main", _ctx.Data.ScreenName);
            var go = GameObject.Instantiate(asset as GameObject);
            _screen = go.GetComponent<IScreen>();
        }

        public async UniTask Show() => await _screen.Show();
        public void HideImmediate() => _screen.HideImmediate();

        protected override void OnDispose()
        {
            base.OnDispose();
            HideImmediate();
            _screen.Release();
        }
    }
}

