using System;
using Domain;
using Manager.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace Manager
{
    public class GlobalDataManager : MonoBehaviour
    {
        private static GlobalDataManager instance = null;

        public static GlobalDataManager Instance => instance ?? (instance = new GlobalDataManager());

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
    }
}