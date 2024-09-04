using Azure.Identity;
using Microsoft.Graph.Beta;
using Microsoft.Graph.Beta.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppThreatHuntAPI
{
    public class GraphManager
    {
        public async Task<List<AppInstance>> ListApplications()
        {
            var AppList = new List<Application>();
            var AppInstanceList = new List<AppInstance>();
            var APPPackageList = await ListAPP();
            // Initialize the GraphServiceClient with DefaultAzureCredential
            var graphClient = new GraphServiceClient(new DefaultAzureCredential());

            // Fetch the first page of applications
            var applicationsPage = await graphClient.Applications
                                                    .GetAsync();

            if (applicationsPage != null && applicationsPage.Value != null)
            {
                // Process the first page of applications
                AppList.AddRange(applicationsPage.Value);

                // Continue fetching and processing additional pages if present
                while (applicationsPage.OdataNextLink != null)
                {
                    // Use the OdataNextLink to fetch the next page
                    applicationsPage = await graphClient.Applications
                                                        .WithUrl(applicationsPage.OdataNextLink)
                                                        .GetAsync();
                    if (applicationsPage != null && applicationsPage.Value != null)
                    {
                        AppList.AddRange(applicationsPage.Value);
                    }
                }
            }
            foreach (var item in AppList)
            {
                var appInstance = new AppInstance
                {
                    AppId  = item.AppId,
                    AppName = item.DisplayName,
                    IsROPC = item.IsFallbackPublicClient == true,
                    IsSecret= ((item.PasswordCredentials != null && item.PasswordCredentials.Count > 0)),
                    IsIntuneAppProtectionInScope = IsMobileBased(item),   
                    IsIntuneAppProtection = IsAPPEnabled(item, APPPackageList),

                };
                AppInstanceList.Add(appInstance);
            }
            return AppInstanceList;
        }
        public async Task<List<string>> ListAPP()
        {
            var AppList = new List<ManagedAppPolicy>();
            var packageList = new List<string>();
            // Initialize the GraphServiceClient with DefaultAzureCredential
            var graphClient = new GraphServiceClient(new DefaultAzureCredential());
            // Fetch the first page of applications
            var policyList = await graphClient.DeviceAppManagement.AndroidManagedAppProtections
                                                    .GetAsync(requestConfiguration =>
                                                    {
                                                        requestConfiguration.QueryParameters.Expand = new string[] { "apps" };
                                                    });

            if (policyList != null && policyList.Value != null)
            {
                // Process the first page of applications
                AppList.AddRange(policyList.Value);

                // Continue fetching and processing additional pages if present
                while (policyList.OdataNextLink != null)
                {
                    // Use the OdataNextLink to fetch the next page
                    policyList = await graphClient.DeviceAppManagement.AndroidManagedAppProtections
                                                        .WithUrl(policyList.OdataNextLink)
                                                        .GetAsync();
                    if (policyList != null && policyList.Value != null)
                    {
                        AppList.AddRange(policyList.Value);
                    }
                }
            }
            foreach (var app in AppList)
            {
                var appList = app.GetType().GetProperty("Apps").GetValue(app, null);
                List<ManagedMobileApp> mList = appList as List<ManagedMobileApp>;

                foreach (var item in mList)
                {
                    var package = (string)item.MobileAppIdentifier.GetType().GetProperty("PackageId").GetValue(item.MobileAppIdentifier, null);
                    packageList.Add(package);
                }
                // If the 'apps' property exists in the response, it will be deserialized into the customPolicy.Apps
            }
            return packageList;
        }
        public bool IsAPPEnabled(Application app, List<string> packageList)
        {
            bool bRet = false;
            if (app.PublicClient.RedirectUris != null && app.PublicClient.RedirectUris.Count > 0)
            {
                bool isMobile = false;
                foreach (var uri in app.PublicClient.RedirectUris)
                {
                    isMobile = CheckMobileRedirectUri(uri);
                    if (isMobile)
                    {
                        foreach (var item in packageList)
                        {
                            if (uri.Contains(item))
                            {
                                bRet = true;
                                break;
                            }
                        }
                        if (bRet) break;
                    }
                }
            }
            return bRet;
        }
        public bool IsMobileBased(Application app)
        {
            bool bRet = false;
            if (app.PublicClient.RedirectUris != null && app.PublicClient.RedirectUris.Count > 0)
            {
                bool isMobile = false;
                foreach (var uri in app.PublicClient.RedirectUris)
                {
                    isMobile = CheckMobileRedirectUri(uri);
                    if (isMobile)
                    {
                       bRet = true;
                        break;
                    }
                }
            }
            return bRet;
        }
        bool CheckMobileRedirectUri(string schema)
        {
            string url = schema;

            string pattern1 = @"^(?<scheme>[a-zA-Z][a-zA-Z0-9+.-]*):\/\/(?:auth)?$";
            string Pattern2 = @"^(?!(http|https|ms-appx-web):\/\/)(?<scheme>[a-zA-Z][a-zA-Z0-9+.-]*):\/\/";

            // Match the regex with the URL
            var match1 = Regex.Match(url, pattern1);
            var match2 = Regex.Match(url, Pattern2);

            return match1.Success || match2.Success;
        }
    }
}
