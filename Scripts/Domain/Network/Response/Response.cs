using System;
using System.Collections.Generic;
using Event;
using Http;
using Manager;
using Newtonsoft.Json;
using UnityEngine;

namespace Domain.Network.Response
{
    // TODO(?)
    [Serializable]
    public class Response
    {
        #region Public Variables

        public string title;
        public Qna[] content;

        #endregion
    }
    
    public static class ResponseKeyManager {
        #region Static Variables
        
        public static readonly string KEY_QNA = "qna";
        public static readonly string KEY_CHAPTER = "chapter";
        
        #endregion
    }

    [Serializable]
    public class PseudoResponse
    {
        #region Public Variables

        public string code;
        public bool status;
        public string timestamp;
        public Dictionary<string, object> data;
        public string message;
        public string error;

        #endregion

        #region Private Variable

        private ResponseOccurredEventArgs eventArgs;
        
        #endregion

        #region Getter

        public HashSet<DeserializeType> GetDeserializeTypes()
        {
            return eventArgs.types;
        }

        public HttpStatus GetHttpCode()
        {
            return HttpStatusExtensions.StringToStatus(code.ToLower());
        }

        #endregion

        #region Setter

        public void AddDeserializeTypes(DeserializeType type)
        {
            eventArgs.types.Add(type);
        }

        #endregion
        
        #region Events

        public static event EventHandler<ResponseOccurredEventArgs> ResponseOccurredEvent;

        #endregion

        #region Constructors

        public PseudoResponse()
        {
            code = "";
            status = false;
            timestamp = "";
            data = new Dictionary<string, object>();
            message = "";
        }

        public PseudoResponse(string code, bool status, string timestamp, Dictionary<string ,object> data, string message)
        {
            this.code = code;
            this.status = status;
            this.timestamp = timestamp;
            this.data = data;
            this.message = message;
        }

        #endregion
        
        #region Public Method

        public static PseudoResponse JsonToResponse(string json)
        {
            var res = JsonConvert.DeserializeObject<PseudoResponse>(json);
            res.InitEventArgs();
            return res;
        }

        public void EmitResponseOccurredEvent()
        {
            if (ResponseOccurredEvent == null) return;
            foreach (var invocation in ResponseOccurredEvent.GetInvocationList())
            {
                invocation?.DynamicInvoke(this, eventArgs);
            }
        }
        
        #endregion

        #region Private Method

        private void InitEventArgs()
        {
            eventArgs = new ResponseOccurredEventArgs();
        }

        #endregion
    }

    public static class ResponseExtension
    {
        private static bool HasKeyAndNotNull(this PseudoResponse response, string key) =>
            response.data.ContainsKey(key) && response.data[key] != null;

        private static T Deserialize<T>(this PseudoResponse response, string key)
        {
            var value = response.data[key];
            return JsonConvert.DeserializeObject<T>(value.ToString());
        }

        private static void DeserializeQna(this PseudoResponse response)
        {
            var key = ResponseKeyManager.KEY_QNA;
            if (!response.HasKeyAndNotNull(key)) return;
            var qna = response.Deserialize<Qna[]>(key);
            QnaManager.Instance.Setup(qna);
            response.AddDeserializeTypes(DeserializeType.Qna);
        }

        private static void DeserializeStage(this PseudoResponse response)
        {
            var key = ResponseKeyManager.KEY_CHAPTER;
            if (!response.HasKeyAndNotNull(key)) return;
            var chapter = response.Deserialize<PseudoChapter>(key);
            PseudoChapter.Instance.SetUp(chapter);
        }

        public static void DeserializeAll(this PseudoResponse response)
        {
            response.DeserializeQna();
            response.DeserializeStage();
            response.EmitResponseOccurredEvent();
        }
    }
}