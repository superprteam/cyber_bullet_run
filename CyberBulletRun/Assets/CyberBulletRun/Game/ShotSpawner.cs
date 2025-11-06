using System.Collections.Generic;
using CyberBulletRun.Game.View;
using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace CyberBulletRun.Game
{
    public class ShotSpawner : BaseDisposable
    {
        private const float DESTROY_TIME = 1.5f;
        public struct Ctx
        {
            public GameObject Root;
            public ReactiveCommand<Shot> ShotSpawn;
            public ReactiveCommand<ShotCollision> ShotCollision;
        }

        public struct ShotCollision {
            public Shot Shot;
            public Collider Collider;
        }
        
        private enum State {
            FREE = 0,
            BUSY = 1,
        }
        
        private readonly Ctx _ctx;
        private List<ShotView> _shotViews;
        private State _currentState;
        public ShotSpawner(Ctx ctx) {
            _ctx = ctx;
            _ctx.ShotSpawn.Subscribe(async (shot) => await OnShotSpawn(shot));
            _shotViews = new List<ShotView>();
            _currentState = State.FREE;
        }

        public async UniTask SpawnShot(Shot shot) {
            ShotView shotView = null;
            
            foreach (var shotViewPool in _shotViews) {
                if (!shotViewPool.IsVisible) {
                    shotView = shotViewPool;
                    break;
                }
            }

            if (shotView == null) {
                var shotPrefab = await Cacher.GetBundleAsync("main", "Shot");
                shotView = GameObject.Instantiate(shotPrefab as GameObject, _ctx.Root.transform).GetComponent<ShotView>();
                _shotViews.Add(shotView);
            }
            shotView.Init(shot, DESTROY_TIME, ShotCollisionCallback);
            _currentState = State.BUSY;
        }

        private void ShotCollisionCallback(ShotView shotView, Collider collider) {
            _currentState = State.FREE;
            _ctx.ShotCollision.Execute(new ShotCollision() {
                Shot = shotView.GetShot(),
                Collider = collider,
            });
        }

        private async UniTask OnShotSpawn(Shot shot) {
            if(_currentState == State.BUSY) {
                return;
            }
            await SpawnShot(shot);
        }

        public void Update() {
            for (int i = 0; i < _shotViews.Count; i++) {
                if (_shotViews[i].IsVisible) {
                    _shotViews[i].UpdateInternal();
                }
            }
        }
    }
}
