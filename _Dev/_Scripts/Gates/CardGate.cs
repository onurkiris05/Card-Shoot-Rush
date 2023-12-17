using System.Collections.Generic;
using DG.Tweening;
using Game.Cards;
using Game.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Gates
{
    [System.Serializable]
    public class CardGateStat
    {
        public int ProgressToUpgrade;
        public int UpgradeAmount;
    }


    public class CardGate : BaseGate
    {
        [Header("Settings")]
        [SerializeField] CardGateStat[] gateStats;
        [SerializeField] protected int cardCount;
        [SerializeField] protected int cardPower;
        [SerializeField] protected float anglePerCard;

        [Space] [Header("Components")]
        [SerializeField] protected BaseCard cardPrefab;
        [SerializeField] protected Transform showcase;
        [SerializeField] protected Slider bar;
        [SerializeField] protected TextMeshProUGUI maxText;

        protected List<BaseCard> _cards = new();
        protected int _currentProgress;
        protected int _statIndex;
        protected bool _isMaxed;


        #region UNITY EVENTS

        protected override void Start()
        {
            InitGate();
        }


        protected override void OnTriggerEnter(Collider other)
        {
            if (_isKilled) return;

            if (other.TryGetComponent(out BaseCard card))
            {
                // Process hit VFX
                var vfxPos = other.ClosestPointOnBounds(transform.position);
                VFXSpawner.Instance.PlayVFX("CardHit", vfxPos);

                InteractEffect();
                card.ReturnEarly();

                if (_isMaxed) return;

                UpdateGate(card.Power);
            }
            else if (other.TryGetComponent(out PlayerController player))
            {
                player.CardGateReward(_cards);
                KillGate();
            }
        }

        #endregion

        #region PROTECTED METHODS

        protected virtual void InitGate()
        {
            // Create cards
            for (int i = 0; i < cardCount; i++)
            {
                var card = Instantiate(cardPrefab, transform);
                card.transform.localScale *= 0.8f;
                card.SetPower(cardPower);
                _cards.Add(card);
            }

            FormatCards();
        }


        protected virtual void FormatCards()
        {
            var currentAngle = -((anglePerCard * (_cards.Count - 1)) / 2);

            for (int i = 0; i < _cards.Count; i++)
            {
                var pos = new Vector3(0, showcase.localPosition.y, i * 0.2f);
                var rot = Quaternion.Euler(0, 0, currentAngle);
                _cards[i].transform.SetLocalPositionAndRotation(pos, rot);

                currentAngle += anglePerCard;
            }
        }


        protected virtual void UpdateGate(int power)
        {
            _currentProgress += power;

            UpdateBar();

            if (_currentProgress >= gateStats[_statIndex].ProgressToUpgrade)
            {
                UpgradeCards();

                _currentProgress -= gateStats[_statIndex].ProgressToUpgrade;

                if (_statIndex >= gateStats.Length - 1)
                    _isMaxed = true;
                else
                    _statIndex++;

                UpdateBar(_isMaxed);
            }
        }


        protected virtual void UpdateBar(bool isMaxed = false)
        {
            maxText.gameObject.SetActive(isMaxed);
            var currentHitRatio = Mathf.Min((float)_currentProgress / gateStats[_statIndex].ProgressToUpgrade, 1f);
            var amount = isMaxed ? 1f : currentHitRatio;

            DOTween.Complete(this);
            DOTween.To(x => bar.value = x, bar.value, amount, 0.15f)
                .SetEase(Ease.Linear).SetId(this);
        }


        protected virtual void UpgradeCards()
        {
            foreach (var card in _cards)
            {
                var power = card.Power + gateStats[_statIndex].UpgradeAmount;
                card.SetPower(power);
            }
        }


        protected override void InteractEffect()
        {
            base.InteractEffect();

            foreach (var card in _cards)
            {
                card.transform.DOComplete();
                card.transform.DOShakeRotation(0.5f, new Vector3(0, 20f, 0));
            }
        }

        #endregion
    }
}