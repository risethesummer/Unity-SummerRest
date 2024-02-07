﻿using System;
using System.Collections.Generic;
using SummerRest.Runtime.Extensions;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{

    /// <summary>
    /// Wrapping <see cref="UnityWebRequest"/> static factory methods to create <see cref="UnityWebRequestAdaptor{TSelf,TResponse}"/>
    /// </summary>
    public class UnityWebRequestAdaptorProvider : IWebRequestAdaptorProvider
    {
        public IWebRequestAdaptor<Texture2D> GetTextureRequest(string url, bool nonReadable)
        {
            var request = UnityWebRequestTexture.GetTexture(url, nonReadable);
            return TextureUnityWebRequestAdaptor.Create(request);
        }
        public IWebRequestAdaptor<AudioClip> GetAudioRequest(string url, AudioType audioType)
        {
            var request = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
            return AudioUnityWebRequestAdaptor.Create(request);
        }
        public IWebRequestAdaptor<UnityWebRequest> GetFromUnityWebRequest(UnityWebRequest webRequest)
        {
            return DumpUnityWebRequestAdaptor.Create(webRequest);
        }
        public IWebRequestAdaptor<TResponse> GetDataRequest<TResponse>(
            string url, HttpMethod method, string bodyData, string contentType)
        {
            UnityWebRequest request;
            switch (method)
            {
                case HttpMethod.Get or HttpMethod.Trace or HttpMethod.Connect or HttpMethod.Options:
                    request = UnityWebRequest.Get(url);
                    break;
                case HttpMethod.Post:
                    if (string.IsNullOrEmpty(contentType))
                        contentType = ContentType.Commons.TextPlain.FormedContentType;
                    request = UnityWebRequest.Post(url, bodyData, contentType);
                    break;
                case HttpMethod.Put  or HttpMethod.Patch:
                    request = UnityWebRequest.Put(url, bodyData);
                    break;
                case HttpMethod.Delete:
                    request = UnityWebRequest.Delete(url);
                    break;
                default:
                    request = new UnityWebRequest(url, method.ToUnityHttpMethod());
                    break;
            }

            request.method = method.ToUnityHttpMethod();
            return RawUnityWebRequestAdaptor<TResponse>.Create(request);
        }

        public IWebRequestAdaptor<TResponse> GetMultipartFileRequest<TResponse>(string url,
            HttpMethod method,
            List<IMultipartFormSection> data)
        {
            var request = UnityWebRequest.Post(url, data, Array.Empty<byte>());
            request.method = method.ToUnityHttpMethod();
            return MultipartFileUnityWebRequestAdaptor<TResponse>.Create(request);
        }
    }
}