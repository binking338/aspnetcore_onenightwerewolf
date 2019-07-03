FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

# copy everything then restore and build app
COPY ./ ./
RUN dotnet restore

WORKDIR /app/OneNightWerewolf.Web
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "OneNightWerewolf.Web.dll"]