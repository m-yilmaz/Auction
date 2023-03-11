#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Auction.Sourcing/Auction.Sourcing.csproj", "Auction.Sourcing/"]
COPY ["src/Common/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "src/Common/EventBusRabbitMQ/"]
RUN dotnet restore "Auction.Sourcing/Auction.Sourcing.csproj"
COPY . .
WORKDIR "/src/Auction.Sourcing"
RUN dotnet build "Auction.Sourcing.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Auction.Sourcing.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auction.Sourcing.dll"]