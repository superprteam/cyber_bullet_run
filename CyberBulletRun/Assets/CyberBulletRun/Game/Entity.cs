using System;
using System.Collections.Generic;
using System.Linq;
using CyberBulletRun.DataSet;
using CyberBulletRun.Game.Controllers;
using CyberBulletRun.Game.View;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Disposable;
using Shared.LocalCache;
using Shared.UI;
using UniRx;
using UnityEngine;
using Screen = CyberBulletRun.Game.View.Screen;

namespace CyberBulletRun.Game 
{
    public sealed class Entity : BaseDisposable
    {
        public struct Ctx
        {
            public Data Data;
            public ReactiveCommand<string> ShowWindow;
            public ReactiveCommand<string> HideWindow;
            
            public DataSet.Data DataLoaded;
        }

        private IWindow _window;
        private LevelGenerate _levelGenerate;
        private ShotSpawner _shotSpawner;
        private EnemySpawner _enemySpawner;
        private CameraController _cameraController;
        private readonly Ctx _ctx;
        private ReactiveProperty<Stair> _currentStair;
        private List<Stair> _stairs;
        private Character _player;
        private LevelData _levelData;
        private ReactiveCommand<Shot> _shotSpawn;
        private ReactiveCommand<ShotSpawner.ShotCollision> _shotCollision;
        private ReactiveCommand _nextStair;
        private ReactiveCommand<EndGameData> _endGame;
        private ReactiveCommand<int> _keyPressed;
        
        public Entity(Ctx ctx)
        {
            _ctx = ctx;
        }

        public async UniTask Init() {
            var asset = await Cacher.GetBundleAsync("main", _ctx.Data.ScreenName);
            var window = GameObject.Instantiate(asset as GameObject);
            _window = window.GetComponent<IWindow>();
            _window.SetOnHide(async () => {
                await Hide();
            });

            int levelNumber = 1;
            
            _levelData = _ctx.DataLoaded.Levels[levelNumber];
            
            
            _currentStair = new ReactiveProperty<Stair>();
            _stairs = new List<Stair>();
            _shotSpawn = new ReactiveCommand<Shot>();
            _shotCollision = new ReactiveCommand<ShotSpawner.ShotCollision>();
            _nextStair = new ReactiveCommand();
            _endGame = new ReactiveCommand<EndGameData>();
            _keyPressed = new ReactiveCommand<int>();

            ((Screen)_window).SetCommands(_endGame, _keyPressed);
            
            _levelGenerate = new LevelGenerate(new LevelGenerate.Ctx {
                Root = window,
                LevelData = _levelData,
                CurrentStair = _currentStair,
                Stairs = _stairs,
                Characters = _ctx.DataLoaded.Characters,
            }).AddTo(this);

            // enemy
            var enemyController = new AIController(new AIController.AIControllerCtx() {
                CurrentStair = _currentStair,
                Stairs = _stairs,
                EndGame = _endGame,
                KeyPressed = _keyPressed,
            });
            
            _enemySpawner = new EnemySpawner(new EnemySpawner.Ctx {
                Root = window,
                CurrentStair = _currentStair,
                Stairs = _stairs,
                Controller = enemyController,
                ShotCollision = _shotCollision,
                ShotSpawn = _shotSpawn,
                NextStair = _nextStair,
                DataLoaded = _ctx.DataLoaded,
                LevelData = _levelData,
            }).AddTo(this);
            
            // player
            var characterPrefab = await Cacher.GetBundleAsync("main", "Character");
            var playerView = GameObject.Instantiate(characterPrefab as GameObject, window.transform).GetComponent<CharacterView>();

            var playerData = new CharacterData() {
                Id = 0,
                Name = "player",
                HP = 1,
                SkinId = 1,
                WeaponId = 1,
            };
            var weapon = _ctx.DataLoaded.Weapons[playerData.WeaponId];
            
            _player = new Character(new CharacterDataRealtime(playerData, weapon, false));
            
            var playerController = new PlayerController(new PlayerController.PlayerControllerCtx() {
                CurrentStair = _currentStair,
                Stairs = _stairs,
                NextStair = _nextStair,
                EndGame = _endGame,
                KeyPressed = _keyPressed,
            });
            await _player.Init(playerController, playerView, _shotSpawn, _nextStair, _shotCollision);
            
            // stairs
            _cameraController = new CameraController(new CameraController.Ctx {
                CameraScreen = Camera.allCameras[0],
                CurrentStair = _currentStair,
                Root = window,
            });

            _shotSpawner = new ShotSpawner(new ShotSpawner.Ctx() {
                Root = window,
                ShotSpawn = _shotSpawn,
                ShotCollision = _shotCollision,
            });
            
            await _levelGenerate.GenerateLevel();
            
            Observable.EveryUpdate().Subscribe(_ => {
                    playerController.Update();
                    enemyController.Update();
                    _shotSpawner.Update();
            }).AddTo(this);
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