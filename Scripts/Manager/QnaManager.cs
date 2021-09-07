﻿using System;
using System.Collections;
using System.Collections.Generic;
using Domain;
using Domain.Network;
using Domain.Network.Response;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Manager
{
    [Serializable]
    public class QnaManager
    {
        #region Public Variables

        [SerializeField] public List<Qna> qna;

        #endregion

        #region Static Variables

        private static QnaManager instance = null;
        public static QnaManager Instance => instance ?? (instance = new QnaManager());

        #endregion
        

        #region Public Method

        public void Setup(List<Qna> content)
        {
            qna = content;
        }
        
        #endregion
    }
}