using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Geographic.Countries.ServiceModel.Types
{
	[DataContract]
	public class Country 
	{
		[DataMember(Name = "refId", Order = 1, IsRequired = true, EmitDefaultValue = false)]
		public CountryRef RefId { get; set; }

		[DataMember(Name = "code", Order = 2, IsRequired = true, EmitDefaultValue = false)]
		public string Code { get; set; }

		[DataMember(Name = "name", Order = 3, IsRequired = true, EmitDefaultValue = false)]
		public string Name { get; set; }

		[DataMember(Name = "displayName", Order = 4, IsRequired = true, EmitDefaultValue = false)]
		public string DisplayName { get; set; }

		[DataMember(Name = "normalizeName", Order = 5)]
		public string NormalizeName { get; set; }

		[DataMember(Name = "nationality", Order = 6)]
		public string Nationality { get; set; }

		[DataMember(Name = "areaRef", Order = 7, IsRequired = true, EmitDefaultValue = false)]
		public CountryAreaRef AreaRef { get; set; }

		[DataMember(Name = "isEuropeanUnion", Order = 11, IsRequired = true)]
		public bool IsEuropeanUnion { get; set; }

		[DataMember(Name = "isIbanMandatory", Order = 12)]
		public bool IsIbanMandatory { get; set; }

		[DataMember(Name = "isPostalCodeMandatory", Order = 13)]
		public bool IsPostalCodeMandatory { get; set; }
		
		[DataMember(Name = "risks", Order = 16)]
		public List<CountryRisk> Risks { get; set; }

		[DataMember(Name = "webSites", Order = 17)]
		public List<CountryWebSite> WebSites { get; set; }
	}
}