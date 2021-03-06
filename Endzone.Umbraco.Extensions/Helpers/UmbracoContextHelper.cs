﻿using System.IO;
using System.Web;
using System.Web.Hosting;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace Endzone.Umbraco.Extensions.Helpers
{
    public class UmbracoContextHelper
    {
        /// <summary>
        /// Ensures UmbracoContext exists for current request by either using existing one, or creating one based on the current request.
        /// </summary>
        /// <example>
        /// A custom PluginController that gets called outside the Umbraco context (route created using the RouteTable.Routes.MapRoute method):
        /// public class MyPluginController : PluginController
        /// {
        ///     public MyPluginController() : base(UmbracoContextHelper.EnsureContext())
        ///     {
        ///         public ActionResult MyAction(int nodeId)
        ///         {
        ///             var node = UmbracoContext.ContentCache.GetById(nodeId);
        ///         }
        ///     }
        /// }
        /// </example>
        /// <param name="replaceContext">Whether to replace the current context if it already exists (defaults to false)</param>
        /// <returns></returns>
        public static UmbracoContext EnsureContext(bool replaceContext = false)
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current ?? CreateHttpContext());
            var applicationContext = ApplicationContext.Current;

            return UmbracoContext.EnsureContext(
                httpContext,
                applicationContext,
                new WebSecurity(httpContext, applicationContext),
                UmbracoConfig.For.UmbracoSettings(),
                UrlProviderResolver.Current.Providers,
                replaceContext);
        }

        /// <summary>
        /// Creates an HttpContext instance based on a dummy request.
        /// </summary>
        /// <example>
        /// Will be used when there is no real HttpContext, for example in tasks running on a background thread.
        /// </example>
        /// <returns></returns>
        private static HttpContext CreateHttpContext()
        {
            return new HttpContext(new SimpleWorkerRequest("/", string.Empty, new StringWriter()));
        }
    }
}
