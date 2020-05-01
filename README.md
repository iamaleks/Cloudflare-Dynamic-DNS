# CloudFlare Dynamic DNS Updater

## Intro

This will allow you to update a DNS record on CloudFlare with a newly updated IP address of your external network.

## Usage

### Docker

#### Simple Method

```
$ docker run -d --name uptest -e apitoken='[APIKey]' -e zoneid='[ZoneID]' -e domain='[FQDN To Update]' -e proxy='false' docker.pkg.github.com/iamaleks/cloudflare-dynamic-dns/cloudflareupdater:current

```

#### Manual Build

```
$ git clone https://github.com/iamaleks/Cloudflare-Dynamic-DNS.git
$ cd Cloudflare-Dynamic-DNS
$ docker build -f docker/Dockerfile -t updater .
$ docker run -d --name uptest -e apitoken='[APIKey]' -e zoneid='[ZoneID]' -e domain='[FQDN To Update]' -e proxy='false' updater
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
