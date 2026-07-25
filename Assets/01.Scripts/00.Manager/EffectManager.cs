using System;
using System.Collections.Generic;
using UnityEngine;

namespace _01.Scripts._00.Manager
{
    public enum EffectType
    {
        Hit,
    }

    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance;
        
        [Serializable]
        public struct EffectData
        {
            public EffectType effectType;
            public GameObject effectPrefab;
            public int initialPoolSize;
        }

        [Header("이펙트 등록 리스트")]
        [SerializeField] private List<EffectData> effectDataList;
        
        private Dictionary<EffectType, Queue<GameObject>> _poolDictionary = new();
        private Dictionary<EffectType, GameObject> _prefabDictionary = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            InitializePool();
        }
        
        private void InitializePool()
        {
            foreach (EffectData data in effectDataList)
            {
                if (data.effectPrefab == null)
                {
                    continue;
                }
                
                _prefabDictionary[data.effectType] = data.effectPrefab;
                _poolDictionary[data.effectType] = new Queue<GameObject>();
                
                int size = data.initialPoolSize > 0 ? data.initialPoolSize : 5;
                for (int i = 0; i < size; i++)
                {
                    AddEffect(data.effectType);
                }
            }
        }
        
        private GameObject AddEffect(EffectType type)
        {
            if (!_prefabDictionary.TryGetValue(type, out GameObject prefab))
            {
                return null;
            }

            GameObject go = Instantiate(prefab, transform);
            go.SetActive(false);
            
            _poolDictionary[type].Enqueue(go);
            return go;
        }

        /// <summary>
        /// 이펙트 재생용 외부 호출 함수
        /// </summary>
        public void PlayEffect(EffectType type, Vector3 position, Color? customColor = null)
        {
            if (!_poolDictionary.TryGetValue(type, out var poolQueue))
            {
                return;
            }

            GameObject targetGo;
            
            if (poolQueue.Count > 0 && !poolQueue.Peek().activeInHierarchy)
            {
                targetGo = poolQueue.Dequeue();
            }
            else
            {
                targetGo = AddEffect(type);
                if (targetGo != null)
                {
                    targetGo = poolQueue.Dequeue();
                }
            }

            if (targetGo == null) return;
            
            targetGo.transform.position = position;
            targetGo.SetActive(true);
            
            if (targetGo.TryGetComponent<ParticleSystem>(out var particle))
            {
                if (customColor.HasValue)
                {
                    var mainModule = particle.main;
                    mainModule.startColor = new ParticleSystem.MinMaxGradient(customColor.Value);
                }
                
                particle.Play();
            }
            
            poolQueue.Enqueue(targetGo);
        }
    }
}