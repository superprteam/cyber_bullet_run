using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using System;
using System.Collections.Generic;
using CyberBulletRun.DataSet;
using CyberBulletRun.Managers.DataLoader;
using CyberBulletRun.Managers.PlayerData;
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
        private PlayerData _playerData;

        public Entity(Ctx ctx)
        {
            _ctx = ctx;
        }

        public async UniTask AsyncProcess() {

            var dataLoaded = new DataSet.Data();
            
            _resourceLoader = new DataLoader(new DataLoader.Ctx() {
                Data = dataLoaded, 
            });

            await _resourceLoader.Load();

            var levels = dataLoaded.Levels;
            var characters = dataLoaded.Characters;
            var weapons = dataLoaded.Weapons;
            var skins = dataLoaded.Skins;

            ReactiveProperty<WeaponData> currentWeapon = new ReactiveProperty<WeaponData>().AddTo(this);
            ReactiveProperty<SkinData> currentSkin = new ReactiveProperty<SkinData>().AddTo(this);
            
            _playerData = new PlayerData(new PlayerData.Ctx() {
                Weapons = weapons,
                Skins = skins,
                CurrentWeapon = currentWeapon,
                CurrentSkin = currentSkin,
            });

            await _playerData.Load();
            
            _uiManager = new UIManager(new UIManager.Ctx {
                Data = new Managers.UIManager.Data {
                    LoadingData = _ctx.Data.LoadingData,
                    MenuData = _ctx.Data.MenuData,
                    GameData = _ctx.Data.GameData,
                    ShopData = _ctx.Data.ShopData,
                    OptionsData = _ctx.Data.OptionsData,
                    DataLoaded = dataLoaded,
                    LoadItemStatus = _playerData.LoadStatus,
                    SetItemStatus = _playerData.SetStatus,
                    CurrentWeapon = currentWeapon,
                    CurrentSkin = currentSkin,
                    }
            });

            _uiManager.ShowWindow.Execute(_ctx.Data.LoadingData.ScreenName);
        }
    }
}
