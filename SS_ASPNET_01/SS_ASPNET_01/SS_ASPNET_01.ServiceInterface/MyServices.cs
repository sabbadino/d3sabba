using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;
using SS_ASPNET_01.ServiceModel;

namespace SS_ASPNET_01.ServiceInterface
{
    public class MyServices : Service
    {
        public object Any(Hello request)
        {
            return new HelloResponse { Result = "Hello, {0}!".Fmt(request.Name) };
        }
    }

    public class MyServices2 : Service
    {
        private void x()
        {
            throw new Exception("bbb");
        }

        public object Any(HEllo22.Hello request)
        {
            //try
            //{
                try
                {
                    x();
                    return new HEllo22.HelloResponse {Result = "Hello, {0}!".Fmt(request.Name)};
                }
                catch (Exception ex)
                {
                    throw new Exception("aaa", ex);
                }
            //}
            //catch (Exception ex)
            //{
            //    var x = new ResponseStatus()
            //    {
            //        ErrorCode = "500",
            //            Message = ex.ToString()
            //    };
            //    var y = new ResponseError();
            //    y.ErrorCode = "500";
            //    x.Errors = new List<ResponseError>();
            //    x.Errors.Add(y);

            //    return new HEllo22.HelloResponse() { ResponseStatus = x};
            //}
        }
    }
}