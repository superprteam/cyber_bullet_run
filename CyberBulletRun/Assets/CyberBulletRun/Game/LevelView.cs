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
    internal partial class LevelView : BaseDisposable
    {
        public struct Ctx
        {
            public GameObject Root;
            public int LevelNumber;
            public ReactiveProperty<Stair> CurrentStair;
        }

        private readonly Ctx _ctx;
        private LevelData _levelData;
        private Dictionary<string, Stair> _stairPrefabs;

        public LevelView(Ctx ctx)
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

            Debug.Log("Generate: " + _levelData.Length + ", " + _stairPrefabs.Count);

            var rand = new Random();
            Transform previousTop = null;
            Stair firstStair = null;
            for (int i = 0; i < _levelData.Length; i++)
            {
                var prefabKey = _stairPrefabs.Keys.ToList()[rand.Next(_stairPrefabs.Count)]; 
                var prefab = _stairPrefabs[prefabKey];
                var stair = GameObject.Instantiate(prefab, _ctx.Root.transform);
                stair.AddTo(this);
                if (i == 0) {
                    firstStair = stair;
                }
                
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

            _ctx.CurrentStair.Value = firstStair;
        }
    }
}
