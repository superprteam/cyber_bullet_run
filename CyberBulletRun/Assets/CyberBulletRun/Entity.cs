using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using System;

namespace CyberBulletRun 
{
    internal sealed class Entity : BaseDisposable
    {
        public struct Ctx
        {
            public Data Data;
        }

        private Loading.Entity _loading;
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
            _loading.ShowImmediate();

            await ShowMainMenu();
        }

        private async UniTask<Menu.Entity> ShowMainMenu() 
        {
            await _loading.Show();

            var mainScreen = new Menu.Entity(new Menu.Entity.Ctx 
            {
                Data = _ctx.Data.MenuData,
            }).AddTo(this);
            await mainScreen.Init();
            await mainScreen.Show();

            await _loading.Hide();

            return mainScreen;
        }
    }
}
