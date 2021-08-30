using System;
using Domain;
using Manager.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace Manager
{
    public class GlobalDataManager : MonoBehaviour
    {
        #region Static Variables

        private static GlobalDataManager instance = null;
        public static GlobalDataManager Instance => instance;

        #endregion

        #region Event Method

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            DontDestroyOnLoad(gameObject);
        }

        #endregion

        #region Public Methods

        public void Set(GlobalDataKey key, object value)
        {
            var convert = JsonConvert.SerializeObject(value);
            PlayerPrefs.SetString(key.key, convert);
        }

        public T Get<T>(GlobalDataKey key)
        {
            var value = PlayerPrefs.GetString(key.key);
            return JsonConvert.DeserializeObject<T>(value);
        }

        public bool HasKey(GlobalDataKey key)
        {
            return PlayerPrefs.HasKey(key.key);
        }

        #endregion
    }
}