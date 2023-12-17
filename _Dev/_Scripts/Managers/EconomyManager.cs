using UnityEngine;

namespace Game.Managers
{
    public class EconomyManager : StaticInstance<EconomyManager>
    {
        [Header("Settings")]
        [SerializeField] private int currentWallet;

        #region ENCAPSULATIONS

        public int CurrentWallet
        {
            get => currentWallet;
            private set
            {
                currentWallet = Mathf.Max(value, 0);
                UIManager.Instance.SetWalletUI(currentWallet, true);
            }
        }


        #endregion

        #region UNITY EVENTS

        private void OnEnable()
        {
            GameManager.OnLevelCompleted += SaveCurrentMoney;
        }

        private void OnDisable()
        {
            GameManager.OnLevelCompleted -= SaveCurrentMoney;
        }

        protected override void Awake()
        {
            base.Awake();

            CurrentWallet = PlayerPrefs.GetInt("Wallet", currentWallet);
        }

        #endregion

        #region PUBLIC METHODS

        public void AddMoney(int amount)
        {
            Debug.Log($"Setting wallet: {CurrentWallet} + {amount}");
            CurrentWallet += amount;
        }

        public bool SpendMoney(int amount)
        {
            if (amount > CurrentWallet)
            {
                Debug.LogError("Insufficient funds!");
                return false;
            }

            CurrentWallet -= amount;
            return true;
        }

        public bool CanAfford(int amount)
        {
            if (amount > CurrentWallet)
                return false;

            return true;
        }

        #endregion

        #region PRIVATE METHODS

        private void SaveCurrentMoney()
        {
            PlayerPrefs.SetInt("Wallet", CurrentWallet);
        }

        #endregion
    }
}