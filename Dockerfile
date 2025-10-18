FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Back/Cmdb.csproj", "."]
RUN dotnet restore "./Cmdb.csproj"
COPY Back/. .
WORKDIR "/src/."
RUN dotnet build "./Cmdb.csproj" -c $BUILD_CONFIGURATION -r linux-x64 --self-contained true -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Cmdb.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM node:latest AS buildng
WORKDIR /usr/local/app
COPY Front/ .
RUN npm install
RUN npm run build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=buildng /usr/local/app/dist/cmdb/browser ./wwwroot
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Cmdb.dll"]