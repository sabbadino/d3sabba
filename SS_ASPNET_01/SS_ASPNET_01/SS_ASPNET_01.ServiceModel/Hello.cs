using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;
using System.Runtime.Serialization;
using SS_ASPNET_01.ServiceModel;

namespace SS_ASPNET_01.ServiceModel
{
	[Route("/hello")]
	[DataContract]
	public class Hello : IReturn<HelloResponse>
	{
		[DataMember]
		public HelloDentro HelloDentro { get; set; }
	}

	[DataContract]
	public class HelloResponse  : ResponseStatusBase
	{
		[DataMember]
		public HelloFuori HelloFuori { get; set; }
	}
}