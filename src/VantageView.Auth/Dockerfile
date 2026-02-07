FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["VantageView.Auth/VantageView.Auth.csproj", "VantageView.Auth/"]
COPY ["VantageView.Data/VantageView.Data.csproj", "VantageView.Data/"]
RUN dotnet restore "VantageView.Auth/VantageView.Auth.csproj"

COPY . .
WORKDIR /src/VantageView.Auth
RUN dotnet publish "VantageView.Auth.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5098
ENTRYPOINT ["dotnet", "VantageView.Auth.dll"]
