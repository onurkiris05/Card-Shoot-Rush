using System.Collections.Generic;
using System.Linq;
using Game.Cards;
using NaughtyAttributes;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Game.Collectables
{
    public class RopeHandler : MonoBehaviour
    {
        [Header("Generator Settings")]
        [SerializeField] private int rowCount;
        [SerializeField] private float rowSpacing;
        [SerializeField] private int columnCount;
        [SerializeField] private float columnSpacing;
        [SerializeField] private float strengthFactor;

        [Space] [Header("Components")]
        [SerializeField] private Rope ropePrefab;
        [SerializeField] private Transform minJump;
        [SerializeField] private Transform maxJump;
        [SerializeField] private TextMeshPro headerText;

        [Space] [Header("Debug")]
        [SerializeField] [ReadOnly] private int currentStrength;
        [SerializeField] [ReadOnly] private float currentRowStrength;
        [SerializeField] [ReadOnly] private float strengthPerRow;
        [SerializeField] [ReadOnly] private float strengthPerRope;

        private List<Rope> _ropes = new();
        private BoxCollider _collider;


        #region UNITY EVENTS

        private void Awake()
        {
            _ropes = GetComponentsInChildren<Rope>().ToList();
            _collider = GetComponent<BoxCollider>();
        }


        private void Start()
        {
            Init();
            InitRopes();
            CalculateColliderBounds();
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out BaseCard card))
            {
                // Process hit VFX
                var vfxPos = other.ClosestPointOnBounds(transform.position);
                VFXSpawner.Instance.PlayVFX("CardHit", vfxPos);

                TakeDamage(card.Power);
                card.ReturnEarly();

                foreach (var rope in _ropes)
                {
                    rope.PlayScaleUpEffect();
                }
            }
        }

        #endregion

        #region PRIVATE METHODS

        private void Init()
        {
            currentStrength = (int)((rowCount * columnCount) * strengthFactor);
            strengthPerRow = (float)currentStrength / rowCount;
            currentRowStrength = strengthPerRow;
            strengthPerRope = strengthPerRow / columnCount;

            UpdateHeaderText();
        }


        private void InitRopes()
        {
            foreach (var rope in _ropes)
                rope.Init(strengthPerRope);
        }


        private void UpdateHeaderText()
        {
            headerText.text = $"{currentStrength}";
        }


        private void TakeDamage(int amount)
        {
            currentStrength -= amount;
            UpdateHeaderText();

            currentRowStrength -= amount;
            if (currentRowStrength <= 0)
            {
                currentRowStrength += strengthPerRow;
                ReleaseLeadingRow();
            }
        }


        private void ReleaseLeadingRow()
        {
            var minZ = _ropes.Min(rope => rope.transform.position.z);
            var leadingRowRopes = _ropes.Where(rope => Mathf.Approximately(rope.transform.position.z, minZ)).ToList();

            foreach (var rope in leadingRowRopes)
            {
                rope.BreakIntoPieces(minJump.position, maxJump.position);
                _ropes.Remove(rope);
            }
        }


        private void CalculateColliderBounds()
        {
            if (_ropes.Count > 0)
            {
                // Initialize min and max bounds to the first rope's position
                var minBounds = _ropes[0].transform.position;
                var maxBounds = _ropes[0].transform.position;

                foreach (var rope in _ropes)
                {
                    // Update min and max bounds based on rope positions
                    minBounds = Vector3.Min(minBounds, rope.transform.position);
                    maxBounds = Vector3.Max(maxBounds, rope.transform.position);
                }

                // Calculate raw center and size for the collider
                var rawCenter = (minBounds + maxBounds) / 2f;
                var rawSize = maxBounds - minBounds;

                // Calculate center and size for the collider
                var center = new Vector3(rawCenter.x, 1.5f, rawCenter.z);
                var size = new Vector3(rawSize.x, 3f, rawSize.z);

                _collider.center = center - transform.position;
                _collider.size = size;
            }
        }

        #endregion

        #region EDITOR METHODS

#if UNITY_EDITOR

        [Button]
        private void CreateRopes()
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    var rope = (Rope)PrefabUtility.InstantiatePrefab(ropePrefab, transform);
                    rope.transform.position = transform.position + new Vector3(j * columnSpacing, 0f, i * rowSpacing);
                    _ropes.Add(rope);
                }
            }
        }

        [Button]
        private void DeleteRopes()
        {
            var ropes = GetComponentsInChildren<Rope>().ToList();

            foreach (var rope in ropes)
                DestroyImmediate(rope.gameObject);
        }
#endif

        #endregion
    }
}