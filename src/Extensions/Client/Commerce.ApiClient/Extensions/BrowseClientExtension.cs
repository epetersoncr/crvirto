﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ApiClient.Utilities;
using VirtoCommerce.Web.Core.Configuration.Catalog;

namespace VirtoCommerce.ApiClient.Extensions
{
    using System.Configuration;

    public static class BrowseClientExtension
    {
        public static BrowseClient CreateBrowseClient(this CommerceClients source, params object[] options)
        {
            var connectionString = string.Format(CatalogConfiguration.Instance.Connection.DataServiceUri, options);
            return CreateBrowseClient(source, connectionString);
        }

        public static BrowseClient CreateBrowseClient(this CommerceClients source, string serviceUrl)
        {
            var connectionString = serviceUrl;
            var subscriptionKey = ConfigurationManager.AppSettings["vc-public-apikey"];
            var client = new BrowseClient(new Uri(connectionString), new AzureSubscriptionMessageProcessingHandler(subscriptionKey, "secret"));
            return client;
        }

        public static BrowseCachedClient CreateBrowseCachedClient(this CommerceClients source, string language = "")
        {
            var session = ClientContext.Session;
            language = String.IsNullOrEmpty(language) ? session.Language : language;
            // http://localhost/admin/api/mp/{0}/{1}/
            var connectionString = ConnectionHelper.GetConnectionString("VirtoCommerce") + String.Format("{0}/{1}/", session.CatalogId, language);
            var subscriptionKey = ConfigurationManager.AppSettings["vc-public-apikey"];
            var client = new BrowseCachedClient(new Uri(connectionString), new AzureSubscriptionMessageProcessingHandler(subscriptionKey, "secret"));
            return client;
        }    
    }
}
