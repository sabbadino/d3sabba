using System;
using System.Collections.Generic;
using System.Diagnostics;

using Funq;
using ServiceStack;
using ServiceStack.Admin;
using ServiceStack.Web;
using SS_ASPNET_01.ServiceInterface;

using ServiceStack.Api.Swagger;
using ServiceStack.DataAnnotations;
using ServiceStack.FluentValidation;
using ServiceStack.Host.Handlers;
using ServiceStack.Metadata;
using ServiceStack.MiniProfiler;
using ServiceStack.Text;
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

		
     
        public override void Configure(Container container)
        {

					//JsConfig.DateHandler = DateHandler.DCJSCompatible;

					JsConfig<DateTime>.SerializeFn = time =>
					{
						var x = new DateTime(time.Ticks, DateTimeKind.Unspecified).ToString("o");
						return x;
					};




					JsConfig<DateTime>.DeSerializeFn = time =>
					{
						// THIS IS NOT CALLED WHEN DOING A GET TO FindDate (date is in querystring)
						var x = DateTime.ParseExact(time, "o", null);
						return x;
					};

					//Plugins.Add(new RequestLogsFeature());
					Plugins.Add(new SwaggerFeature());
					//Plugins.Add(new ValidationFeature());
					//container.Register<ServiceStack.FluentValidation.IValidator<Hello2>>(
					//		new Hello2ValidatorCollection());

            SetConfig(new HostConfig
            {
							
							WsdlServiceNamespace = "http://myschemas.myservicestack.net/types",
							//WsdlServiceNamespace = "http://myschemas.myservicestack.net/WsdlServiceNamespace",
                DebugMode = true,
                ReturnsInnerException = false
            });


					//ServiceExceptionHandlers.Add(this.ServiceExceptionHandler);
					//UncaughtExceptionHandlers.Add(this.UncaughtExceptionHandler);
					//GlobalResponseFilters.Add(this.GlobalResponseFilter);


					//typeof(AuthUserSession).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(Profiler).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(SqlTiming).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(SqlTimingParameter).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(Timing).AddAttributes(new ExcludeAttribute(Feature.Soap));

					//typeof(RequestInfo).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(RequestLogs).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(RequestLogEntry).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(RequestLogEntry[]).AddAttributes(new ExcludeAttribute(Feature.Soap));
					////typeof(ResponseError[]).AddAttributes(new ExcludeAttribute(Feature.Soap));
					////typeof(ResponseError).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(RequestLogsResponse).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(RequestInfoResponse).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(ResourceResponse).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(MethodDescription).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(MethodOperation).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(IEnumerable<MethodOperation>).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(MethodOperationParameter).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(ParameterAllowableValues).AddAttributes(new ExcludeAttribute(Feature.Soap));

					//typeof(ResourceRequest).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(SwaggerModel).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(ModelProperty).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(ServiceStack.Api.Swagger.Resources).AddAttributes(new ExcludeAttribute(Feature.Soap));
					//typeof(ServiceStack.Api.Swagger.ResourcesResponse).AddAttributes(new ExcludeAttribute(Feature.Soap));

					//typeof(RestService).AddAttributes(new ExcludeAttribute(Feature.Soap));


    //});





      
        }

			//public override string GenerateWsdl(WsdlTemplateBase wsdlTemplate)
			//{
			//	string str = wsdlTemplate.ToString().Replace("http://schemas.datacontract.org/2004/07/ServiceStack", this.Config.WsdlServiceNamespace);
			//	//if (this.Config.WsdlServiceNamespace != "http://schemas.servicestack.net/types")
			//		//str = str.Replace("http://schemas.servicestack.net/types", this.Config.WsdlServiceNamespace);
			//	return str;
			//	//return base.GenerateWsdl(wsdlTemplate);
			//}
    }



		//public class Hello2ValidatorCollection : AbstractValidator<Hello2>
		//{
		//	public Hello2ValidatorCollection()
		//	{
		//		When(x => true, () => { RuleFor(r => r.Number).GreaterThan(-1); });
		//	}
		//}

}