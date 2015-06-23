using System.Runtime.Serialization;
using EntityDiffSample;

namespace Geographic.Countries.ServiceModel.Types
{
	[DataContract]
	public class CountryWebSite : ILocalEntity
	{
		[DataMember(Name = "localId", Order = 1, IsRequired = true, EmitDefaultValue = false)]
		public string LocalId { get; set; }
	}
}