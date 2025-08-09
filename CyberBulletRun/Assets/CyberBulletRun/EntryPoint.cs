using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.LowLevel;

namespace CyberBulletRun
{
    internal sealed class EntryPoint : MonoBehaviour
    {
        [SerializeField] private Data _data;

        private Entity _entity;

        private void OnEnable()
        {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopHelper.Initialize(ref playerLoop);

            _entity = new Entity(new Entity.Ctx
            {
                Data = _data,
            });
            _entity.AsyncProcess().Forget();
        }

        private void OnDisable()
        {
            _entity?.Dispose();
        }
    }
}

