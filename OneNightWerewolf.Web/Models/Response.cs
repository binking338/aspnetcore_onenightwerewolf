using System;
namespace OneNightWerewolf.Web.Models
{
    public class Response<T>
    {
        public Response()
        {
        }

        public T Data { get; set; }

        public int Code { get; set; }

        public string Message { get; set; }

        public static Response<T> Return(T data)
        {
            Response<T> response = new Response<T>();
            response.Data = data;
            return response;
        }
        public static Response<T> Error(int code, string message = null, T data = default(T))
        {
            Response<T> response = new Response<T>();
            response.Code = code;
            response.Message = message;
            response.Data = data;
            return response;
        }
    }
}
