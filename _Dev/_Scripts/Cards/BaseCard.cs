using Game.Player;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace Game.Cards
{
    public abstract class BaseCard : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] protected float returnAngleSharpness;
        [SerializeField] protected float headTurnAngleSharpness;
        [SerializeField] protected float headTurnRadius;
        [SerializeField] protected float returnTimeFactor;

        [Space] [Header("Components")]
        [SerializeField] protected TextMeshProUGUI centerText;
        [SerializeField] protected TextMeshProUGUI[] cornerTexts;

        [Space] [Header("Debug")]
        [SerializeField] [ReadOnly] protected float range;
        [SerializeField] [ReadOnly] protected int power;

        public int Power => power;

        protected MeshRenderer _meshRenderer;
        protected Collider _collider;


        protected void Awake()
        {
            _collider = GetComponent<Collider>();
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        protected abstract void Update();

        public abstract void Init(PlayerController player);

        public abstract void SetPower(int power);

        public abstract void UpdateMaterial(Material mat = null);

        public abstract void UpdateTexts(bool isEnabled);

        public abstract void Release(float range);

        public abstract void ReturnEarly();
        
        public abstract void ReturnFromFlag();
    }
}