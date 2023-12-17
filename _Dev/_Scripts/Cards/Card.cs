using System;
using DG.Tweening;
using Game.Player;
using NaughtyAttributes;
using UnityEngine;

namespace Game.Cards
{
    public class Card : BaseCard
    {
        [SerializeField] protected AnimationCurve returnSpeedCurve;
        [SerializeField] protected AnimationCurve headTurnSpeedCurve;
        [SerializeField] protected AnimationCurve forwardSpeedCurve;
        [SerializeField] [ShowAssetPreview] protected Material[] cardMaterials;
        [SerializeField] protected Color[] cardTextColors;

        public int FlagPower => _flagPower;
        
        protected PlayerController _player;
        protected Action _cardBehaviour;
        protected Vector3 _startPos;
        protected Vector3 _returnPos;
        protected float _startTime;
        protected float _travelDistance;
        protected int _flagPower;
        protected bool _isReturning;


        #region UNITY EVENTS

        protected override void Update()
        {
            if (!_isReturning) return;

            _cardBehaviour?.Invoke();
        }

        #endregion

        #region PUBLIC METHODS

        public override void Init(PlayerController player)
        {
            _player = player;
        }


        public override void SetPower(int power)
        {
            this.power = power;

            UpdateTexts(true);
            UpdateMaterial();
        }


        public override void UpdateMaterial(Material mat = null)
        {
            if (mat == null)
                _meshRenderer.material = cardMaterials[power - 1];
            else
                _meshRenderer.material = mat;
        }


        public override void Release(float range)
        {
            this.range = range;
            _flagPower = power;
            _collider.enabled = true;
            transform.parent = null;

            var targetPos = transform.position + (transform.forward * range);
            transform.DOMove(targetPos, (range / returnTimeFactor)).SetEase(forwardSpeedCurve).SetId(this)
                .OnComplete(() => { ProcessHeadTurn(); });

            transform.rotation = Quaternion.Euler(45f, 0f, 0f);
            transform.DORotate(new Vector3(90f, 0f, 0f), 0.15f).SetEase(Ease.Linear);
        }


        public override void ReturnEarly()
        {
            if (_isReturning) return;

            _collider.enabled = false;
            transform.DOKill();
            ProcessReturn();
        }


        public override void ReturnFromFlag()
        {
            _flagPower--;

            if (_flagPower <= 0)
                ReturnEarly();
        }


        public override void UpdateTexts(bool isEnabled)
        {
            foreach (var text in cornerTexts)
            {
                text.text = $"+{power}";
                text.gameObject.SetActive(isEnabled);
            }

            centerText.text = $"+{power}";
            centerText.gameObject.SetActive(isEnabled);
            centerText.color = cardTextColors[power - 1];
        }

        #endregion

        #region PROTECTED METHODS

        protected virtual void ProcessHeadTurn()
        {
            _isReturning = true;
            _startPos = transform.position;
            _returnPos = new Vector3(_startPos.x - headTurnRadius, _startPos.y, _startPos.z);
            _startTime = Time.time;
            _cardBehaviour = Behaviour_HeadTurn;
        }


        protected virtual void ProcessReturn()
        {
            _isReturning = true;
            _startPos = transform.position;
            _returnPos = _player.GetAvailablePos();
            _travelDistance = Vector3.Distance(_startPos, _returnPos);
            _startTime = Time.time;
            _cardBehaviour = Behaviour_Return;
        }


        protected virtual void Behaviour_Return()
        {
            _returnPos = _player.GetAvailablePos();

            // Calculate center between start and target
            var center = (_startPos + _returnPos) / 2;

            // Subtract angle sharpness to make it curved
            center -= Vector3.left * returnAngleSharpness;

            // Calculate data for Slerp (see Vector3.Slerp documentation)
            var startRelCenter = _startPos - center;
            var finishRelCenter = _returnPos - center;

            // Evaluate completion using Animation Curve
            var fracComplete =
                returnSpeedCurve.Evaluate((Time.time - _startTime) / (_travelDistance / returnTimeFactor));

            // Finally set curved position with Slerp
            transform.position = Vector3.Slerp(startRelCenter, finishRelCenter, fracComplete);
            transform.position += center;

            // Get it back to card holder
            if (Vector3.Distance(_returnPos, transform.position) < 0.1f)
            {
                _isReturning = false;
                _player.ReturnCard(this);
            }
        }


        protected virtual void Behaviour_HeadTurn()
        {
            // Calculate center between start and target
            var center = (_startPos + _returnPos) / 2;

            // Subtract angle sharpness to make it curved
            center -= Vector3.forward * headTurnAngleSharpness;

            // Calculate data for Slerp (see Vector3.Slerp documentation)
            var startRelCenter = _startPos - center;
            var finishRelCenter = _returnPos - center;

            // Evaluate completion using Animation Curve
            var fracComplete =
                headTurnSpeedCurve.Evaluate((Time.time - _startTime) / (headTurnRadius * 2f / returnTimeFactor));

            // Finally set curved position with Slerp
            transform.position = Vector3.Slerp(startRelCenter, finishRelCenter, fracComplete);
            transform.position += center;

            if (Vector3.Distance(_returnPos, transform.position) < 0.1f)
            {
                ProcessReturn();
            }
        }

        #endregion
    }
}