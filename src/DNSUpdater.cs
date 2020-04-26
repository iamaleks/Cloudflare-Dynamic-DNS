using System.Net;
using System.IO;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CloudflareDynamicDNS
{
    public class DNSUpdater
    {        
        /// <summary>
        /// FQDN of DNS entry to update.
        /// </summary>
        private string dnsDomain;
        
        /// <summary>
        /// Authorization header to pass with API request.
        /// </summary>
        private string authorizationHeader;

        /// <summary>
        /// API Request Types.
        /// </summary>
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
            this.dnsDomain = dnsDomain;
            this.authorizationHeader = string.Format("Bearer {0}", apiKey);;

            apiRequestQueries = new Dictionary<APIRequestType, string>();
            apiRequestQueries.Add(APIRequestType.GetAllDomains, string.Format("https://api.cloudflare.com/client/v4/zones/{0}/dns_records/", dnsZoneID));
        }

        /// <summary>
        /// Update domain with new IP address.
        /// </summary>
        public void UpdateDomain() {

            Console.WriteLine(GetDomainID());

        }

        /// <summary>
        /// Fetch the ID associated with the FQDN (dnsDomain) passed to the instance.
        /// </summary>
        /// <returns>
        /// Returns the ID in the form of a string.
        /// </returns>
        private string GetDomainID() {

            // Setup headers
            WebHeaderCollection requestHeaders = new WebHeaderCollection();
            requestHeaders.Add(HttpRequestHeader.Authorization, authorizationHeader);
            requestHeaders.Add(HttpRequestHeader.ContentType, "application/json");

            // Send API request
            string requestContents = SendGetRequest(apiRequestQueries[APIRequestType.GetAllDomains], requestHeaders);

            // Create object out of JSON
            dynamic deserializedJsonObject = JsonConvert.DeserializeObject(requestContents);

            // Get the amount of times the domain occurs
            int occurCount = 0;

            foreach (var dnsEntry in deserializedJsonObject.result) {
                if (dnsEntry.name == dnsDomain) {
                    occurCount++;
                }
            }

            if (occurCount == 0) {
                // Error, the domain does not exist in the DNS zone
                throw new DomainDoesNotExist("Domain entered does not exist in the current DNS zone within CloudFlare.");
            }
            else if (occurCount > 1) {
                // Error, this domain shows up more than once in the DNS zone
                throw new DomainOccursMoreThanOnce("Domain entered occurs more than once in the current DNS zone within CloudFlare");
            }

            // Find the ID of the domain
            string currentDomainID = string.Empty;

            foreach (var dnsEntry in deserializedJsonObject.result) {
                if (dnsEntry.name == dnsDomain) {
                    currentDomainID = dnsEntry.id;
                }
            }

            return currentDomainID;
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

    /// <summary>
    /// A domain entry does not exist within the DNS Zone
    /// </summary>
    class DomainDoesNotExist : Exception {

        public DomainDoesNotExist() {

        }

        public DomainDoesNotExist(string message) : base(message) {

        }
    }

    /// <summary>
    /// There is more than 1 instance of a domain entered within CloudFlare
    /// </summary>
    class DomainOccursMoreThanOnce : Exception {

        public DomainOccursMoreThanOnce() {
            
        }

        public DomainOccursMoreThanOnce(string message) : base(message) {

        }
    }
}