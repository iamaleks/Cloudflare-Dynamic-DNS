# CloudFlare Dynamic DNS Updater

## Intro

This will allow you to update a DNS record on CloudFlare with a newly updated IP address of your external network.

## Usage

### Docker

### Command Line

You can run the program using command line arguments or by specifying the values in envirnment variables.

#### Command Line Arguments
```
$ ./CloudflareDynamicDNS -a [APIKey] -z [ZoneID] -d [FQDN To Update] -p
```

#### Envirnment Variables

```
$ export apitoken="[APIKey]"
$ export zoneid="[ZoneID]"
$ export domain="[FQDN To Update]"
$ export proxy="true"
$ ./CloudflareDynamicDNS
```
