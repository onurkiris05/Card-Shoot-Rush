using System.Collections.Generic;
using DG.Tweening;
using Game.Cards;
using Game.Gates;
using UnityEngine;

namespace Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        public bool IsTrapped => _isTrapped;

        private AnimationHandler _animationHandler;
        private CardHandler _cardHandler;
        private ThrowHandler _throwHandler;
        private MovementHandler _movementHandler;
        private bool _isTrapped;

        #region UNITY EVENTS

        private void Awake()
        {
            _animationHandler = GetComponent<AnimationHandler>();
            _throwHandler = GetComponent<ThrowHandler>();
            _cardHandler = GetComponent<CardHandler>();
            _movementHandler = GetComponent<MovementHandler>();

            _animationHandler.Init(this);
            _movementHandler.Init(this);
            _throwHandler.Init(this);
            _cardHandler.Init(this);
        }

        #endregion

        #region PUBLIC METHODS

        public void ProcessThrowCapacity(int throwCapacity)
        {
            _throwHandler.SetThrowCapacity(throwCapacity);
        }


        public BaseCard GetCard()
        {
            return _cardHandler.Card_Get();
        }


        public Vector3 GetAvailablePos()
        {
            return _cardHandler.GetAvailableSlot();
        }


        public void ReturnCard(BaseCard card)
        {
            _cardHandler.Card_Retrieve(card);
        }


        public void GenericGateReward(GenericGateData genericGateData)
        {
            _throwHandler.ProcessGenericGateReward(genericGateData);
        }


        public void CardGateReward(List<BaseCard> cards)
        {
            _cardHandler.ProcessCardGateReward(cards);
        }


        public void TriggerLeftHandAnimation()
        {
            _animationHandler.ProcessLeftHandAnimation();
        }


        public void OnThrowRateUpdate(float value)
        {
            _animationHandler.SetFireRate(value);
        }


        public void PushBack(float pushBackDistance, float pushBackDuration)
        {
            _isTrapped = true;

            transform.DOMoveZ(transform.position.z - pushBackDistance, pushBackDuration)
                .OnComplete(() => { _isTrapped = false; });
        }

        #endregion
    }
}