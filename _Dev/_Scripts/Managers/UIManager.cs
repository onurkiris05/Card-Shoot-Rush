using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.Managers
{
    public class UIManager : StaticInstance<UIManager>
    {
        [Header("Canvases")]
        [SerializeField] private GameObject canvasStartGame;
        [SerializeField] private GameObject canvasGameUI;
        [SerializeField] private GameObject canvasGameEnd;

        [Header("Components")]
        [SerializeField] private TextMeshProUGUI walletText;
        [SerializeField] private TextMeshProUGUI levelText;

        public RectTransform WalletTextUI => walletText.rectTransform;

        #region UNITY EVENTS

        private void OnEnable()
        {
            GameManager.OnAfterStateChanged += AdjustCanvases;
        }

        private void OnDisable()
        {
            GameManager.OnAfterStateChanged -= AdjustCanvases;
        }

        private void Start()
        {
            AdjustCanvases(GameManager.Instance.State);
            SetLevelUI();
        }

        #endregion

        #region PUBLIC METHODS

        public void SetWalletUI(int value, bool withEffect = false)
        {
            walletText.text = $"${value}";

            if (withEffect)
            {
                walletText.rectTransform.DOComplete();
                walletText.rectTransform.DOScale(Vector3.one * 1.3f, 0.3f).From();
            }
        }

        #endregion

        #region PRIVATE METHODS

        private void SetLevelUI()
        {
            levelText.text = $"Level {GameManager.Instance.GetLevel()}";
        }

        private void AdjustCanvases(GameState state)
        {
            canvasStartGame.SetActive(state == GameState.Start);
            canvasGameUI.SetActive(state == GameState.Start || state == GameState.Running);
            canvasGameEnd.SetActive(state == GameState.EndGame);
        }

        #endregion
    }
}