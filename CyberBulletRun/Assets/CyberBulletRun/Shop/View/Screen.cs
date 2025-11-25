using System;
using System.Threading.Tasks;
using CyberBulletRun.DataSet;
using Cysharp.Threading.Tasks;
using Shared.LocalCache;
using Shared.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CyberBulletRun.Shop.View 
{
    public class Screen : BaseWindow
    {
        [SerializeField] private Button _hideButton;
        [SerializeField] private Transform _itemsContainer;

        private Entity.Ctx _ctx;
        public override void SetOnHide(Action onHideCallback) {
            base.SetOnHide(onHideCallback);
            _hideButton.onClick.RemoveAllListeners();
            _hideButton.onClick.AddListener(OnHideButtonClick);
        }

        private void OnHideButtonClick() {
            _onHide?.Invoke();
        }

        public async UniTask ShowItems(Entity.Ctx ctx) {
            _ctx = ctx;
            var itemPrefab = await Cacher.GetBundleAsync("main", "Item");
            
            foreach (Transform child in _itemsContainer) {
                Destroy(child.gameObject);
            }

            var currentWeapon = _ctx.CurrentWeapon.Value;
            var currentSkin = _ctx.CurrentSkin.Value;
            string status;
            
            foreach (var weapon in _ctx.Weapons.Values) {
                if (weapon.Id == currentWeapon.Id) {
                    status = "SELECTED";
                } else {
                    status = (await _ctx.LoadItemStatus(weapon.ToString())).ToString();
                }
                var go = GameObject.Instantiate(itemPrefab as GameObject, _itemsContainer);
                go.GetComponent<ItemView>().Init(weapon.Name, status, OnClickItem);
            }
            
            foreach (var skin in _ctx.Skins.Values) {
                if (skin.Id == currentSkin.Id) {
                    status = "SELECTED";
                } else {
                    status = (await _ctx.LoadItemStatus(skin.ToString())).ToString();
                }
                var go = GameObject.Instantiate(itemPrefab as GameObject, _itemsContainer);
                go.GetComponent<ItemView>().Init(skin.Name, status, OnClickItem);
            }
            
        }

        private async void OnClickItem(ItemView itemView) {
            string itemName = itemView.GetItem();
            foreach (var weapon in _ctx.Weapons.Values) {
                if (weapon.Name == itemName) {
                    if ((await _ctx.LoadItemStatus(weapon.ToString())) == ItemStatus.AVAILABLE) {
                        _ctx.CurrentWeapon.Value = weapon;
                        await ShowItems(_ctx);
                        //itemView.Init(weapon.Name, "SELECTED", OnClickItem);
                    }
                    return;
                }
            }
            foreach (var skin in _ctx.Skins.Values) {
                if (skin.Name == itemName) {
                    if ((await _ctx.LoadItemStatus(skin.ToString())) == ItemStatus.AVAILABLE) {
                        _ctx.CurrentSkin.Value = skin;
                        await ShowItems(_ctx);
                        //itemView.Init(skin.Name, "SELECTED", OnClickItem);
                    }
                    return;
                }
            }
            
        }
    }
}

