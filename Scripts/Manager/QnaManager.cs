﻿using System;
using System.Collections;
using Domain;
using Domain.Network;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Manager
{
    public class QnaManager: MonoBehaviour
    {
        #region Public Variables

        public Response response;
        [HideInInspector] public bool isLoaded = false;

        #endregion

        #region Const Variable

        private const string baseURL =
            "https://script.google.com/macros/s/AKfycbxfW0c3rzK1PEu5cHGIkUuIk0R1YjTsRIjmLuip1jO5-0MP77WJFNmMmuIQ1MR2h9qNlA/exec";

        #endregion

        #region Static Variables

        private static QnaManager instance = null;

        public static QnaManager Instance => instance;

        #endregion

        #region Event Method

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            
            DontDestroyOnLoad(gameObject);
        }

        #endregion

        #region Public Method

        public void GetQnaData(ChapterType chapterType, StageType stageType)
        {
            WWWForm form = new WWWForm();
            form.AddField("stage", $"Stage {(int) chapterType}-{(int) stageType + 1}");
            
            StartCoroutine(Post(form, baseURL));
        }

        #endregion

        #region Private Methods

        private IEnumerator Post(WWWForm form, string url)
        {
            using var www = UnityWebRequest.Post(url, form);
            yield return www.SendWebRequest();

            if (www.isDone)
            {
                ProcessResponse(www.downloadHandler.text);
            }
            else
            {
                // TODO("If data cannot receive. how can handle to user")
                Debug.Log("No response from google spreed sheet");
            }
        }

        private void ProcessResponse(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            response = JsonConvert.DeserializeObject<Response>(json);
            isLoaded = true;
        }

        #endregion
    }
}