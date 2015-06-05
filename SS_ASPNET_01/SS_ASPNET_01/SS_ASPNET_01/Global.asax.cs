using System;
using System.Web;

namespace SS_ASPNET_01
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            new AppHost().Init();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            try
            {
                    Console.WriteLine((HttpContext.Current.Request.HttpMethod + ": " + HttpContext.Current.Request.RawUrl));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}