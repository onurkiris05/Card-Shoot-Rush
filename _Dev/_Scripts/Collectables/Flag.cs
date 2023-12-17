using DG.Tweening;
using Game.Cards;
using Game.Managers;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
namespace Game.Collectables
{
    public class Flag : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int progressAmount;
        [SerializeField] private float blendEndValue;
        [SerializeField] private float blendDuration;

        [Space] [Header("Components")]
        [SerializeField] private SkinnedMeshRenderer flagRenderer;
        [SerializeField] private MeshRenderer foldedFlagRenderer;

        private FlagHandler _flagHandler;
        private DynamicBone _dynamicBone;
        private BoxCollider _collider;
        private Vector3 _initialPos;
        private bool _isTriggered;


        #region UNITY EVENTS

        private void Awake()
        {
            _flagHandler = GetComponentInParent<FlagHandler>();
            _dynamicBone = GetComponent<DynamicBone>();
            _collider = GetComponent<BoxCollider>();
            startScale = transform.localScale;
        }


        private void Start() => _initialPos = transform.position;

        private bool shaking;
        private Vector3 startScale;
        private IEnumerator ShakeFlag()
        {
            transform.DOScale(startScale * 1.1f, .1f);
            yield return new WaitForSeconds(.1f);
            transform.DOScale(startScale, .1f);
            yield return new WaitForSeconds(.1f);
            shaking = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered) return;

            if (other.TryGetComponent(out Card card))
            {
                if (card.FlagPower <= 0) return;
                GameObject slashParticle =  ObjectPooler.Instance.Spawn("Slash",null);
                slashParticle.transform.position = transform.position+new Vector3(0,1.5f,0);
                foreach(ParticleSystem ps in slashParticle.GetComponentsInChildren<ParticleSystem>())
                {
                    ps.Play();
                }
                _isTriggered = true;
                _flagHandler.ReleaseMe(this);
                if (!shaking)
                {
                    StartCoroutine(ShakeFlag());
                }
                SetCollider(false);
                ProcessTearing();
                card.ReturnFromFlag();
            }
        }

        #endregion

        #region PUBLIC METHODS

        public void SetMaterial(Material mat)
        {
            flagRenderer.material = mat;
            foldedFlagRenderer.material = mat;
        }


        public void SetCollider(bool state)
        {
            _collider.enabled = state;
        }

        #endregion

        #region PRIVATE METHODS

        private void ProcessTearing()
        {
            float tearBlendValue = 0f;
            float upperPieceBlendValue = 0f;
            float dynamicBlendValue = 1f;
            bool flagTeared = false;

            // Get flag a bit closer
            var closerPos = new Vector3(_initialPos.x, _initialPos.y, _initialPos.z - 0.3f);
            transform.DOMove(closerPos, 0.3f).SetEase(Ease.Linear)
                .OnComplete(() => { transform.parent = null; });

            // Slowly show up folded flag model
            foldedFlagRenderer.transform.DOLocalMoveY(foldedFlagRenderer.transform.localPosition.y + 0.35f,
                blendDuration);

            // Turn off dynamic bone
            DOTween.To(() => dynamicBlendValue, x => dynamicBlendValue = x, 0f, 0.25f)
                .OnUpdate(() => { _dynamicBone.m_BlendWeight = dynamicBlendValue; });

            // Tear the flag
            DOTween.To(() => tearBlendValue, x => tearBlendValue = x, blendEndValue, blendDuration)
                .SetEase(Ease.InQuad).OnUpdate(() =>
                {
                    flagRenderer.SetBlendShapeWeight(0, tearBlendValue);

                    if (!flagTeared && tearBlendValue > blendEndValue / 1.75f)
                    {
                        flagTeared = true;

                        // Move down upper piece
                        DOTween.To(() => upperPieceBlendValue, x => upperPieceBlendValue = x,
                                blendEndValue / 2f, blendDuration)
                            .OnUpdate(() => { flagRenderer.SetBlendShapeWeight(1, upperPieceBlendValue); })
                            .OnComplete(() =>
                            {
                                // Finally send folded flag model to conveyor
                                flagRenderer.enabled = false;
                                transform.DOScale(transform.localScale * 0.8f, blendDuration);
                                ConveyorManager.Instance.SendToUpgradeGate(gameObject, progressAmount);
                            });
                    }
                });
        }

        #endregion
    }
}