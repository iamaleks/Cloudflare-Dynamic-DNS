#!/bin/sh

cd /src ; dotnet publish -c Release -o out
chmod +x /src/out/CloudflareDynamicDNS

while true
do
	echo "Running Update Program"
	cd /src/out ; ./CloudflareDynamicDNS
	sleep 60
done

