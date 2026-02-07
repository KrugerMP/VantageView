FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["VantageView.API/VantageView.API.csproj", "VantageView.API/"]
COPY ["VantageView.Data/VantageView.Data.csproj", "VantageView.Data/"]
RUN dotnet restore "VantageView.API/VantageView.API.csproj"

COPY . .
WORKDIR /src/VantageView.API
RUN dotnet publish "VantageView.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5157
ENTRYPOINT ["dotnet", "VantageView.API.dll"]
