using LuckIndia.APIs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;

namespace LusckIndia.APIs.Handlers
{
    public class HttpRequestHelper
    {
        private readonly HttpRequestMessage _request;

        private readonly NameValueCollection _queryStringParams;

        public HttpRequestHelper(HttpRequestMessage request)
        {
            _request = request;
            _queryStringParams = _request.RequestUri.ParseQueryString();
        }

        public string GetQueryParam(string key)
        {
            return _queryStringParams.AllKeys.Contains(key) ?
                _queryStringParams[key] :
                null;
        }

        public string GetHeaderParam(string key)
        {
            IEnumerable<string> headerCollection;

            return _request.Headers.TryGetValues(key, out headerCollection) ?
                headerCollection.FirstOrDefault() :
                null;
        }

        public bool GetBoolDefaultTrue(string key)
        {
            var rawValue = this.GetQueryParam(key).ToLower();
            return rawValue != "false";
        }

        public DateTime GetDateTime(string key)
        {
            DateTime toReturn;
            DateTime.TryParse(this.GetQueryParam(key), out toReturn);
            return toReturn;
        }

        public List<int> GetListOfInts(string key)
        {
            var rawValue = GetQueryParam(key);

            if (String.IsNullOrWhiteSpace(rawValue))
            {
                return new List<int>();
            }

            try
            {
                var toReturn = rawValue.Split(',').Select(int.Parse).ToList();
                return toReturn;
            }
            catch
            {
                return new List<int>();
            }
        }

        public string GetAccessToken()
        {
            var token = this.GetAccessTokenFromAuthorizationHeader();
            if (null != token)
            {
                return token;
            }

            token = this.GetAccessTokenFromAccessTokenHeader();
            return token ?? this.GetAccessTokenFromQueryStringNameAccessToken();
        }

        private string GetAccessTokenFromAuthorizationHeader()
        {
            if (_request.Headers.Authorization == null)
            {
                return null;
            }

            if (!_request.Headers.Authorization.Scheme.Equals(WebApiConfig.AUTHORIZATION_HEADER_SCHEME))
            {
                return null;
            }

            var parameter = _request.Headers.Authorization.Parameter;

            return !string.IsNullOrWhiteSpace(parameter) ? parameter : null;
        }

        private string GetAccessTokenFromAccessTokenHeader()
        {
            var token = this.GetHeaderParam(WebApiConfig.HeaderNameAccessToken);

            return !string.IsNullOrWhiteSpace(token) ? token : null;
        }

        private string GetAccessTokenFromQueryStringNameAccessToken()
        {
            return WebApiConfig.QueryStringNamesAccessToken
                .Select(this.GetQueryParam)
                .FirstOrDefault(token => !string.IsNullOrWhiteSpace(token));
        }
    }
}
