using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;
using System.Runtime.Serialization;

namespace SS_ASPNET_01.ServiceModel
{
    [Route("/sayhello/{Name}",Verbs="PUT")]
    [DataContract]
    public class HelloWord : IReturn<HelloWordResponsexx>
    {
          [DataMember]
        public string MyName { get; set; }
    }
      [DataContract]
    public class HelloWordResponsexx
    {
          [DataMember]
        public string Result { get; set; }
    }
}

//namespace SS_ASPNET_01.HEllo22
//{
//    [Route("/sayhello2/{Name}", Verbs = "PUT")]
//    public class Hello : IReturn<HelloResponse>
//    {
//        public string Name { get; set; }
//    }

//    public class HelloResponse
//    {
//        public ResponseStatus ResponseStatus { get; set; } //Automatic exception handling

//        public string Result { get; set; }
//    }
//}