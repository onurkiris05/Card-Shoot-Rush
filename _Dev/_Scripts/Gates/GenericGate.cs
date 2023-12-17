using DG.Tweening;
using Game.Cards;
using Game.Player;
using TMPro;
using UnityEngine;

namespace Game.Gates
{
    public class GenericGate : BaseGate
    {
        [Header("Settings")]
        [SerializeField] protected GenericGateType genericGateType;
        [SerializeField] protected float rewardValue;
        [SerializeField] protected bool isLocked;
        [SerializeField] [Range(1, 5)] protected int unlockCount;

        [Space] [Header("Components")]
        [SerializeField] protected GameObject normalGate;
        [SerializeField] protected GameObject redGate;
        [SerializeField] protected GameObject lockedGate;
        [SerializeField] protected TextMeshPro headerText;
        [SerializeField] protected TextMeshPro valueText;
        [SerializeField] protected LockRopeController lockRopeController;


        #region UNITY EVENTS

        protected override void Start()
        {
            InitGate(genericGateType, rewardValue);
        }


        protected override void OnTriggerEnter(Collider other)
        {
            if (_isKilled) return;

            if (other.TryGetComponent(out BaseCard card))
            {
                // Process hit VFX
                var vfxPos=other.ClosestPointOnBounds(transform.position);
                VFXSpawner.Instance.PlayVFX("CardHit", vfxPos);
                
                InteractEffect();
                card.ReturnEarly();

                if (isLocked)
                {
                    ProcessUnlockRope();
                    return;
                }

                UpdateGate(card.Power);
            }
            else if (other.TryGetComponent(out PlayerController player))
            {
                if (!isLocked)
                {
                    var gateData = new GenericGateData(genericGateType, rewardValue);
                    player.GenericGateReward(gateData);
                    KillGate();
                }
            }
        }

        #endregion

        #region PROTECTED METHODS

        protected override void KillGate()
        {
            _isKilled = true;
            transform.DOComplete();
            transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InElastic)
                .OnComplete(() =>
                    {
                        lockRopeController.KillRopes();
                        Destroy(gameObject);
                    }
                );
        }


        protected virtual void InitGate(GenericGateType genericGateType, float value)
        {
            if (isLocked)
            {
                lockRopeController.gameObject.SetActive(true);
                lockRopeController.Init(unlockCount);
            }

            SetGate(value);

            switch (genericGateType)
            {
                case GenericGateType.Rate:
                    headerText.text = "Rate";
                    break;
                case GenericGateType.Range:
                    headerText.text = "Range";
                    break;
            }
        }


        protected virtual void SetGate(float value)
        {
            var isNormalGate = value > 0;
            valueText.text = isNormalGate ? $"+{value}" : $"{value}";

            TextInteractEffect();

            lockedGate.SetActive(isLocked);
            normalGate.SetActive(!isLocked && isNormalGate);
            redGate.SetActive(!isLocked && !isNormalGate);
        }


        protected virtual void UpdateGate(float power)
        {
            rewardValue += power;
            SetGate(rewardValue);
        }


        protected virtual void ProcessUnlockRope()
        {
            if (!lockRopeController.TearRope())
            {
                isLocked = false;
                SetGate(rewardValue);
            }
        }


        protected override void InteractEffect()
        {
            base.InteractEffect();

            headerText.transform.DOComplete();
            headerText.transform.DOShakeRotation(0.5f, new Vector3(0, 25f, 0));

            valueText.transform.DOComplete();
            valueText.transform.DOShakeRotation(0.5f, new Vector3(0, 25f, 0));
        }


        protected virtual void TextInteractEffect()
        {
            valueText.DOComplete();
            valueText.DOColor(Color.green, 0.15f).From();

            headerText.DOComplete();
            headerText.DOColor(Color.green, 0.15f).From();
        }

        #endregion
    }
}