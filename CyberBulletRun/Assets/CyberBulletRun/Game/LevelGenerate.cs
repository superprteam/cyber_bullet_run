using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    internal partial class LevelGenerate : BaseDisposable
    {
        public struct Ctx
        {
            public GameObject Root;
            public int LevelNumber;
            public ReactiveProperty<Stair> CurrentStair;
            public List<Stair> Stairs;
        }

        private readonly Ctx _ctx;
        private LevelData _levelData;
        private Dictionary<string, Stair> _stairPrefabs;
        private Stair _startPlatform;

        public LevelGenerate(Ctx ctx)
        {
            _ctx = ctx;
        }

        public async UniTask GenerateLevel()
        {
            var levelPath = $"Level{_ctx.LevelNumber}/Level.json";
            var levelText = await Cacher.GetTextAsync(levelPath);
            _levelData = JsonConvert.DeserializeObject<LevelData>(levelText);
            
            _stairPrefabs = new Dictionary<string, Stair>();
            foreach(var stairName in _levelData.Prefabs) {
                var stair = await Cacher.GetBundleAsync("main", stairName) as GameObject;
                _stairPrefabs.Add(stairName, stair.GetComponent<Stair>());
            }

            var startPlatformPrefab = await Cacher.GetBundleAsync("main", _levelData.StartPlatform) as GameObject;
            
            Debug.Log("Generate: " + _levelData.Length + ", " + _stairPrefabs.Count);

            var rand = new Random();
            Transform previousTop = null;
            // StartPlatform
            _startPlatform = GameObject.Instantiate(startPlatformPrefab, _ctx.Root.transform).GetComponent<Stair>();
            previousTop = _startPlatform.LinkPointUp;

            _startPlatform.AddTo(this);
            _ctx.Stairs.Add(_startPlatform);
            
            // Stairs
            for (int i = 0; i < _levelData.Length; i++)
            {
                var prefabKey = _stairPrefabs.Keys.ToList()[rand.Next(_stairPrefabs.Count)]; 
                var prefab = _stairPrefabs[prefabKey];
                var stair = GameObject.Instantiate(prefab, _ctx.Root.transform);
                
                stair.AddTo(this);
                _ctx.Stairs.Add(stair);
                
                if (i % 2 == 1)
                {
                    stair.transform.localRotation = Quaternion.Euler(0, 180, 0);
                }

                if (previousTop != null)
                {
                    var offset = previousTop.position - stair.LinkPointDown.position;
                    stair.transform.position += offset;
                }

                previousTop = stair.LinkPointUp;
            }

            await BakeNavMesh();

            await UniTask.NextFrame();
            
            _ctx.CurrentStair.Value = _startPlatform;
        }

        private async UniTask BakeNavMesh() {
            await UniTask.NextFrame();
            
            foreach (var surface in _startPlatform.NavMeshSurfaces) {
                surface.BuildNavMesh();
            }
            
            foreach(var stair in _ctx.Stairs) {
                foreach (var surface in stair.NavMeshSurfaces) {
                    surface.BuildNavMesh();
                }
            }
        }
    }
}
