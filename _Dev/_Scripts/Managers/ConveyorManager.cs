using DG.Tweening;
using Game.Gates;
using UnityEngine;

namespace Game.Managers
{
    public class ConveyorManager : StaticInstance<ConveyorManager>
    {
        [Header("Settings")]
        [SerializeField] private float conveyorSpeed;
        [SerializeField] private CapacityUpgradeGate[] capacityGates;

        private int _index;


        #region PUBLIC METHODS

        public void MoveToNextGate() => _index++;

        
        public void SendToUpgradeGate(GameObject item, float progressAmount)
        {
            var t = item.transform;
            var jumpPos = new Vector3(capacityGates[_index].EndPos.x,
                capacityGates[_index].EndPos.y, t.position.z);

            // Jump item to conveyor
            t.DOJump(jumpPos, 1f, 1, 0.5f)
                .OnComplete(() =>
                {
                    // Then move item to upgrade point
                    t.DOMove(capacityGates[_index].EndPos, conveyorSpeed)
                        .SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(() =>
                        {
                            capacityGates[_index].UpdateGate(progressAmount);
                            item.SetActive(false);
                        });
                });
        }

        #endregion
    }
}