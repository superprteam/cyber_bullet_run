using CyberBulletRun.Game.View;
using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using Shared.UI;
using UniRx;
using UnityEngine;

namespace CyberBulletRun.Game 
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
        private LevelGenerate _levelGenerate;
        private CameraController _cameraController;
        private readonly Ctx _ctx;
        private ReactiveProperty<Stair> _currentStair;
        
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

            _currentStair = new ReactiveProperty<Stair>();

            _cameraController = new CameraController(new CameraController.Ctx {
                CameraScreen = Camera.main,
                CurrentStair = _currentStair,
                Root = go,
            });
            
            _levelGenerate = new LevelGenerate(new LevelGenerate.Ctx {
                Root = go,
                LevelNumber = 1,
                CurrentStair = _currentStair,
            }).AddTo(this);

            await _levelGenerate.GenerateLevel();
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