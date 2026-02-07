FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["VantageView.Admin.Portal/VantageView.Admin.Portal.csproj", "VantageView.Admin.Portal/"]
RUN dotnet restore "VantageView.Admin.Portal/VantageView.Admin.Portal.csproj"

COPY . .
WORKDIR /src/VantageView.Admin.Portal
RUN dotnet publish "VantageView.Admin.Portal.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5046
ENTRYPOINT ["dotnet", "VantageView.Admin.Portal.dll"]
