using System;
using System.Collections.Generic;
using Event;
using Http;
using Manager;
using Newtonsoft.Json;
using UnityEngine;

namespace Domain.Network.Response
{
    public static class ResponseKeyManager {
        #region Static Variables
        
        public static readonly string KEY_QNA = "qna";
        public static readonly string KEY_CHAPTER_INFO = "chapterInfo";
        public static readonly string KEY_TUTORIAL_QNA = "tutorialQna";
        
        #endregion
    }

    [Serializable]
    public class Response
    {
        #region Public Variables

        public string code;
        public bool status;
        public string timestamp;
        public Dictionary<string, object> data;
        public string message;
        // public string error;

        #endregion

        #region Private Variable

        private ResponseOccurredEventArgs responseOccurredEventArgs;
        
        #endregion

        #region Getter

        public HashSet<DeserializeType> GetDeserializeTypes()
        {
            return responseOccurredEventArgs.types;
        }

        public HttpStatus GetHttpCode()
        {
            return HttpStatusExtensions.StringToStatus(code.ToLower());
        }

        #endregion

        #region Setter

        public void AddDeserializeTypes(DeserializeType type)
        {
            responseOccurredEventArgs.types.Add(type);
        }

        #endregion
        
        #region Events

        public static event EventHandler<ResponseOccurredEventArgs> ResponseOccurredEvent;

        #endregion

        #region Constructors

        public Response()
        {
            code = "";
            status = false;
            timestamp = "";
            data = new Dictionary<string, object>();
            message = "";
        }

        public Response(string code, bool status, string timestamp, Dictionary<string ,object> data, string message)
        {
            this.code = code;
            this.status = status;
            this.timestamp = timestamp;
            this.data = data;
            this.message = message;
        }

        #endregion
        
        #region Public Method

        public static Response JsonToResponse(string json)
        {
            var res = JsonConvert.DeserializeObject<Response>(json);
            res.InitEventArgs();
            return res;
        }

        public void EmitResponseOccurredEvent()
        {
            if (ResponseOccurredEvent == null) return;
            foreach (var invocation in ResponseOccurredEvent.GetInvocationList())
            {
                invocation?.DynamicInvoke(this, responseOccurredEventArgs);
            }
        }
        
        #endregion

        #region Private Method

        private void InitEventArgs()
        {
            responseOccurredEventArgs = new ResponseOccurredEventArgs();
        }

        #endregion
    }

    public static class ResponseExtension
    {
        private static bool HasKeyAndNotNull(this Response response, string key) =>
            response.data.ContainsKey(key) && response.data[key] != null;

        private static T Deserialize<T>(this Response response, string key)
        {
            var value = response.data[key];
            return JsonConvert.DeserializeObject<T>(value.ToString());
        }

        private static void DeserializeQna(this Response response)
        {
            var key = ResponseKeyManager.KEY_QNA;
            if (!response.HasKeyAndNotNull(key)) return;
            var qna = response.Deserialize<List<Qna>>(key);
            QnaManager.Instance.Setup(qna);
            response.AddDeserializeTypes(DeserializeType.Qna);
        }
        private static void DeserializeTutorialQna(this Response response)
        {
            var key = ResponseKeyManager.KEY_TUTORIAL_QNA;
            if (!response.HasKeyAndNotNull(key)) return;
            var qna = response.Deserialize<List<Qna>>(key);
            QnaManager.Instance.Setup(qna, true);
            response.AddDeserializeTypes(DeserializeType.Tutorial);
        }

        private static void DeserializeStageInfo(this Response response)
        {
            var key = ResponseKeyManager.KEY_CHAPTER_INFO;
            if (!response.HasKeyAndNotNull(key)) return;
            var chapter = response.Deserialize<List<ChapterInfo>>(key);
            ChapterManager.Instance.SetUp(chapter);
            response.AddDeserializeTypes(DeserializeType.ChapterInfo);
        }

        public static void DeserializeAll(this Response response, bool deserialize = true)
        {
            // NetworkManager.cs의 HandleResponse에서 DeserializeAll()을 할것인지 아니면
            // Data를 사용하는 script내에서 추가적인 처리를 한 다음에 DeserializeAll()을 할것인지 여부를 결정하기 위한 bool값
            if (!deserialize) return;
            response.DeserializeQna();
            response.DeserializeTutorialQna();
            response.DeserializeStageInfo();
            response.EmitResponseOccurredEvent();
        }
    }
}