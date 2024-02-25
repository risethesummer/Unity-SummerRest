﻿using System;
using System.Collections.Generic;
using System.Net;
using SummerRest.Runtime.RequestComponents;

namespace SummerRest.Runtime.Requests
{
    /// <summary>
    /// Contains response data of a web request
    /// </summary>
    /// <typeparam name="TBody"></typeparam>
    public struct WebResponse<TBody> : IDisposable
    {
        public object WrappedRequest { get;  }
        public HttpStatusCode StatusCode { get;  }
        public ContentType ContentType { get; }
        public IEnumerable<KeyValuePair<string, string>> Headers { get; }
        public string RawData { get;  }
        public TBody Data { get;  }

        public WebResponse(object wrappedRequest, HttpStatusCode statusCode, ContentType contentType, 
            IEnumerable<KeyValuePair<string, string>> headers, string rawData, TBody data)
        {
            WrappedRequest = wrappedRequest;
            StatusCode = statusCode;
            ContentType = contentType;
            Headers = headers;
            RawData = rawData;
            Data = data;
        }

        public void Dispose()
        {
            if (WrappedRequest is IDisposable disposable)
                disposable.Dispose();
        }
    }
}