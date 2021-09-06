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
        Qna, ChapterInfo
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
                        .Form("stage", $"Stage {(int) LoadingManager.Instance.chapterType + 1}-{(int) LoadingManager.Instance.stageType + 1}");
                    break;
                case RequestUrlType.ChapterInfo:
                    builder
                        .Method(HttpMethod.Get);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return builder.Build();
        }
    }


    public static class RequestUrlUtils
    {
        // TODO (url) : 전체 Chapter Count 어디다 저장해서 get url 에 넣어줄건지.. 고민
        private static readonly string qnaUrl = "https://script.google.com/macros/s/AKfycbzU3qAqk9M6qw677feQ8wQuTWCId0-_2Vk7Tr4eI_Xn92NDsYnyPCPGSl-6K5Hqf_lIrQ/exec";
        private static readonly string chapterUrl = "https://script.google.com/macros/s/AKfycbxEoGzqz8A5PKHLQkWaWEogxuHubLsoUaTkdWKoUqlTs3PvNbopTWXhOtGcorw54S-k/exec?chapterCount=6";
        public static string TypeToUrl(RequestUrlType type)
        {
            switch (type)
            {
                case RequestUrlType.Qna:
                    return qnaUrl;
                case RequestUrlType.ChapterInfo:
                    return chapterUrl;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}