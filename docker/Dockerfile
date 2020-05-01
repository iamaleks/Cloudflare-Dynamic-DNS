FROM mcr.microsoft.com/dotnet/core/sdk:3.1

COPY ./docker/entry.sh /
COPY ./src /src

ENTRYPOINT ["sh", "/entry.sh"]