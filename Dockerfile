FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore .
WORKDIR "/src/Web"
RUN dotnet build "Web.csproj" -c Release -o /app

FROM build AS publish
WORKDIR "/src/Web"
RUN dotnet publish "Web.csproj" --no-restore -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Web.dll"]