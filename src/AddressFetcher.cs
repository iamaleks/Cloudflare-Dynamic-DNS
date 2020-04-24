using System.Net;
using System.IO;
using System.Text;

namespace CloudflareDynamicDNS
{
    /// <summary>
    /// Responsible for fetching the public IP address.
    /// </summary>
    public class AddressFetcher
    {
        private string ipFetchServiceURL = "https://api.ipify.org";

        /// <summary>
        /// Fetch public IPv4 Address.
        /// </summary>
        public string GetPublicIPv4() {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ipFetchServiceURL);
            
            HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
            Stream receiveStream = response.GetResponseStream ();
            StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);

            return readStream.ReadToEnd();
        }
    }
}