using System;
using System.Net;

namespace PedroLamas.WP7.ServiceModel
{
    public class Result<T>
    {
        #region Properties
        public ResultStatus Status { get; private set; }

        public T Data { get; private set; }

        public HttpStatusCode StatusCode { get; private set; }

        public DateTime? LastModified { get; private set; }

        public string ETag { get; private set; }

        public Exception Error { get; private set; }

        public object State { get; private set; }

        #endregion

        public Result(T data, HttpStatusCode statusCode, DateTime? lastModified, string etag, object state)
        {
            Data = data;
            StatusCode = statusCode;
            LastModified = lastModified;
            ETag = etag;
            State = state;

            switch (statusCode)
            {
                case HttpStatusCode.OK:
                    Status = ResultStatus.Completed;
                    break;

                case HttpStatusCode.NoContent:
                case HttpStatusCode.NotModified:
                    Status = ResultStatus.Empty;
                    break;

                default:
                    Status = ResultStatus.Error;
                    break;
            }
        }

        public Result(Exception error, object state)
        {
            Error = error;
            State = state;

            Status = ResultStatus.Error;
        }

        public Result(ResultStatus status, object state)
        {
            Status = status;
            State = state;
        }
    }
}