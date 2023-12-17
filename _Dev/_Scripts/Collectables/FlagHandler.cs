using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Game.Collectables
{
    public class FlagHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int flagCount;
        [SerializeField] private float offsetZ;
        [SerializeField] private float forthAndBackSpeed;
        [SerializeField] private float forthAndBackDistance;

        [Space] [Header("Components")]
        [SerializeField] private TextMeshPro headerText;
        [SerializeField] private Flag[] flagPrefabs;
        [SerializeField] private Material[] flagMaterials;

        private List<Flag> _flags = new();


        #region UNITY EVENTS

        private void Start()
        {
            InitFlags();
            SetHeaderText();
            ProcessForthAndBack();
        }

        #endregion

        #region PUBLIC METHODS

        public void ReleaseMe(Flag releasedFlag)
        {
            _flags.Remove(releasedFlag);

            if (_flags.Count > 0)
                _flags[0].SetCollider(true);

            SetHeaderText();
        }

        #endregion

        #region PRIVATE METHODS

        private void InitFlags()
        {
            _flags = GetComponentsInChildren<Flag>().OrderBy(flag => flag.transform.position.z).ToList();
            _flags[0].SetCollider(true);
        }


        private void ProcessForthAndBack()
        {
            transform.DOMoveZ(transform.position.z + forthAndBackDistance, forthAndBackSpeed)
                .SetEase(Ease.InOutSine).SetSpeedBased().SetLoops(-1, LoopType.Yoyo);
        }


        private void SetHeaderText()
        {
            headerText.text = $"{_flags.Count}";

            var middleZ = ((_flags[^1].transform.position.z - _flags[0].transform.position.z) / 2f) +
                          _flags[0].transform.position.z;

            var midPos = new Vector3(headerText.transform.position.x, headerText.transform.position.y, middleZ);

            headerText.rectTransform.position = midPos;
        }

        #endregion

        #region EDITOR METHODS

#if UNITY_EDITOR

        [Button]
        private void CreateFlags()
        {
            DeleteRopes();

            for (int i = 0; i < flagCount; i++)
            {
                var flag = (Flag)PrefabUtility.InstantiatePrefab(flagPrefabs[i % flagPrefabs.Length], transform);
                flag.transform.position = transform.position + new Vector3(0f, 0f, i * offsetZ);
                flag.SetMaterial(flagMaterials[i % flagMaterials.Length]);
            }
        }

        [Button]
        private void DeleteRopes()
        {
            var flags = GetComponentsInChildren<Flag>().ToList();

            foreach (var flag in flags)
                DestroyImmediate(flag.gameObject);
        }
#endif

        #endregion
    }
}