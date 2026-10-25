FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . ./
RUN dotnet restore AdmissionWeb.csproj
RUN dotnet publish AdmissionWeb.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
RUN groupadd --system app && useradd --system --gid app --home-dir /app app \
    && mkdir -p /app/App_Data && chown -R app:app /app
COPY --from=build --chown=app:app /app/publish ./
ENV ASPNETCORE_URLS=http://+:10000 \
    ASPNETCORE_FORWARDEDHEADERS_ENABLED=true \
    ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000
USER app
ENTRYPOINT ["dotnet", "AdmissionWeb.dll"]
