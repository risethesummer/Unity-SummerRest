﻿using System.Collections;
using System.Net;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.Pool;
using SummerRest.Runtime.Request;
using SummerRest.Utilities.Extensions;
using SummerRest.Utilities.RequestComponents;
using UnityEngine.Networking;
using UnityEngine.Pool;

namespace SummerRest.Runtime.RequestAdaptor
{
    internal abstract class UnityWebRequestAdaptor<TSelf, TResponse> :
        IWebRequestAdaptor<TResponse>,
        IPoolable<TSelf, UnityWebRequest> where TSelf : UnityWebRequestAdaptor<TSelf, TResponse>, new()
    {
        protected UnityWebRequest WebRequest { get; private set; }
        public virtual string RawResponse => null;
        public TResponse ResponseData { get; private set; }
        public IObjectPool<TSelf> Pool { get; set; }

        public static TSelf Create(UnityWebRequest webRequest)
        {
            return BasePool<TSelf, UnityWebRequest>.Create(webRequest);
        }

        public string Url
        {
            get => WebRequest.url;
            set => WebRequest.url = value;
        }

        public void Init(UnityWebRequest data)
        {
            WebRequest = data;
        }

        public void SetHeader(string key, string value)
        {
            WebRequest.SetRequestHeader(key, value);
        }

        public bool IsError(out string error)
        {
            if (WebRequest.result == UnityWebRequest.Result.Success)
            {
                error = null;
                return false;
            }

            error = $"Error {WebRequest.result}: {WebRequest.error}";
            return true;
        }

        public string GetHeader(string key) => WebRequest.GetRequestHeader(key);

        public HttpMethod Method
        {
            get => HttpMethodExtensions.UnityHttpMethod(WebRequest.method);
            set => WebRequest.method = value.ToUnityHttpMethod();
        }

        public int RedirectLimit
        {
            get => WebRequest.redirectLimit;
            set => WebRequest.redirectLimit = value;
        }

        public int TimeoutSeconds
        {
            get => WebRequest.timeout;
            set => WebRequest.timeout = value;
        }

        private ContentType? _contentType = IContentTypeParser.Current.DefaultContentType;

        public ContentType? ContentType
        {
            get
            {
                if (WebRequest.uploadHandler is null)
                    return null;
                return _contentType;
            }
            set
            {
                if (WebRequest.uploadHandler is null)
                    return;
                _contentType = value;
                _contentType ??= IContentTypeParser.Current.DefaultContentType;
                WebRequest.uploadHandler.contentType = _contentType.Value.FormedContentType;
            }
        }

        internal abstract TResponse BuildResponse();

        public WebResponse<TResponse> WebResponse
            => new(WebRequest,
                (HttpStatusCode)WebRequest.responseCode,
                IContentTypeParser.Current.ParseContentTypeFromHeader(
                    WebRequest.GetResponseHeader(IContentTypeParser.Current.ContentTypeHeaderKey)),
                WebRequest.GetResponseHeaders(),
                WebRequest.error,
                RawResponse,
                ResponseData
            );

        public virtual IEnumerator RequestInstruction
        {
            get
            {
#if UNITY_EDITOR
                WebRequest.SendWebRequest();
                while (!WebRequest.isDone)
                {
                    yield return null;
                }
#else
                yield return WebRequest.SendWebRequest();
#endif
                if (WebRequest.result == UnityWebRequest.Result.Success)
                    ResponseData = BuildResponse();
            }
        }


        public virtual void Dispose()
        {
            WebRequest = default;
            ResponseData = default;
            if (Pool is null || this is not TSelf self)
                return;
            Pool.Release(self);
        }
    }
}