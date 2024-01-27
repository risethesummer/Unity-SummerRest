using System.Collections.Generic;
using System.Text;
using RestSourceGenerator.Utilities;

namespace RestSourceGenerator.Metadata
{
    public struct Request
    {
        public string TypeName { get; set; }
        public string EndpointName { get; set; }
        public string? Url { get; set; }
        public string? UrlWithParams { get; set; }
        public HttpMethod? Method { get; set; }
        public int? TimeoutSeconds { get; set; }
        public int? RedirectsLimit { get; set; }
        public ContentType? ContentType { get; set; }
        public KeyValue[]? Headers { get; set; }
        public KeyValue[]? RequestParams { get; set; }
        public AuthContainer? AuthContainer { get; set; }
        public DataFormat? DataFormat { get; set; }
        public string? SerializedBody { get; set; }
        public KeyValue[]? SerializedForm { get; set; }
        public IEnumerable<Request>? Services { get; set; }
        public IEnumerable<Request>? Requests { get; set; }
        public bool IsMultipart { get; set; }

        private string BuildHeaders()
        {
            if (Headers is not { Length: > 0 })
                return string.Empty;
            return Headers.BuildSequentialValues(e => $@"Headers.Add(""{e.Key}"", ""{e.Value}"")", ";") + ";";
        }

        private string BuildParams()
        {
            if (RequestParams is not { Length: > 0 })
                return string.Empty;
            return RequestParams.BuildSequentialValues(e => $@"Params.AddParam(""{e.Key}"", ""{e.Value}"")", ";") + ";";
        }

        private string BuildBaseClass()
        {
            if (!IsMultipart)
                return $"SummerRest.Runtime.Requests.BaseDataRequest<{EndpointName.ToClassName()}>";
            return $"SummerRest.Runtime.Requests.BaseMultipartRequest<{EndpointName.ToClassName()}>";
            //return $"SummerRest.Runtime.Requests.BaseAuthRequest<{EndpointName.ToClassName()}, {AuthContainer.Value.AppenderType}, {AuthContainer.Value.AuthDataType}>";
        }
        private (string authClass, string authKey) BuildAuth()
        {
            if (!AuthContainer.HasValue)
                return ("null", string.Empty);
            return ($@"
IRequestModifier<AuthRequestModifier<{AuthContainer.Value.AppenderType}, {AuthContainer.Value.AuthDataType}>>.GetSingleton()", 
                $@"
AuthKey = ""{AuthContainer.Value.AuthKey}"";
");
        }

        private string BuildBody()
        {
            if (!IsMultipart)
            {
                if (string.IsNullOrEmpty(SerializedBody))
                    return $"BodyFormat = DataFormat.{DataFormat};";
                return $@"
BodyFormat = DataFormat.{DataFormat};
InitializedSerializedBody = @""{SerializedBody.Replace("\"", "\"\"")}"";";
            }
            if (SerializedForm is not {Length: >0})
                return string.Empty;
            return SerializedForm.BuildSequentialValues(e => $@"MultipartFormSections.Add(new MultipartFormDataSection(""{e.Key}"", ""{e.Value}""))", ";") + ";";
        }
        public void BuildRequest(StringBuilder builder)
        {
            var className = EndpointName.ToClassName();
            var method = $"HttpMethod.{Method}";
            var timeout = TimeoutSeconds.HasValue ? $"{nameof(TimeoutSeconds)} = {TimeoutSeconds.Value};" : string.Empty;
            var redirects = RedirectsLimit.HasValue ? $"{nameof(RedirectsLimit)} = {RedirectsLimit.Value};" : string.Empty;
            var contentType = ContentType.HasValue ? 
                $@"{nameof(ContentType)} = new ContentType(""{ContentType.Value.MediaType}"", ""{ContentType.Value.Charset}"", ""{ContentType.Value.Boundary}"");" : string.Empty;
            var headers = BuildHeaders();
            var @params = BuildParams();
            var (authProp, authKey) = BuildAuth();
            var body = BuildBody();
            builder.Append($@"
public class {className} : {BuildBaseClass()}
{{
    public {className}() : base(""{Url}"", ""{UrlWithParams}"", {authProp}) 
    {{
        Method = {method};
        {timeout}
        {redirects}
        {contentType}
        {headers}
        {@params}
        {authKey}
        {body}
        Init();
    }}
}}
");
        }
        public void BuildClass(StringBuilder builder)
        {
            if (TypeName == "Request")
            {
                BuildRequest(builder);
            }
            else
            {
                builder.Append($"public static class {EndpointName.ToClassName()} {{");
                if (Requests is not null)
                {
                    foreach (var request in Requests)
                        request.BuildClass(builder);
                }
                if (Services is not null)
                {
                    foreach (var service in Services)
                        service.BuildClass(builder);
                }   
                builder.Append("}");
            }
        }
    }
}