using System.Runtime.Serialization;
// ReSharper disable All

namespace Geographic.Countries.ServiceModel.Types
{
	[DataContract]
	public class CountryRef
	{
		[DataMember(Name = "id", IsRequired = true, EmitDefaultValue = false)]
		public string Id { get; set; }
	}
}
