﻿using System;
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

        [SerializeField] public int status;
        [SerializeField] public int code;
        [SerializeField] public int subCode;
        [SerializeField] public string message;

        #endregion

        #region Getter

        public ErrorCode GetErrorCode()
        {
            return ErrorCode.GetValues().First(errorCode => errorCode.code == code && errorCode.subCode == subCode);
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

        public readonly int status;
        public readonly int code;
        public readonly int subCode;
        public readonly string message;

        #endregion

        #region Const Variables

        public const int ERROR_CODE_QNA = 1;
        public const int ERROR_CODE_CHAPTER = 2;

        #endregion

        #region Static Variables

        // TODO(Error) : Error 상황 생각나면 ErrorCode 추가
        public static readonly ErrorCode QNA_NOT_FOUND = new ErrorCode(404, ERROR_CODE_QNA, 100, "");

        public static readonly ErrorCode CHAPTER_NOT_FOUND = new ErrorCode(404, ERROR_CODE_CHAPTER, 200, "Chapter Not Found");

        #endregion

        #region Getter

        public static IEnumerable<ErrorCode> GetValues()
        {
            yield return QNA_NOT_FOUND;
            yield return CHAPTER_NOT_FOUND;
        }

        #endregion

        #region Constructor

        private ErrorCode(int status, int code, int subCode, string message)
        {
            this.status = status;
            this.code = code;
            this.subCode = subCode;
            this.message = message;
        }

        #endregion
    }
}