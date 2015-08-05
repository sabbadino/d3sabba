/* Options:
Date: 2015-08-05 10:08:56
Version: 1
BaseUrl: http://localhost:56440

//GlobalNamespace: 
//MakePartial: True
//MakeVirtual: True
//MakeDataContractsExtensible: False
//AddReturnMarker: True
//AddDescriptionAsComments: True
//AddDataContractAttributes: False
//AddIndexesToDataMembers: False
//AddGeneratedCodeAttributes: False
//AddResponseStatus: False
//AddImplicitVersion: 
//InitializeCollections: True
//IncludeTypes: 
//ExcludeTypes: 
//AddDefaultXmlNamespace: http://schemas.servicestack.net/types
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceStack;
using ServiceStack.DataAnnotations;
using SS_ASPNET_01.ServiceModel;


namespace SS_ASPNET_01.ServiceModel
{

    [Route("/dates", "GET")]
    [DataContract]
    public partial class FindDate
        : IReturn<FindDateResponse>
    {
        [DataMember(Name="DateTime")]
        public virtual DateTime DateTime { get; set; }
    }

    [DataContract]
    public partial class FindDateResponse
    {
        public FindDateResponse()
        {
            Dates = new List<DateTime>{};
        }

        [DataMember(Name="Dates")]
        public virtual List<DateTime> Dates { get; set; }
    }
}

