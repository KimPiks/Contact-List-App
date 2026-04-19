FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
RUN apt-get update \
	&& apt-get install -y --no-install-recommends curl \
	&& rm -rf /var/lib/apt/lists/*
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/NetPC.Api/NetPC.Api.csproj", "src/NetPC.Api/"]
COPY ["src/NetPC.Infrastructure/NetPC.Infrastructure.csproj", "src/NetPC.Infrastructure/"]
COPY ["src/NetPC.Domain/NetPC.Domain.csproj", "src/NetPC.Domain/"]
COPY ["src/NetPC.Application/NetPC.Application.csproj", "src/NetPC.Application/"]
RUN dotnet restore "src/NetPC.Api/NetPC.Api.csproj"
RUN dotnet restore "src/NetPC.Infrastructure/NetPC.Infrastructure.csproj"
RUN dotnet restore "src/NetPC.Domain/NetPC.Domain.csproj"
RUN dotnet restore "src/NetPC.Application/NetPC.Application.csproj"
COPY . .
WORKDIR "/src/src/NetPC.Api"
RUN dotnet build "NetPC.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet ef migrations bundle --self-contained -r linux-x64 -o /app/publish/efbundle --project "../NetPC.Infrastructure/NetPC.Infrastructure.csproj" --startup-project "NetPC.Api.csproj" --configuration $BUILD_CONFIGURATION

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NetPC.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetPC.Api.dll"]
