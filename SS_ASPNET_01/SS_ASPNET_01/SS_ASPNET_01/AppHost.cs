using System;
using System.Diagnostics;

using Funq;
using ServiceStack;

using ServiceStack.Web;
using SS_ASPNET_01.ServiceInterface;

using ServiceStack.Api.Swagger;
using ServiceStack.FluentValidation;
using ServiceStack.Validation;
using SS_ASPNET_01.ServiceModel;

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


        public override void OnEndRequest(IRequest request = null)
        {
            base.OnEndRequest(request);
        }

        public override void OnStartupException(Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            base.OnStartupException(ex);
        }


			
      public override void OnUncaughtException(IRequest httpReq, IResponse httpRes, string operationName, Exception ex)
        {
            base.OnUncaughtException(httpReq, httpRes, operationName, ex);
        }

	    public override object OnServiceException(IRequest httpReq, object request, Exception ex)
	    {
				return DtoUtils.CreateErrorResponse(request, ex);
	    }

			public override void OnExceptionTypeFilter(Exception ex, ResponseStatus responseStatus)
			{
				
			}

	    public object ServiceExceptionHandler(IRequest httpReq, object request, Exception exception)
				{
				
						return DtoUtils.CreateErrorResponse(request, exception);
					
				}


				public void UncaughtExceptionHandler(IRequest httpReq, IResponse httpRes, string operationName, Exception ex)
				{
					
				}
      
        public override void Configure(Container container)
        {

					Plugins.Add(new SwaggerFeature());
					Plugins.Add(new ValidationFeature());
					container.Register<ServiceStack.FluentValidation.IValidator<Hello2>>(
							new Hello2ValidatorCollection());

            SetConfig(new HostConfig
            {
                WsdlServiceNamespace = "http://myschemas.myservicestack.net/types",
                DebugMode = true,
                ReturnsInnerException = false
            });


	        ServiceExceptionHandlers.Add(this.ServiceExceptionHandler);
					UncaughtExceptionHandlers.Add(this.UncaughtExceptionHandler);





    //});





      
        }

      
       
    }



		public class Hello2ValidatorCollection : AbstractValidator<Hello2>
		{
			public Hello2ValidatorCollection()
			{
				When(x => true, () => { RuleFor(r => r.Number).GreaterThan(-1); });
			}
		}

}