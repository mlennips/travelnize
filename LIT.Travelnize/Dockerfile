FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish LIT.Travelnize/LIT.Travelnize.csproj -c Release -o /dist

FROM caddy:alpine
# Caddyfile hinein kopieren
COPY LIT.Travelnize/Caddyfile /etc/caddy/Caddyfile
# Statische Blazor Files
COPY --from=build /dist/wwwroot /srv