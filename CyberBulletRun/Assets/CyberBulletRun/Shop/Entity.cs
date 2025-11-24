using System;
using System.Collections.Generic;
using CyberBulletRun.DataSet;
using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using UniRx;
using UnityEngine;

namespace CyberBulletRun.Shop 
{
    public sealed class Entity : BaseDisposable
    {
        public struct Ctx
        {
            public Data Data;
            public ReactiveCommand<string> ShowWindow;
            public ReactiveCommand<string> HideWindow;
            public Dictionary<int, WeaponData> Weapons;
            public Dictionary<int, SkinData> Skins;
            
            public Func<string, UniTask<ItemStatus>> LoadItemStatus;
            public Func<string, ItemStatus, UniTask> SetItemStatus;
            public Func<UniTask<int>> GetCurrentWeapon;
            public Func<UniTask<int>> GetCurrentSkin;
            public Func<int, UniTask> SetCurrentWeapon;
            public Func<int, UniTask> SetCurrentSkin;
        }

        private View.Screen _window;
        private readonly Ctx _ctx;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;
        }

        public async UniTask Init() {
            var asset = await Cacher.GetBundleAsync("main", _ctx.Data.ScreenName);
            var go = GameObject.Instantiate(asset as GameObject);
            _window = go.GetComponent<View.Screen>();
            _window.SetOnHide(async () => {
                await Hide();
            });

            await _window.ShowItems(_ctx);
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