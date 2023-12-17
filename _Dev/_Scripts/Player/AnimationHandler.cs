using Game.Managers;
using UnityEngine;

namespace Game.Player
{
    public class AnimationHandler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator rightHandAnimator;
        [SerializeField] private Animator leftHandAnimator;

        private PlayerController _player;


        #region UNITY EVENTS

        private void OnEnable()
        {
            GameManager.OnAfterStateChanged += SetAnimationState;
        }


        private void OnDisable()
        {
            GameManager.OnAfterStateChanged -= SetAnimationState;
        }

        #endregion

        #region PUBLIC METHODS

        public void Init(PlayerController player)
        {
            _player = player;
        }


        public void ProcessLeftHandAnimation()
        {
            leftHandAnimator.Play("Jiggle");
        }


        public void SetFireRate(float value)
        {
            rightHandAnimator.SetFloat("throwRate", value);
            leftHandAnimator.SetFloat("throwRate", value);
        }

        #endregion

        #region PRIVATE METHODS

        private void SetAnimationState(GameState gameState)
        {
            rightHandAnimator.SetBool("isThrowing", gameState == GameState.Running);
        }

        #endregion
    }
}