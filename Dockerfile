FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["users-webapi/users-webapi.csproj", "users-webapi/"]
RUN dotnet restore "users-webapi/users-webapi.csproj"
COPY . .
WORKDIR "/src/users-webapi"
RUN dotnet build "users-webapi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "users-webapi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "users-webapi.dll"]