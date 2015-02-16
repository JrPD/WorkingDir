using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SalesForAmazon
{
    using SalesForAmazon.DAL;

    public class MvcApplication : System.Web.HttpApplication
    {
        public static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static BooksDownloader BooksDownloader = new BooksDownloader();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            logger.Info("Start");
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}