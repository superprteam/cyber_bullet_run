using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CyberBulletRun.Menu.View 
{
    public interface IScreen
    {
        public UniTask Show();
        public void HideImmediate();
        public void Release();
        public void Init(Action<string> onButtonClick);
    }

    public sealed class Screen : MonoBehaviour, IScreen
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _gameButton; 
        [SerializeField] private Button _shopButton; 
        [SerializeField] private Button _optionsButton; 

        private readonly Stack<GameObject> _objects = new ();
        private Action<string> _onButtonClick;

        public void Init(Action<string> onButtonClick) {
            _onButtonClick = onButtonClick;
            
            _gameButton.onClick.RemoveAllListeners();
            _gameButton.onClick.AddListener(OnGameClick);
            _shopButton.onClick.RemoveAllListeners();
            _shopButton.onClick.AddListener(OnShopClick);
            _optionsButton.onClick.RemoveAllListeners();
            _optionsButton.onClick.AddListener(OnOptionsClick);
        }

        private void OnGameClick() {
            _onButtonClick("game");
        }
        private void OnShopClick() {
            _onButtonClick("shop");
        }
        private void OnOptionsClick() {
            _onButtonClick("options");
        }

        public async UniTask Show()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.DOFade(1, 0.5f);
        }

        public void HideImmediate()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.gameObject.SetActive(false);
        }

        public void Release() 
        {
            while(_objects.Count > 0) 
            {
                GameObject.Destroy(_objects.Pop());
            }

            GameObject.Destroy(gameObject);
        }

    }
}

