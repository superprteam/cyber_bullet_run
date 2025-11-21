using System;
using System.Collections.Generic;
using System.Reflection;
using CyberBulletRun.DataSet;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Disposable;
using Shared.LocalCache;
using UniRx;
using UnityEngine;

namespace CyberBulletRun.Managers.DataLoader {
    public class DataLoader : IDisposable {
        
        public struct Ctx {
            public Data Data;
        }
        
        private CompositeDisposable _disposables;
        private Ctx _ctx;
        
        public DataLoader(Ctx ctx) {
            _ctx = ctx;
            
            _disposables = new CompositeDisposable();
        }

        public async UniTask Load() {
            _ctx.Data.Weapon = await Load<WeaponData>($"DataSet/weapons.json");
            _ctx.Data.Characters = await Load<CharacterData>($"DataSet/characters.json");
            _ctx.Data.Skins = await Load<SkinData>($"DataSet/skins.json");
            _ctx.Data.Levels = await Load<LevelData>($"DataSet/levels.json");
        }

        private async UniTask<Dictionary<int, T>> Load<T>(string path) {
            var dataText = await Cacher.GetTextAsync(path);
            var datas = JsonConvert.DeserializeObject<List<T>>(dataText);
            var dict = new Dictionary<int, T>();
            
            Type type = typeof(T);
        
            FieldInfo field = type.GetField("Id");            
            foreach (var data in datas) {
                dict.Add((int)field.GetValue(data), data);
            }

            return dict;
        }
        
        public void Dispose()
        {
            _disposables.Dispose();
        }        
    }
}
