using Game.Managers;
using UnityEngine;

namespace Game.Player
{
    public class MovementHandler : MonoBehaviour
    {
        [Header("Move Settings")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float turnSpeed;
        [SerializeField] private float sensitivity;
        [SerializeField] private Vector3 moveDirection;

        [Space] [Header("Rotation Settings")]
        [SerializeField] private bool isRotationEnabled;
        [SerializeField] private float slippageSensitivity = 300f;
        [SerializeField] private float slippageCorrectionSpeed = 0.003f;
        [SerializeField] private float rotationLimit = 45f;
        [SerializeField] private Vector3 rotationAxis;

        [Space] [Header("Components")]
        [SerializeField] private Transform playerObject;
        [SerializeField] private Transform minBound;
        [SerializeField] private Transform maxBound;

        private GameState _gameState = GameState.Start;
        private PlayerController _player;
        private Vector2 _firstMousePos;
        private Vector2 _secondMousePos;
        private float _speed;
        private float _xPosition;
        private float _xSlippage;
        private bool _clicked;

        #region UNITY EVENTS

        private void OnEnable()
        {
            GameManager.OnAfterStateChanged += SetState;
        }

        private void OnDisable()
        {
            GameManager.OnAfterStateChanged -= SetState;
        }

        private void Update()
        {
            if (_player.IsTrapped) return;

            if (_gameState == GameState.Running)
            {
                ForwardMovement();
                HorizontalMovement();
            }
        }

        #endregion

        #region PUBLIC METHODS

        public void Init(PlayerController player) => _player = player;

        #endregion

        #region PRIVATE METHODS

        private void ForwardMovement()
        {
            transform.Translate(moveDirection * (_speed * Time.deltaTime), Space.World);
        }

        private void HorizontalMovement()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Get mouse first position
                _firstMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                _xPosition = transform.position.x;
                _clicked = true;
            }
            else if (Input.GetMouseButton(0) && _clicked)
            {
                // Get mouse second position and calculate difference
                _secondMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                var difference = (_secondMousePos - _firstMousePos) * sensitivity;

                // Add and clamp X position
                _xPosition += difference.x;
                _xPosition = Mathf.Clamp(_xPosition, minBound.position.x, maxBound.position.x);

                // Define target pos
                var targetPos = new Vector3(_xPosition, transform.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, targetPos, turnSpeed * Time.deltaTime);
                // transform.Translate(targetPos - transform.position, Space.World);  //For harder input feeling


                // Give smooth rotation to gun model according to swerve direction
                if (isRotationEnabled)
                {
                    _xSlippage += slippageSensitivity * (_secondMousePos - _firstMousePos).x;
                    _xSlippage = Mathf.Clamp(_xSlippage, -rotationLimit, rotationLimit);
                    playerObject.eulerAngles = rotationAxis * _xSlippage;
                }

                // Set first mouse position to second mouse position
                _firstMousePos = _secondMousePos;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _clicked = false;
            }

            // Smoothly correct rotation of gun model
            if (!isRotationEnabled) return;
            _xSlippage = Mathf.Lerp(_xSlippage, 0, slippageCorrectionSpeed);
            playerObject.eulerAngles = rotationAxis * _xSlippage;
        }

        private void SetState(GameState state)
        {
            // For example change running speed according to game state
            _gameState = state;
            _speed = moveSpeed;
            // _speed = state == GameState.MinigameRunning ? minigameSpeed : moveSpeed;
        }

        #endregion
    }
}