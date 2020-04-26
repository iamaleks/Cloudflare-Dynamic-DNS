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
        /// Zone ID from CloudFlare
        /// </summary>
        private string dnsZoneID;

        /// <summary>
        /// New public IP address to update domain with.
        /// </summary>
        private string newPublicAddress;

        /// <summary>
        /// API Request Types.
        /// </summary>
        enum APIRequestType {
            GetAllDomains, UpdateDomain, GetSpecificDomain
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
        /// <param name="newPublicAddress">New public IP address to update the domain with.</param>
        public DNSUpdater(string dnsZoneID, string dnsDomain, string apiKey, string newPublicAddress) {
            this.dnsDomain = dnsDomain;
            this.authorizationHeader = string.Format("Bearer {0}", apiKey);
            this.newPublicAddress = newPublicAddress;
            this.dnsZoneID = dnsZoneID;

            apiRequestQueries = new Dictionary<APIRequestType, string>();
            apiRequestQueries.Add(APIRequestType.GetAllDomains, "https://api.cloudflare.com/client/v4/zones/{0}/dns_records/");
            apiRequestQueries.Add(APIRequestType.UpdateDomain, "https://api.cloudflare.com/client/v4/zones/{0}/dns_records/{1}");
            apiRequestQueries.Add(APIRequestType.GetSpecificDomain, "https://api.cloudflare.com/client/v4/zones/{0}/dns_records/{1}");
        }

        /// <summary>
        /// Update domain with new IP address.
        /// </summary>
        public void UpdateDomain() {

            // Get ID of domain
            string domainID = GetDomainID();

            // Check if an update is needed, if so perform the update
            if (IsIPAddressDifferent(domainID)) {
                UpdateIPAddress(domainID);
            }
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
            string requestURL = string.Format(apiRequestQueries[APIRequestType.GetAllDomains], dnsZoneID);
            string requestContents = SendGetRequest(requestURL, requestHeaders);

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
        /// <param name="domainID">ID of the record to check.</param>
        /// <returns></returns>
        private bool IsIPAddressDifferent(string domainID) {

            // Setup headers
            WebHeaderCollection requestHeaders = new WebHeaderCollection();
            requestHeaders.Add(HttpRequestHeader.Authorization, authorizationHeader);
            requestHeaders.Add(HttpRequestHeader.ContentType, "application/json");

            // Send API request
            string requestURL = string.Format(apiRequestQueries[APIRequestType.GetSpecificDomain], dnsZoneID, domainID);
            string requestContents = SendGetRequest(requestURL, requestHeaders);

            // Create object out of JSON
            dynamic deserializedJsonObject = JsonConvert.DeserializeObject(requestContents);

            // Check current IP address in record against new public IP address
            if (newPublicAddress.Equals(deserializedJsonObject.result["content"])) {
                return false;
            }
            else {
                return true;
            }
        }

        /// <summary>
        /// Update domain with new IP address
        /// </summary>
        /// <param name="domainID">ID of record to update</param>
        private void UpdateIPAddress(string domainID) {
                // Setup headers
                WebHeaderCollection requestHeaders = new WebHeaderCollection();
                requestHeaders.Add(HttpRequestHeader.Authorization, authorizationHeader);
                requestHeaders.Add(HttpRequestHeader.ContentType, "application/json");

                // Format post data 
                string postData = string.Format("{{\"type\":\"A\",\"name\":\"{0}\",\"content\":\"{1}\",\"ttl\":1}}", dnsDomain, newPublicAddress); 

                // Send API request
                string requestURL = string.Format(apiRequestQueries[APIRequestType.UpdateDomain], dnsZoneID, domainID);
                SendPutRequest(requestURL, requestHeaders, postData);
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

        private void SendPutRequest(string url, WebHeaderCollection requestHeaders, string postData) {
            
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "PUT";
            request.Headers = requestHeaders;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(postData);
            }

            var httpResponse = (HttpWebResponse)request.GetResponse();

            //return httpResponse.ToString();
            // TODO return both HTTP return code and JSON in order to double check everything
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