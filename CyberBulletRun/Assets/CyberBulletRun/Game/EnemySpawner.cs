using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CyberBulletRun.Game.View;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Disposable;
using Shared.LocalCache;
using Shared.UI;
using UniRx;
using UnityEngine;
using Random = System.Random;
using Screen = UnityEngine.Screen;

namespace CyberBulletRun.Game
{
    internal class EnemySpawner : BaseDisposable
    {
        public struct Ctx
        {
            public GameObject Root;
            public LevelData LevelData;
            public ReactiveProperty<Stair> CurrentStair;
            public ReactiveCommand<Shot> ShotSpawn;
            public ReactiveCommand<ShotSpawner.ShotCollision> ShotCollision;
            public List<Stair> Stairs;
            public IController Controller;
        }

        private readonly Ctx _ctx;
        private Character _enemy;
        private CharacterView _enemyView;

        public EnemySpawner(Ctx ctx) {
            _ctx = ctx;
            _ctx.CurrentStair.Subscribe(async (stair) => await OnChangeCurrentStair(stair));
            _ctx.ShotCollision.Subscribe(async (shotCollision) => await OnShotCollision(shotCollision));
        }

        private async UniTask SpawnEnemy(Stair stair) {
            var characterPrefab = await Cacher.GetBundleAsync("main", "Character");
            _enemyView = GameObject.Instantiate(characterPrefab as GameObject, _ctx.Root.transform).GetComponent<CharacterView>();
            
            _enemy = new Character(new CharacterData() {
                HP = 1,
                SkinId = 1,
                WeaponId = 1,
            });
            await _enemy.Init(_ctx.Controller, _enemyView, _ctx.ShotSpawn);
            _ctx.Controller.SetPos(stair.EnemySpawnPoint.position, false);
            _ctx.Controller.SetPos(stair.EnemyPoint.position, true);

            var indexStair = _ctx.Stairs.IndexOf(stair);
            _ctx.Controller.SetTarget(_ctx.Stairs[indexStair-1].StopPoint.position);
        }

        private async UniTask OnChangeCurrentStair(Stair stair) {
            if (stair == null) {
                return;
            }

            if (_enemy == null || _enemy.Data.HP == 0) {
                if (_enemy != null) {
                    _enemy.Dispose();
                }

                var index = _ctx.Stairs.IndexOf(stair);
                if (index + 1 < _ctx.Stairs.Count) {
                    await SpawnEnemy(_ctx.Stairs[index + 1]);
                }
            }
        }
        
        private async UniTask OnShotCollision(ShotSpawner.ShotCollision shotCollision) {
            if (shotCollision.Collider != null && shotCollision.Collider.gameObject.CompareTag("Character")) {
                if (shotCollision.Collider.gameObject.transform.parent.GetComponent<CharacterView>() == _enemyView) {
                    _enemy.Data.HP--;
                    if (_enemy.Data.HP <= 0) {
                        var index = _ctx.Stairs.IndexOf(_ctx.CurrentStair.Value);
                        if (index + 1 < _ctx.Stairs.Count) {
                            _ctx.CurrentStair.Value = _ctx.Stairs[index + 1];
                        }
                    }
                }
            } else {
                if (shotCollision.IsLastCollision) {
                    Debug.Log("Enemy shot to player");
                }
            }
        }
        
    }
}
