using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Domain.Network.Response
{
    [Serializable]
    public class ErrorResponse
    {
        #region Public Variables

        [SerializeField] public int code;
        [SerializeField] public bool status;
        [SerializeField] public int index;
        [SerializeField] public int subCode;
        [SerializeField] public string message;

        #endregion

        #region Getter

        public ErrorCode GetErrorCode()
        {
            return ErrorCode.GetValues().First(errorCode => errorCode.index == index && errorCode.subCode == subCode);
        }

        #endregion

        #region Constructor

        public static ErrorResponse JsonToErrorResponse(string json)
        {
            var response = JsonConvert.DeserializeObject<ErrorResponse>(json);
            return response;
        }

        #endregion
    }

    public class ErrorCode
    {
        #region Readonly Variables

        public int code;
        public bool status;
        public int index;
        public int subCode;
        public string message;
        
        #endregion

        #region Const Variables

        public const int ERROR_INDEX_QNA = 1;
        public const int ERROR_INDEX_CHAPTER = 2;

        #endregion

        #region Static Variables

        // TODO(Error) : Error 상황 생각나면 ErrorCode 추가
        public static readonly ErrorCode QNA_NOT_FOUND = new ErrorCode(404, false, ERROR_INDEX_QNA, 100, "Stage Not Found");

        public static readonly ErrorCode CHAPTER_NOT_FOUND = new ErrorCode(404, false, ERROR_INDEX_CHAPTER, 200, "Chapter Not Found");
        public static readonly ErrorCode STAGE_NOT_FOUND = new ErrorCode(404, false, ERROR_INDEX_CHAPTER, 201, "Stage Not Found");

        #endregion

        #region Getter

        public static IEnumerable<ErrorCode> GetValues()
        {
            yield return QNA_NOT_FOUND;
            yield return CHAPTER_NOT_FOUND;
            yield return STAGE_NOT_FOUND;
        }

        #endregion

        #region Constructor

        private ErrorCode(int code, bool status, int index, int subCode, string message)
        {
            this.status = status;
            this.code = code;
            this.subCode = subCode;
            this.message = message;
        }

        #endregion
    }
}