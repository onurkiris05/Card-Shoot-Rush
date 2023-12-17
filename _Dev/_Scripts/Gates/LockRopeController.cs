using Obi;
using UnityEngine;

namespace Game.Gates
{
    public class LockRopeController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private ObiRope[] ropes;

        private ObiSolver _solver;
        private int _currentIndex;
        private int _activeRopeCount;


        #region UNITY EVENTS

        private void OnEnable()
        {
            _solver = GetComponent<ObiSolver>();
        }

        #endregion

        #region PUBLIC METHODS

        public void Init(int ropeCount)
        {
            _activeRopeCount = ropeCount;

            for (var i = 0; i < _activeRopeCount; i++)
            {
                ropes[i].gameObject.SetActive(true);
            }
        }


        public bool TearRope()
        {
            var ropeMiddle = ropes[_currentIndex].elements.Count / 2;
            ropes[_currentIndex].Tear(ropes[_currentIndex].elements[ropeMiddle]);
            ropes[_currentIndex].RebuildConstraintsFromElements();
            _currentIndex++;

            if (_currentIndex == _activeRopeCount) return false;

            return true;
        }


        public void KillRopes()
        {
            _solver.actors.Clear();
        }

        #endregion
    }
}