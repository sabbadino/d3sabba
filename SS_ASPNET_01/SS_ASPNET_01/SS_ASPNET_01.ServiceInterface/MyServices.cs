using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ServiceStack;
using SS_ASPNET_01.ServiceModel;
using SS_ASPNET_01.ServiceModel;

namespace SS_ASPNET_01.ServiceInterface
{
	public class MyServices : Service
	{

		public FindDateResponse Any(FindDate request)
		{
			using (var fs = File.AppendText("C:\\server_logdates.txt"))
			{
				fs.WriteLine("Kind=" + request.DateTime.Kind
										 + " yyyyMMddHHmmss=" + request.DateTime.ToString("yyyyMMddHHmmss")
										 + " ToLocalTime_yyyyMMddHHmmss=" + request.DateTime.ToLocalTime().ToString("yyyyMMddHHmmss")
										 + " yyyyMMddHHmmss zzz=" + request.DateTime.ToString("yyyyMMddHHmmss zzz"));

				var resp = new FindDateResponse();
				resp.Dates = new List<DateTime>();
				resp.Dates.Add(new DateTime(2015, 08, 12, 12, 12, 12, DateTimeKind.Local));
				//resp.Dates[0] = DateTime.SpecifyKind(resp.Dates[0], DateTimeKind.Unspecified);
				return resp;
			}
		}

		//public object Any(Hello2 request)
		//{
		//	return new Hello2Response {Result = request.Number + 1};
		//}

		//public object Any(Hello request)
		//{
		//	var x = new HelloResponse { HelloFuori = new HelloFuori { Number = request.HelloDentro.Number}};
		//	x.ResponseStatus.Message = "hello";
		//	return x;
		//}
	}
}