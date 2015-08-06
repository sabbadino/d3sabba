using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceStack;

namespace SS_ASPNET_01.ServiceModel
{
	[Route("/dates", "GET")]
	[DataContract]
	public class FindDate : IReturn<FindDateResponse>
	{
		[DataMember(Name = "DateTime")]
		public DateTime DateTime { get; set; }
	}

	[DataContract]
	public class FindDateResponse : IHasResponseStatus
	{
		[DataMember(Name = "Dates")]
		public List<DateTime> Dates { get; set; }

		ResponseStatus _ResponseStatus = new ResponseStatus();

		public ResponseStatus ResponseStatus
		{
			get { return _ResponseStatus; }
			set { _ResponseStatus = value; }
		}
	}
}
