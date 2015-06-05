using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;

namespace WebApplication1
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [SoapDocumentService(SoapBindingUse.Literal,
  SoapParameterStyle.Wrapped)]

    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public GetCustomersResponse GetCustomers(GetCustomers GetCustomersDTO)
        {
            throw new Exception("sss");
            return new GetCustomersResponse { Customers = new List<Customer>() { new Customer() { Name = GetCustomersDTO.MyName } } };
        }
    }


   
    public class GetCustomers 
    {
        public string MyName { get; set; }
        public string MySecondName { get; set; }

    }

    public class GetCustomersResponse
    {

        private List<Customer> _Customers = new List<Customer>();

        public List<Customer> Customers
        {
            get { return _Customers; }
            set { _Customers = value; }
        }

    }

    public class Customer
    {
        public string Name { get; set; }
    }

}
