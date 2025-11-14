using System;
using System.Collections.Generic;
using CyberBulletRun.Game.View;
using Cysharp.Threading.Tasks;
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
            public ReactiveCommand NextStair;
        }

        private readonly Ctx _ctx;
        private Character _enemy;
        private CharacterView _enemyView;

        public EnemySpawner(Ctx ctx) {
            _ctx = ctx;
            _ctx.CurrentStair.Subscribe(async (stair) => await OnChangeCurrentStair(stair));
        }

        private async UniTask SpawnEnemy(Stair stair) {
            var characterPrefab = await Cacher.GetBundleAsync("main", "Character");
            _enemyView = GameObject.Instantiate(characterPrefab as GameObject, stair.EnemySpawnPoint.position + new Vector3(0, 0.1f, 0), Quaternion.identity, _ctx.Root.transform).GetComponent<CharacterView>();
            
            _enemy = new Character(new CharacterData() {
                HP = 1,
                SkinId = 1,
                WeaponId = 1,
                IsEnemy = true,
            });
            await _enemy.Init(_ctx.Controller, _enemyView, _ctx.ShotSpawn, _ctx.NextStair, _ctx.ShotCollision);
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
    }
}
