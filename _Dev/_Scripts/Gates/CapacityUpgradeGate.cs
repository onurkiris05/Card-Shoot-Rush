using System.Collections.Generic;
using DG.Tweening;
using Game.Cards;
using Game.Managers;
using Game.Player;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace Game.Gates
{
    public class CapacityUpgradeGate : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int cardCount;
        [SerializeField] private int[] upgradeProgresses;
        [SerializeField] private float anglePerCard;

        [Space] [Header("Components")]
        [SerializeField] private Transform conveyorEnd;
        [SerializeField] private Transform cardHolder;
        [SerializeField] private Material activeMat;
        [SerializeField] private Material deactiveMat;
        [SerializeField] private BaseCard cardPrefab;
        [SerializeField] private TextMeshPro capacityText;

        [Space] [Header("Debug")]
        [SerializeField] [ReadOnly] private float currentProgress;
        [SerializeField] [ReadOnly] private int currentCapacity = 1;

        public Vector3 EndPos => conveyorEnd.position;

        private List<BaseCard> _cards = new();
        private List<BaseCard> _activeCards = new();
        private bool _isTriggered;
        private bool _isPlayingEffect;
        private bool _isMaxedOut;


        #region UNITY EVENTS

        private void Start()
        {
            InitGate();
            FormatCards();
            UpdateCards();
            UpdateCapacityText();
        }


        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered) return;

            if (other.TryGetComponent(out PlayerController player))
            {
                player.ProcessThrowCapacity(currentCapacity);
                KillGate();
            }
        }

        #endregion

        #region PUBLIC METHODS

        public void UpdateGate(float progressAmount)
        {
            currentProgress += progressAmount;

            if (!_isMaxedOut && currentProgress >= upgradeProgresses[currentCapacity - 1])
            {
                currentProgress -= upgradeProgresses[currentCapacity - 1];

                currentCapacity++;
                if (currentCapacity > upgradeProgresses.Length)
                    _isMaxedOut = true;

                UpdateCards();
                UpdateCapacityText();
            }

            ProcessInteractEffect();
        }

        #endregion

        #region PRIVATE METHODS

        private void UpdateCards()
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                _cards[i].UpdateMaterial(i < currentCapacity ? activeMat : deactiveMat);

                if (!_activeCards.Contains(_cards[i]) && i < currentCapacity)
                    _activeCards.Add(_cards[i]);
            }
        }


        private void UpdateCapacityText()
        {
            capacityText.text = $"+{currentCapacity - 1} Capacity";
        }


        private void ProcessInteractEffect()
        {
            if (_isPlayingEffect) return;

            _isPlayingEffect = true;

            capacityText.transform.DOShakeRotation(0.2f, new Vector3(0, 20f, 0));

            foreach (var card in _activeCards)
                card.transform.DOShakeRotation(0.2f, new Vector3(0, 20f, 0))
                    .OnComplete(() => _isPlayingEffect = false);
        }


        private void InitGate()
        {
            // Create cards
            for (int i = 0; i < cardCount; i++)
            {
                var card = Instantiate(cardPrefab, transform);
                card.transform.localScale *= 1.2f;
                card.SetPower(1);
                card.UpdateTexts(false);
                _cards.Add(card);

                if (i < currentCapacity)
                    _activeCards.Add(card);
            }
        }


        private void FormatCards()
        {
            var currentAngle = -((anglePerCard * (cardCount - 1)) / 2);

            for (int i = 0; i < cardCount; i++)
            {
                var pos = new Vector3(cardHolder.localPosition.x, cardHolder.localPosition.y,
                    cardHolder.localPosition.z + (i * 0.15f));
                var rot = Quaternion.Euler(0, 0, currentAngle);
                _cards[i].transform.SetLocalPositionAndRotation(pos, rot);

                currentAngle += anglePerCard;
            }
        }


        private void KillGate()
        {
            _isTriggered = true;
            transform.DOComplete();
            transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)
                .OnComplete(() => Destroy(gameObject));

            ConveyorManager.Instance.MoveToNextGate();
        }

        #endregion
    }
}