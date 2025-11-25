using System;
using System.Collections.Generic;
using CyberBulletRun.DataSet;
using CyberBulletRun.Managers.PlayerData.DataSet;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace CyberBulletRun.Managers.PlayerData {
    public class PlayerData : IDisposable {
        
        public class Ctx {
            public Dictionary<int, WeaponData> Weapons;
            public Dictionary<int, SkinData> Skins;
            public ReactiveProperty<WeaponData> CurrentWeapon;
            public ReactiveProperty<SkinData> CurrentSkin;
        }
        
        private CompositeDisposable _disposables;
        private Ctx _ctx;
        private List<ItemData> _items;
        
        public PlayerData(Ctx ctx) {
            _ctx = ctx;

            _disposables = new CompositeDisposable();
        }

        public async UniTask Load() {
            _items = new List<ItemData>();
            await UnlockAll();
            ItemStatus status;
            foreach (var weapon in _ctx.Weapons.Values) {
                status = await LoadStatus(weapon.ToString());
                _items.Add(new ItemData() {
                    ID = weapon.Id,
                    ItemType = ItemType.WEAPON,
                    ItemStatus = status,
                });
            }
            foreach (var skin in _ctx.Skins.Values) {
                status = await LoadStatus(skin.ToString());
                _items.Add(new ItemData() {
                    ID = skin.Id,
                    ItemType = ItemType.SKIN,
                    ItemStatus = status,
                });
            }
            
            _ctx.CurrentWeapon.Value = await GetCurrentWeapon();
            _ctx.CurrentSkin.Value = await GetCurrentSkin();

            _ctx.CurrentWeapon.Subscribe(async (weapon) => await OnChangeCurrentWeapon(weapon));
            _ctx.CurrentSkin.Subscribe(async (skin) => await OnChangeCurrentSkin(skin));
        }

        private async UniTask UnlockAll() {
            ItemStatus status;
            foreach (var weapon in _ctx.Weapons.Values) {
                status = await LoadStatus(weapon.ToString());
                if (status == ItemStatus.LOCKED) {
                    await SetStatus(weapon.ToString(), ItemStatus.AVAILABLE);
                }
            }
            foreach (var skin in _ctx.Skins.Values) {
                status = await LoadStatus(skin.ToString());
                if (status == ItemStatus.LOCKED) {
                    await SetStatus(skin.ToString(), ItemStatus.AVAILABLE);
                }
            }
        }
        
        public async UniTask<ItemStatus> LoadStatus(string key) {
            var itemStatus = (ItemStatus)PlayerPrefs.GetInt(key, 0);
            return itemStatus;
        }

        public async UniTask SetStatus(string key, ItemStatus status) {
            PlayerPrefs.SetInt(key, (int)status);
        }

        private async UniTask<WeaponData> GetCurrentWeapon() {
            var weaponId = PlayerPrefs.GetInt("weapon", 1);
            return _ctx.Weapons[weaponId];
        } 

        private async UniTask<SkinData> GetCurrentSkin() {
            var skinId = PlayerPrefs.GetInt("skin", 1);
            return _ctx.Skins[skinId];
        }

        private async UniTask OnChangeCurrentWeapon(WeaponData weapon) {
            PlayerPrefs.SetInt("weapon", weapon.Id);            
        }

        private async UniTask OnChangeCurrentSkin(SkinData skin) {
            PlayerPrefs.SetInt("skin", skin.Id);            
        }
        
        public void Dispose()
        {
            _disposables.Dispose();
        }        
    }
}
