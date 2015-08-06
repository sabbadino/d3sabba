using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SS_ASPNET_01.ServiceModel
{
	[DataContract(Namespace = "mycustomnaspace")]
	public class HelloDentro
	{
		[DataMember]
		public int Number { get; set; }
	}
}
