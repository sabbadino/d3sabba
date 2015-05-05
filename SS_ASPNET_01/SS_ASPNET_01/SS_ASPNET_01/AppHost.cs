using System;
using System.Text;
using Funq;
using ServiceStack;
using ServiceStack.Model;
using ServiceStack.Web;
using SS_ASPNET_01.ServiceInterface;

namespace SS_ASPNET_01
{
    public class AppHost : AppHostBase
    {
        /// <summary>
        /// Default constructor.
        /// Base constructor requires a name and assembly to locate web service classes. 
        /// </summary>
        public AppHost()
            : base("SS_ASPNET_01", typeof(MyServices).Assembly)
        {

        }

        /// <summary>
        /// Application specific configuration
        /// This method should initialize any IoC resources utilized by your web service classes.
        /// </summary>
        /// <param name="container"></param>
        public override void Configure(Container container)
        {
         
             this.ServiceExceptionHandlers.Add((httpReq, request, exception) => {
             return MyCreateErrorResponse(request, exception);
                 return DtoUtils.CreateErrorResponse(request, exception);

    });

            //Config examples
            //this.Plugins.Add(new PostmanFeature());
            //this.Plugins.Add(new CorsFeature());
        }

        public static object MyCreateErrorResponse(object request, Exception ex)
        {
   
      ResponseStatus responseStatus = ex.MyToResponseStatus();
      if (HostContext.DebugMode)
        responseStatus.StackTrace = DtoUtils.GetRequestErrorBody(request) + "\n" + ex.StackTrace;
      object errorResponse = DtoUtils.CreateErrorResponse(request, ex, responseStatus);
      HostContext.OnExceptionTypeFilter(ex, responseStatus);
      return errorResponse;
 
        }

       
    }



    public static class Extensions
{

        public static ResponseStatus MyToResponseStatus(this Exception exception)
        {
            IResponseStatusConvertible statusConvertible = exception as IResponseStatusConvertible;
            if (statusConvertible == null)
                return DtoUtils.CreateResponseStatus(exception.GetType().Name, exception.AllMessages());
            return statusConvertible.ToResponseStatus();
        }

        public static string AllMessages(this Exception ex)
        {
            var sb = new StringBuilder();
            while (ex != null)
            {
                sb.Append(ex.Message);
                if(ex.InnerException!=null)
                    sb.Append(" --> ");
                ex = ex.InnerException;
            }
            return sb.ToString();
        }

}

}