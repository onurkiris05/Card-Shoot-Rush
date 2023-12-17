using System.Collections.Generic;
using DG.Tweening;
using Game.Cards;
using Game.Gates;
using Game.Incrementals;
using Game.Managers;
using UnityEngine;

namespace Game.Player
{
    public class ThrowHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int throwCapacity;
        [SerializeField] private float throwRate;
        [SerializeField] private float throwRateDivider;
        [SerializeField] private float throwRange;
        [SerializeField] private float throwRangeDivider;

        [Space] [Header("Components")]
        [SerializeField] private Transform cardThrower;

        private List<BaseCard> _holdingCards = new();
        private List<float[]> _angleOffsets = new();
        private PlayerController _player;

        #region ENCAPSULATIONS

        public float ThrowRate
        {
            get { return throwRate; }

            private set
            {
                throwRate = value;
                _player.OnThrowRateUpdate(throwRate);
            }
        }

        #endregion

        #region UNITY EVENTS

        private void Start()
        {
            _player.OnThrowRateUpdate(throwRate);

            InitAngles();
        }

        #endregion

        #region PUBLIC METHODS

        public void Init(PlayerController player) => _player = player;


        public void SetThrowCapacity(int capacity)
        {
            if (capacity > throwCapacity)
                throwCapacity = capacity;

            Debug.Log($"New throw capacity ({throwCapacity}) settled!");
        }


        public void ProcessGenericGateReward(GenericGateData genericGateData)
        {
            switch (genericGateData.Type)
            {
                case GenericGateType.Range:
                    Debug.Log($"Throw Range: {throwRange} += {genericGateData.Amount}/{throwRangeDivider}");
                    throwRange += genericGateData.Amount / throwRangeDivider;
                    Debug.Log($"Current Throw Range: {throwRange}");
                    break;

                case GenericGateType.Rate:
                    Debug.Log($"Throw Rate: {ThrowRate} += {genericGateData.Amount}/{throwRateDivider}");
                    ThrowRate += genericGateData.Amount / throwRateDivider;
                    Debug.Log($"Current Throw Rate: {ThrowRate}");
                    break;
            }
        }


        public void ProcessButtonReward(IncrementalButton button)
        {
            if (!EconomyManager.Instance.SpendMoney(button.Cost)) return;

            switch (button.Type)
            {
                case ButtonType.Range:
                    Debug.Log($"Throw Range: {throwRange} += {button.Amount}/{throwRangeDivider}");
                    throwRange += button.Amount / throwRangeDivider;
                    Debug.Log($"Current Throw Range: {throwRange}");
                    break;

                case ButtonType.Rate:
                    Debug.Log($"Throw Rate: {ThrowRate} += {button.Amount}/{throwRateDivider}");
                    ThrowRate += button.Amount / throwRateDivider;
                    Debug.Log($"Current Throw Rate: {ThrowRate}");
                    break;
            }
        }

        #endregion

        #region PRIVATE METHODS

        // Calling from animation events
        private void HoldCard()
        {
            for (int i = 0; i < throwCapacity; i++)
            {
                var holdCard = _player.GetCard();
                if (holdCard == null)
                {
                    Debug.Log("No card to throw!!");
                    return;
                }

                holdCard.transform.DOLocalRotate(Vector3.zero, 0.2f / throwRate).SetEase(Ease.Linear);
                holdCard.transform.DOLocalMove(new Vector3(1.2f, 0f, 0f), 0.2f / throwRate)
                    .SetEase(Ease.Linear).OnComplete(() =>
                    {
                        holdCard.transform.parent = cardThrower;
                        holdCard.transform.DOKill();
                    });

                _holdingCards.Add(holdCard);
            }

            if (_holdingCards.Count > 0)
                _player.TriggerLeftHandAnimation();
        }


        // Calling from animation events
        private void Throw()
        {
            if (_holdingCards.Count == 0) return;

            var angles = _angleOffsets[_holdingCards.Count - 1];

            for (int i = 0; i < angles.Length; i++)
            {
                var angle = angles[i];
                var rotation = Quaternion.Euler(0, angle, 0);

                _holdingCards[i].transform.rotation = rotation;
                _holdingCards[i].Release(throwRange);
            }

            _holdingCards.Clear();
        }


        private void InitAngles()
        {
            _angleOffsets.Add(new[] { 0f });
            _angleOffsets.Add(new[] { 10f, -10f });
            _angleOffsets.Add(new[] { 0f, 10f, -10f });
            _angleOffsets.Add(new[] { 5f, -5f, 15f, -15f });
            _angleOffsets.Add(new[] { 0f, 10f, -10f, 20f, -20f });
        }

        #endregion
    }
}