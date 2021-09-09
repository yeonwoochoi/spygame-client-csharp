using System;
using System.Collections.Generic;
using Domain;
using Manager;
using UnityEngine;
using UnityEngine.Networking;

namespace Http
{
    public enum RequestUrlType
    {
        Qna, ChapterInfo, Tutorial
    }

    public enum HttpMethod
    {
        Get, Post
    }

    public static class HttpSender
    {
        public static HttpSenderBuilder Builder()
        {
            return new HttpSenderBuilder();
        }
    }

    public class HttpSenderBuilder
    {
        private string requestUrl = null;
        private WWWForm form;
        private HttpMethod method;

        public HttpSenderBuilder RequestUrl(RequestUrlType type)
        {
            requestUrl = RequestUrlUtils.TypeToUrl(type);
            return this;
        }

        public HttpSenderBuilder Form(string key, string value)
        {
            var wwwForm = new WWWForm();
            wwwForm.AddField(key, value);
            form = wwwForm;
            return this;
        }

        public HttpSenderBuilder Method(HttpMethod httpMethod)
        {
            method = httpMethod;
            return this;
        }

        public UnityWebRequest Build()
        {
            switch(method)
            {
                case HttpMethod.Get:
                    return UnityWebRequest.Get(requestUrl);
                case HttpMethod.Post:
                    return UnityWebRequest.Post(requestUrl, form);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public static class HttpFactory
    {
        public static UnityWebRequest Build(RequestUrlType type)
        {
            var builder = HttpSender.Builder()
                .RequestUrl(type);
            switch (type)
            {
                case RequestUrlType.Qna:
                    builder
                        .Method(HttpMethod.Post)
                        .Form("sheetName", $"Stage {(int) LoadingManager.Instance.chapterType + 1}-{(int) LoadingManager.Instance.stageType + 1}");
                    break;
                case RequestUrlType.ChapterInfo:
                    builder
                        .Method(HttpMethod.Get);
                    break;
                case RequestUrlType.Tutorial:
                    builder
                        .Method(HttpMethod.Post)
                        .Form("sheetName", "Tutorial");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return builder.Build();
        }
    }


    public static class RequestUrlUtils
    {
        private static readonly string qnaUrl = "https://script.google.com/macros/s/AKfycby0VAQJcrt8N3vmZ0UxpZ-v7MU1bh9aU54STgEv71b7xHt__fRv5gV2MpSGYQT0AsKrjg/exec";
        private static readonly string chapterUrl = "https://script.google.com/macros/s/AKfycbzZaoqioJ4watZMqCAS7WyeGdMWNBJVobKvT71PaNQeqF3yqs9QwShCWtDSyjY1lTQH/exec";
        public static string TypeToUrl(RequestUrlType type)
        {
            switch (type)
            {
                case RequestUrlType.Qna:
                    return qnaUrl;
                case RequestUrlType.ChapterInfo:
                    return chapterUrl + $"?chapterCount={StageManager.totalChapterCounts}";
                case RequestUrlType.Tutorial:
                    return qnaUrl;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}