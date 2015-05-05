using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;

namespace SS_ASPNET_01.ServiceModel
{
    [Route("/sayhello/{Name}",Verbs="PUT")]
    public class Hello : IReturn<HelloResponse>
    {
        public string Name { get; set; }
    }

    public class HelloResponse
    {
        public string Result { get; set; }
    }
}

namespace SS_ASPNET_01.HEllo22
{
    [Route("/sayhello2/{Name}", Verbs = "PUT")]
    public class Hello : IReturn<HelloResponse>
    {
        public string Name { get; set; }
    }

    public class HelloResponse
    {
        public ResponseStatus ResponseStatus { get; set; } //Automatic exception handling

        public string Result { get; set; }
    }
}