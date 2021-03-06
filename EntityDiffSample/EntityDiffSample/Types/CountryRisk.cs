﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using EntityDiffSample;

namespace Geographic.Countries.ServiceModel.Types
{
	[DataContract]
	public class CountryRisk : ILocalEntity
	{
		[DataMember(Name = "localId", Order = 1, IsRequired = true, EmitDefaultValue = false)]
		public string LocalId { get; set; }

		[DataMember(Name = "description", Order = 2)]
		public string Description { get; set; }


		[DataMember(Name = "childrenOfCountryRisk", Order = 17)]
		public List<ChildrenOfCountryRisk> ChildrenOfCountryRisk { get; set; }

	}
}