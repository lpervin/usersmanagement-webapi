FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["users-webapi-tests/users-webapi-tests.csproj", "users-webapi-tests/"]
COPY ["users-webapi/users-webapi.csproj", "users-webapi/"]
RUN dotnet restore "users-webapi-tests/users-webapi-tests.csproj"
COPY . .
WORKDIR "/src/users-webapi-tests"
RUN dotnet build "users-webapi-tests.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "users-webapi-tests.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "users-webapi-tests.dll"]
