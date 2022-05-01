FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
LABEL cache=true
ARG VERSION
ENV VERSION=${VERSION:-2.1.0}
WORKDIR /app
COPY . .
RUN echo 'Building $VERSION'
RUN dotnet build ./CinemaApi -p:Version=$VERSION --output /app/build

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
COPY --from=build /app/build /opt/cinema

EXPOSE 85
ENTRYPOINT dotnet /opt/cinema/CinemaApi.dll
