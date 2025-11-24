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

        public async UniTask<int> GetCurrentWeapon() {
            var weaponId = PlayerPrefs.GetInt("weapon", 1);
            return weaponId;
        } 

        public async UniTask<int> GetCurrentSkin() {
            var skinId = PlayerPrefs.GetInt("skin", 1);
            return skinId;
        }

        public async UniTask SetCurrentWeapon(int id) {
            PlayerPrefs.SetInt("weapon", id);            
        }

        public async UniTask SetCurrentSkin(int id) {
            PlayerPrefs.SetInt("skin", id);            
        }
        
        public void Dispose()
        {
            _disposables.Dispose();
        }        
    }
}
