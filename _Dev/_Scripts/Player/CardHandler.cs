using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Cards;
using UnityEngine;

namespace Game.Player
{
    public class CardHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int startCardCount;
        [SerializeField] private int cardPower;
        [SerializeField] private float anglePerCard;

        [Space][Header("Components")]
        [SerializeField] private BaseCard cardPrefab;
        [SerializeField] private Transform cardHolder;

        private PlayerController _player;
        private List<BaseCard> _currentCards = new();


        #region PUBLIC METHODS

        public void Init(PlayerController player)
        {
            _player = player;
            Card_Create(startCardCount);
            Card_Format();
        }


        public void ProcessCardGateReward(List<BaseCard> cards)
        {
            StartCoroutine(ProcessRewardCards(cards));
        }


        public Vector3 GetAvailableSlot()
        {
            var availablePos = _currentCards.Any() ? _currentCards[^1].transform.position : cardHolder.position;
            return availablePos;
        }


        public BaseCard Card_Get()
        {
            if (!_currentCards.Any())
                return null;

            var card = _currentCards[0];
            _currentCards.RemoveAt(0);
            Card_Format();
            return card;
        }


        public void Card_Retrieve(BaseCard card)
        {
            card.transform.parent = cardHolder;
            card.transform.localScale = cardPrefab.transform.localScale;
            _currentCards.Add(card);
            Card_Format();
        }

        #endregion

        #region PRIVATE METHODS

        private void Card_Create(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var card = Instantiate(cardPrefab, cardHolder.transform);

                _currentCards.Add(card);
                card.Init(_player);
                card.SetPower(cardPower);
            }
        }


        private void Card_Remove(BaseCard removedCard)
        {
            _currentCards.Remove(removedCard);
            Destroy(removedCard.gameObject);
        }


        private void Card_Format()
        {
            var currentAngle = -((anglePerCard * (_currentCards.Count - 1)) / 2);

            for (int i = 0; i < _currentCards.Count; i++)
            {
                var t = _currentCards[i].transform;

                t.DOLocalMove(new Vector3(0, 0, i * 0.1f), 0.1f)
                    .SetEase(Ease.Linear);
                t.DOLocalRotate(new Vector3(0, 0, currentAngle), 0.1f)
                    .SetEase(Ease.Linear);
                currentAngle += anglePerCard;
            }
        }


        private IEnumerator ProcessRewardCards(List<BaseCard> cards)
        {
            foreach (var card in cards)
            {
                var t = card.transform;

                t.DOLocalRotate(new Vector3(45f, t.localRotation.y, t.localRotation.z), 0.15f)
                    .OnComplete(() =>
                    {
                        card.Init(_player);
                        Card_Retrieve(card);
                    });

                yield return Helpers.BetterWaitForSeconds(0.15f);
            }
        }

        #endregion
    }
}