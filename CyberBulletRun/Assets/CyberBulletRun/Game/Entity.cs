using System;
using System.Collections.Generic;
using CyberBulletRun.Game.Controllers;
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
        private Func<List<Stair>> GetStair;
        private Character _player;
        
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
            
            _levelGenerate = new LevelGenerate(new LevelGenerate.Ctx {
                Root = go,
                LevelNumber = 1,
                CurrentStair = _currentStair,
            }).AddTo(this);

            GetStair = _levelGenerate.GetStair;
            
            // player
            var characterPrefab = await Cacher.GetBundleAsync("main", "Character");
            var playerView = GameObject.Instantiate(characterPrefab as GameObject).GetComponent<CharacterView>();
            
            _player = new Character(new CharacterData());
            var playerController = new PlayerController(new PlayerController.PlayerControllerCtx() {
                Data = _player.Data,
                CurrentStair = _currentStair,
                GetStair = GetStair,
            });
            _player.Init(playerController, playerView);
            
            // stairs
            _cameraController = new CameraController(new CameraController.Ctx {
                CameraScreen = Camera.allCameras[0],
                CurrentStair = _currentStair,
                Root = go,
            });
            
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