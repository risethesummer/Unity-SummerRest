﻿using SummerRest.Runtime.Authenticate.TokenRepositories;
using SummerRest.Runtime.RequestAdaptor;
using UnityEngine;

namespace SummerRest.Runtime.Authenticate.Appenders
{
    /// <summary>
    /// Bearer token appender simply adding a header {"Authorization": "Bearer ..."}
    /// </summary>
    public class BearerTokenAuthAppender : IAuthAppender<BearerTokenAuthAppender>
    {
        public void Append<TResponse>(string authDataKey, IWebRequestAdaptor<TResponse> requestAdaptor)
        {
            var data = IAuthDataRepository.Current.Get<string>(authDataKey);
            if (string.IsNullOrEmpty(data))
            {
                Debug.LogWarningFormat(@"The auth key ""{0}"" does not exist in the program", authDataKey);
                return;
            }
            requestAdaptor.SetHeader("Authorization", $"Bearer {data}");
        }
    }
}