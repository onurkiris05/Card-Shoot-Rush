using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Managers;
using NaughtyAttributes;
using UnityEngine;

namespace Game.Collectables
{
    public class Rope : BaseCollectable
    {
        [Header("Settings")]
        [SerializeField] private GameObject[] ropePieces;
        [SerializeField] [ReadOnly] private float strengthPerPiece;

        private RopeHandler _ropeHandler;


        #region UNITY EVENTS

        protected override void OnTriggerEnter(Collider other)
        {
            // Maybe fill later
        }

        #endregion

        #region PUBLIC METHODS

        public void Init(float strengthPerRope)
        {
            strengthPerPiece = strengthPerRope / ropePieces.Length;
        }


        public void BreakIntoPieces(Vector3 minPos, Vector3 maxPos)
        {
            StartCoroutine(ProcessBreakIntoPieces(minPos, maxPos));
        }


        public void PlayScaleUpEffect()
        {
            StartCoroutine(ProcessScaleUpEffect());
        }

        #endregion

        #region PRIVATE METHODS

        private IEnumerator ProcessScaleUpEffect()
        {
            for (int i = 0; i < ropePieces.Length; i++)
            {
                ropePieces[i].transform.DOComplete();
                ropePieces[i].transform.DOScale(ropePieces[i].transform.localScale * 1.4f, 0.1f)
                    .SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
                yield return Helpers.BetterWaitForSeconds(0.05f);
            }
        }


        private IEnumerator ProcessBreakIntoPieces(Vector3 minPos, Vector3 maxPos)
        {
            var reversedPieces = ropePieces.Reverse();

            foreach (var piece in reversedPieces)
            {
                var jumpPos = Helpers.GenerateRandomVector3(minPos, maxPos);
                var rot = new Vector3(0f, Random.Range(0f, 90f), 0f);

                piece.transform.DOJump(jumpPos, 2f, 1, 0.5f);
                piece.transform.DORotate(rot, 0.5f);

                yield return Helpers.BetterWaitForSeconds(0.1f);
            }

            yield return Helpers.BetterWaitForSeconds(0.7f);

            StartCoroutine(ProcessSendToGate(reversedPieces.ToList()));
        }


        private IEnumerator ProcessSendToGate(List<GameObject> pieces)
        {
            foreach (var piece in pieces)
            {
                ConveyorManager.Instance.SendToUpgradeGate(piece, strengthPerPiece);

                yield return Helpers.BetterWaitForSeconds(0.1f);
            }
        }

        #endregion
    }
}