using System.Runtime.Serialization;

namespace Geographic.Countries.ServiceModel.Types
{
	[DataContract]
	public class CountryAreaRef
	{
		[DataMember(Name = "id", IsRequired = true, EmitDefaultValue = false)]
		public string Id { get; set; }
	}
	
	[DataContract]
	public class CountryArea
	{
		[DataMember(Name = "refId", Order = 1, IsRequired = true, EmitDefaultValue = false)]
		public CountryAreaRef RefId { get; set; }

		[DataMember(Name = "code", Order = 2, IsRequired = true, EmitDefaultValue = false)]
		public string Code { get; set; }

		[DataMember(Name = "name", Order = 3, IsRequired = true, EmitDefaultValue = false)]
		public string Name { get; set; }
	}
}