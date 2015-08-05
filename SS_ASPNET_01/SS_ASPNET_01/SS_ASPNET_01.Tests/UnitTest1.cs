using System;
using System.Configuration;
using NUnit.Framework;
using ServiceStack;
using ServiceStack.Text;
using SSTACK_OP = SS_ASPNET_01.ServiceModel;

namespace SS_ASPNET_01.Tests
{
    [TestFixture]
    public class UnitTests
    {
    //    private readonly ServiceStackHost appHost;

        public UnitTests()
        {
      //      appHost = new BasicAppHost(typeof(MyServices).Assembly)
           // {
            //    ConfigureContainer = container =>
              //  {
                    //Add your IoC dependencies here
               // }
           // }
        //    .Init();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
          //  appHost.Dispose();
        }

        [Test]
        public void TestMethod1()
        {
					string baseUrl = ConfigurationManager.AppSettings["baseUrl"]; //"http://localhost:61805";
					using (var jsonClient = new JsonServiceClient(baseUrl))
	        {
						JsConfig.DateHandler = DateHandler.RFC1123;
						JsConfig<DateTime>.RawSerializeFn = time =>
						{
							var x = new DateTime(time.Ticks, DateTimeKind.Unspecified).ToString("o");
							return x;
						};

						JsConfig<DateTime>.DeSerializeFn = time =>
						{
							var x = DateTime.ParseExact(time, "o", null);
							return x;
						};

						var findDate = new SSTACK_OP.FindDate();
						findDate.DateTime = new DateTime(2015, 08, 12, 12, 12, 12, DateTimeKind.Unspecified);
						var respp = jsonClient.Post<SSTACK_OP.FindDateResponse>("/json/reply/FindDate", findDate);
						var respg = jsonClient.Get<SSTACK_OP.FindDateResponse>(findDate);
	        }

        }

    }
}
