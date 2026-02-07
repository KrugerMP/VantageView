FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["VantageView.Frontend/VantageView.Frontend.csproj", "VantageView.Frontend/"]
RUN dotnet restore "VantageView.Frontend/VantageView.Frontend.csproj"

COPY . .
WORKDIR /src/VantageView.Frontend
RUN dotnet publish "VantageView.Frontend.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5059
ENTRYPOINT ["dotnet", "VantageView.Frontend.dll"]
