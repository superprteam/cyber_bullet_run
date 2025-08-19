using System;
using Cysharp.Threading.Tasks;
using Shared.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CyberBulletRun.Shop.View 
{
    public class Screen : BaseWindow
    {
        [SerializeField] private Button _hideButton;

        public override void SetOnHide(Action onHideCallback) {
            base.SetOnHide(onHideCallback);
            _hideButton.onClick.RemoveAllListeners();
            _hideButton.onClick.AddListener(OnHideButtonClick);
        }

        private void OnHideButtonClick() {
            _onHide?.Invoke();
        }
        
    }
}

