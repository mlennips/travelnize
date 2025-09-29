FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish LIT.Travelnize/LIT.Travelnize.csproj -c Release -o /dist

FROM nginx:alpine
RUN rm -f /etc/nginx/conf.d/default.conf
COPY LIT.Travelnize/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /dist/wwwroot /usr/share/nginx/html