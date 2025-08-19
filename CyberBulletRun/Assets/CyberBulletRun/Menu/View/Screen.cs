using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Shared.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CyberBulletRun.Menu.View 
{
    public sealed class Screen : BaseWindow
    {
        [SerializeField] private Button _gameButton; 
        [SerializeField] private Button _shopButton; 
        [SerializeField] private Button _optionsButton; 

        private readonly Stack<GameObject> _objects = new ();

        private Tween _canvasGroupTween;

        private ReactiveCommand<string> _showWindow;
        private string _gameScreenName;
        private string _shopScreenName;
        private string _optionsScreenName;

        public void Init(ReactiveCommand<string> showWindow, string gameScreenName, string shopScreenName, string optionsScreenName) {

            _showWindow = showWindow;
            _gameScreenName = gameScreenName;
            _shopScreenName = shopScreenName;
            _optionsScreenName = optionsScreenName;
            
            _gameButton.onClick.RemoveAllListeners();
            _gameButton.onClick.AddListener(OnGameClick);
            _shopButton.onClick.RemoveAllListeners();
            _shopButton.onClick.AddListener(OnShopClick);
            _optionsButton.onClick.RemoveAllListeners();
            _optionsButton.onClick.AddListener(OnOptionsClick);
        }

        private void OnGameClick() {
            _showWindow?.Execute(_gameScreenName);
        }
        private void OnShopClick() {
            _showWindow?.Execute(_shopScreenName);
        }
        private void OnOptionsClick() {
            _showWindow?.Execute(_optionsScreenName);
        }

        public override async UniTask Show()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.gameObject.SetActive(true);
            _canvasGroupTween?.Kill();
            _canvasGroupTween =  _canvasGroup.DOFade(1, 0.5f);
        }

        public override void Release() 
        {
            _canvasGroupTween?.Kill();
            while(_objects.Count > 0) 
            {
                GameObject.Destroy(_objects.Pop());
            }

            GameObject.Destroy(gameObject);
        }

    }
}

