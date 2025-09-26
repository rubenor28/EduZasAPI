FROM mcr.microsoft.com/dotnet/sdk:9.0

WORKDIR /workspace

ENV DOTNET_USE_POLLING_FILE_WATCHER=true

RUN dotnet tool install --global dotnet-ef --version 9.*

CMD ["sleep", "infinity"]
