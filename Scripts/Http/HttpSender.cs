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
        Qna, ChapterInfo, StageScore
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
                case RequestUrlType.StageScore:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return builder.Build();
        }

        public static UnityWebRequest Build(RequestUrlType type, int score)
        {
            var builder = HttpSender.Builder()
                .RequestUrl(type);

            if (type == RequestUrlType.StageScore)
            {
                builder
                    .Method(HttpMethod.Post)
                    .Form("chapter", $"{(int) LoadingManager.Instance.chapterType + 1}/{(int) LoadingManager.Instance.stageType + 1}/{score}");
            }
            else
            {
                return Build(type);
            }

            return builder.Build();
        }
    }


    public static class RequestUrlUtils
    {
        private static readonly string qnaUrl = "https://script.google.com/macros/s/AKfycbzAD_1OmkUJGdOivcysEs9GquKhnkfT9k0RA5Hemzu5CqhOMnZIq27YhKgkm7oJo3Qjjg/exec";
        private static readonly string chapterUrl = "https://script.google.com/macros/s/AKfycbzSdZLzrHXK5kKkj8NeaQ1TBUiQgdjxnWOMhV7l4w4zes_lS4iJeOEuW3vkMR9YXCmO/exec";
        public static string TypeToUrl(RequestUrlType type)
        {
            switch (type)
            {
                case RequestUrlType.Qna:
                    return qnaUrl;
                case RequestUrlType.ChapterInfo:
                    return chapterUrl + "?chapterCount=6";
                case RequestUrlType.StageScore:
                    return chapterUrl;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}