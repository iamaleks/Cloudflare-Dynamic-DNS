using System.Net;
using System.IO;
using System;
using System.Collections.Generic;

namespace CloudflareDynamicDNS
{
    public class DNSUpdater
    {
        
        /// <summary>
        /// Zone ID from Cloudflare.
        /// </summary>
        private string dnsZoneID;
        
        /// <summary>
        /// FQDN of DNS entry to update.
        /// </summary>
        private string dnsDomain;
        
        /// <summary>
        /// Authorization header to pass with API request.
        /// </summary>
        private string authorizationHeader;

        enum APIRequestType {
            GetAllDomains, UpdateDomain
        }

        /// <summary>
        /// Dictionary of query types and assoicted URL for API interface.
        /// Values are initlized in the constructor.
        /// </summary>
        Dictionary<APIRequestType, string> apiRequestQueries;

        /// <summary>
        /// Setup Cloudflare DNS Updater.
        /// </summary>
        /// <param name="dnsZoneID">Zone ID from Cloudflare.</param>
        /// <param name="dnsDomain">FQDN of DNS entry to update.</param>
        /// <param name="apiKey">API Key token issued by Cloudflare.</param>
        public DNSUpdater(string dnsZoneID, string dnsDomain, string apiKey) {
            this.dnsZoneID = dnsZoneID;
            this.dnsDomain = dnsDomain;
            this.authorizationHeader = string.Format("Bearer {0}", apiKey);;

            apiRequestQueries = new Dictionary<APIRequestType, string>();
            apiRequestQueries.Add(APIRequestType.GetAllDomains, "https://api.cloudflare.com/client/v4/zones/{0}/dns_records/");
        }

        /// <summary>
        /// Update domain with new IP address.
        /// </summary>
        public void UpdateDomain() {

            Console.WriteLine(GetDomainID());

        }

        private string GetDomainID() {

            string getAllDNSRecordsQuery = string.Format(apiRequestQueries[APIRequestType.GetAllDomains], dnsZoneID);
             
            WebHeaderCollection requestHeaders = new WebHeaderCollection();
            requestHeaders.Add(HttpRequestHeader.Authorization, authorizationHeader);
            requestHeaders.Add(HttpRequestHeader.ContentType, "application/json");

            string requestContents = SendGetRequest(getAllDNSRecordsQuery, requestHeaders);

            return requestContents;
        }

        /// <summary>
        /// Check if external IP address differs from the IP address assoicted with DNS domain.
        /// </summary>
        /// <returns>Boolean</returns>
        private bool IsIPAddressDifferent() {
            return false;
        }

        /// <summary>
        /// Update domain with new IP address
        /// </summary>
        /// <param name="ipAddress">New External IP Address</param>
        private void UpdateIPAddress(string ipAddress) {

        }

        /// <summary>
        /// Send GET request with specfied headers.
        /// </summary>
        /// <param name="url">Formatted API URL path</param>
        /// <param name="requestHeaders">Headers formatted for API access</param>
        /// <returns>String of all contents that is returned</returns>
        private string SendGetRequest(string url, WebHeaderCollection requestHeaders) {

            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "GET";
            request.Headers = requestHeaders;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            var content = string.Empty;
            
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }

            return content;
        }
    }
}