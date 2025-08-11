using System;
using Cysharp.Threading.Tasks;
using Shared.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CyberBulletRun.Options.View 
{
    public class Screen : BaseWindow
    {
        [SerializeField] private Button _hideButton;

        public override void SetOnHideCallback(Action onHideCallback) {
            base.SetOnHideCallback(onHideCallback);
            _hideButton.onClick.RemoveAllListeners();
            _hideButton.onClick.AddListener(OnHideButtonClick);
        }

        private async void OnHideButtonClick() {
            await Hide();
        }
        
    }
}

