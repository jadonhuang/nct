using System.Numerics;

namespace Nct
{
    public class RetResult : RetResultOfT<object>
    {

        public override RetResult Success(object data = default, string msg = null)
        {
            return (RetResult)base.Success(data, msg);
        }


        public override RetResult Error(string msg = null, int statusCode = 500)
        {
            return (RetResult)base.Error(msg, statusCode);
        }


    }


    public class ResultOfT<T>
    {

        public bool IsSuccess { get; set; } = true;

        public string Msg { get; set; }

        public T Data { get; set; }


        public virtual ResultOfT<T> Success(T data = default, string msg = null)
        {
            IsSuccess = true;
            Msg = msg;
            Data = data;
            return this;
        }


        public virtual ResultOfT<T> Error(string msg = null)
        {
            IsSuccess = false;
            Msg = msg;
            return this;
        }



    }


    public class Result : ResultOfT<object>
    {
        public override Result Success(object data = default, string msg = null)
        {
            base.Success(data, msg);
            return this;
        }


        public override Result Error(string msg = null)
        {
            base.Error(msg);
            return this;
        }



    }


    public class RetResultOfT<T> : ResultOfT<T>
    {
        public int StatusCode { get; set; } = 200;

        public override RetResultOfT<T> Success(T data = default, string msg = null)
        {
            StatusCode = 200;
            base.Success(data, msg);
            return this;
        }


        public virtual RetResultOfT<T> Error(string msg = null, int statusCode = 500)
        {
            StatusCode = statusCode;
            base.Error(msg);
            return this;
        }

    }

}
