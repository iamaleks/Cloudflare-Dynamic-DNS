# CloudFlare Dynamic DNS Updater

## Intro

This will allow you to update a DNS record on CloudFlare with a newly updated IP address of your external network.

## Parameters


The following lists references to values that are required when running this program.
The following lists references to values that are required when running this program.
* **[APIKey]** - API key issued by CloudFlare in order to access their service.
* **[ZoneID]** - The Zone ID that the record to be updated is located in.
* **[FQDN To Update]** - The FQDN of the record to update, for example ```home.example.com```.
* **Proxy Parameter** - This is a boolean value that specfies whether to proxy though Clouflare.

## Usage

### Docker

#### Simple Method

```
$ docker run -d --name cf_updater -e apitoken='[APIKey]' -e zoneid='[ZoneID]' -e domain='[FQDN To Update]' -e proxy='false' iamaleks/cloudflareupdater:latest
```

#### Manual Build

```
$ git clone https://github.com/iamaleks/Cloudflare-Dynamic-DNS.git
$ cd Cloudflare-Dynamic-DNS
$ docker build -f docker/Dockerfile -t updater .
$ docker run -d --name cf_updater -e apitoken='[APIKey]' -e zoneid='[ZoneID]' -e domain='[FQDN To Update]' -e proxy='false' updater
```

### Command Line

You can run the program using command line arguments or by specifying the values in envirnment variables. This program can be called using Cron or some other scheduling method.

#### Command Line Arguments
```
$ .\CloudflareDynamicDNS.exe --apitoken [APIKey] --zoneid [ZoneID] --domain [FQDN To Update] -p
```

#### Envirnment Variables (Bash)

```
$ export apitoken="[APIKey]"
$ export zoneid="[ZoneID]"
$ export domain="[FQDN To Update]"
$ export proxy="true"
$ ./CloudflareDynamicDNS
```

### Compiling

#### Compile for Specfic Runtime

```
$ dotnet publish -r win-x64 -c Release
```

#### Compile for all Runtimes

```
$ cd src
$ bash build_all_runtimes.sh
```