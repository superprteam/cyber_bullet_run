using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using System;
using System.Collections.Generic;
using CyberBulletRun.DataSet;
using CyberBulletRun.Managers.DataLoader;
using CyberBulletRun.Managers.UIManager;
using UnityEngine;
using UniRx;

namespace CyberBulletRun 
{
    internal sealed class Entity : BaseDisposable
    {
        public struct Ctx
        {
            public Data Data;
        }

        private readonly Ctx _ctx;
        private UIManager _uiManager;
        private DataLoader _resourceLoader;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;
        }

        public async UniTask AsyncProcess() {

            var levels = new Dictionary<int, LevelData>();
            var characters = new Dictionary<int, CharacterData>();
            var weapon = new Dictionary<int, WeaponData>();
            var skins = new Dictionary<int, SkinData>();

            var dataLoaded = new DataSet.Data() {
                Levels = levels,
                Characters = characters,
                Weapon = weapon,
                Skins = skins,
            };
            
            _resourceLoader = new DataLoader(new DataLoader.Ctx() {
                Data = dataLoaded, 
            });

            await _resourceLoader.Load();
            
            _uiManager = new UIManager(new UIManager.Ctx {
                Data = new Managers.UIManager.Data {
                    LoadingData = _ctx.Data.LoadingData,
                    MenuData = _ctx.Data.MenuData,
                    GameData = _ctx.Data.GameData,
                    ShopData = _ctx.Data.ShopData,
                    OptionsData = _ctx.Data.OptionsData,
                    DataLoaded = dataLoaded, 
                    }
            });

            _uiManager.ShowWindow.Execute(_ctx.Data.LoadingData.ScreenName);
        }
    }
}
