using System.Runtime.Serialization;
using ServiceStack;

namespace SS_ASPNET_01.ServiceModel
{
	[DataContract]
	public abstract class ResponseStatusBase : IHasResponseStatus
	{
		protected ResponseStatusBase()
		{
			ResponseStatus = new ResponseStatus();
		}

		[DataMember]
		public ResponseStatus ResponseStatus { get; set; }
	}
}