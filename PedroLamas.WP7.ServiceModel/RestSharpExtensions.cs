using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;

namespace PedroLamas.WP7.ServiceModel
{
    public static class RestSharpExtensions
    {
        public static RestRequestAsyncHandle GetResultAsync(this RestClient client, ResultCallback<string> callback, object state = null)
        {
            return client.GetResultAsync(new RestRequest(), callback, state, x => x);
        }

        public static RestRequestAsyncHandle GetResultAsync<T>(this RestClient client, ResultCallback<T> callback, object state = null)
        {
            return client.GetResultAsync(new RestRequest(), callback, state, x => JsonConvert.DeserializeObject<T>(x));
        }

        public static RestRequestAsyncHandle GetResultAsync(this RestClient client, RestRequest request, ResultCallback<string> callback, object state = null)
        {
            return client.GetResultAsync(request, callback, state, x => x);
        }

        public static RestRequestAsyncHandle GetResultAsync<T>(this RestClient client, RestRequest request, ResultCallback<T> callback, object state = null)
        {
            return client.GetResultAsync(request, callback, state, x => JsonConvert.DeserializeObject<T>(x));
        }

        public static RestRequestAsyncHandle GetResultAsync<T>(this RestClient client, RestRequest request, ResultCallback<T> callback, object state, Func<string, T> deserialize)
        {
            return client.ExecuteAsync(request, response =>
            {
                try
                {
                    if (response.ResponseStatus == ResponseStatus.Aborted)
                    {
                        callback(new Result<T>(ResultStatus.Aborted, state));

                        return;
                    }
                    else if (response.ErrorException != null)
                    {
                        callback(new Result<T>(response.ErrorException, state));

                        return;
                    }

                    DateTime? lastModified = null;
                    DateTime lastModifiedDate;

                    if (DateTime.TryParse(response.Headers.GetParameterValue("Last-Modified"), CultureInfo.InvariantCulture, DateTimeStyles.None, out lastModifiedDate))
                        lastModified = lastModifiedDate;

                    var etag = response.Headers.GetParameterValue("ETag");

                    T data;

                    try
                    {
                        data = deserialize(response.Content);
                    }
                    catch
                    {
                        data = default(T);
                    }

                    callback(new Result<T>(data, response.StatusCode, lastModified, etag, state));
                }
                catch (Exception ex)
                {
                    callback(new Result<T>(ex, state));
                }
            });
        }

        public static Parameter GetParameter(this IList<Parameter> parameters, string name, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            return parameters.FirstOrDefault(x => string.Equals(x.Name, name, stringComparison));
        }

        public static string GetParameterValue(this IList<Parameter> parameters, string name)
        {
            var parameter = parameters.GetParameter(name);

            return parameter != null && parameter.Value != null ? parameter.Value.ToString() : null;
        }
    }
}