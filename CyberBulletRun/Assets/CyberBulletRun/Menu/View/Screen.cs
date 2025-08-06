using Cysharp.Threading.Tasks;
using Shared.Disposable;
using Shared.LocalCache;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CyberBulletRun.Menu.View 
{
    public interface IScreen
    {
        public UniTask Show();
        public void HideImmediate();
        public void Release();
    }

    public sealed class Screen : MonoBehaviour, IScreen
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        private readonly Stack<GameObject> _objects = new ();

        public async UniTask Show()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.gameObject.SetActive(true);
            await UniTask.Delay(50);
            _canvasGroup.gameObject.SetActive(false);
            await UniTask.Delay(50);
            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.alpha = 1f;
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

