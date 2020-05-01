using System;
using CommandLine;

namespace CloudflareDynamicDNS
{
    class Program
    {

        public class Options
        {
            [Option('a', "apitoken", Required = true, HelpText = "CloudFlare API Token.")]
            public string APIToken {get; set; }

            [Option('z', "zoneid", Required = true, HelpText = "CloudFlare Zone ID.")]
            public string ZoneID {get; set; }

            [Option('d', "domain", Required = true, HelpText = "Domain to update in Zone.")]
            public string DomainName {get; set; }

            [Option('p', "proxy", Required = false, HelpText = "Proxy through CloudFlare option.")]
            public bool Proxy { get; set; }
        }

        static void Main(string[] args)
        {
            // Get public IP address
            AddressFetcher addressFetcher = new AddressFetcher();
            string currentIPAddress = addressFetcher.GetPublicIPv4();

            Options commandLineOptions = null;

           if (args.Length > 0) {

               // Parse command line arguments
               
               Parser.Default.ParseArguments<Options>(args).WithParsed(o => {
                   commandLineOptions = o;
               });

           } 
           else {
               // Parse envirnment variables

               commandLineOptions = new Options();

               commandLineOptions.APIToken = Environment.GetEnvironmentVariable("apitoken");
               commandLineOptions.ZoneID = Environment.GetEnvironmentVariable("zoneid");
               commandLineOptions.DomainName = Environment.GetEnvironmentVariable("domain");

               if (Environment.GetEnvironmentVariable("proxy").ToLower().Equals("true")) {
                   commandLineOptions.Proxy = true;
               } 
               else {
                   commandLineOptions.Proxy = false;
               }
           }

           // Perform update
           DNSUpdater dnsUpdater = new DNSUpdater(commandLineOptions.ZoneID, commandLineOptions.DomainName, commandLineOptions.APIToken, currentIPAddress, commandLineOptions.Proxy);
           dnsUpdater.UpdateDomain();
        }
    }
}
