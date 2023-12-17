using DG.Tweening;
using UnityEngine;

namespace Game.Gates
{
    public abstract class BaseGate : MonoBehaviour
    {
        protected bool _isKilled;

        #region UNITY EVENTS

        protected abstract void Start();

        protected abstract void OnTriggerEnter(Collider other);

        #endregion

        #region PROTECTED METHODS

        protected virtual void InteractEffect()
        {
            transform.DOComplete();
            transform.DOShakeScale(0.15f, new Vector3(0.1f, 0.1f, 0.1f));
        }

        protected virtual void KillGate()
        {
            _isKilled = true;
            transform.DOComplete();
            transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InElastic)
                .OnComplete(() => Destroy(gameObject));
        }

        #endregion
    }
}