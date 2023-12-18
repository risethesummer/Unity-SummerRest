using System;

namespace SummerRest.Scripts.Models
{
    [Serializable]
    public enum HttpMethod
    {
        Get, Post, Put, Delete, Patch, Head, Options, Trace, Connect
    }
}