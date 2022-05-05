FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Kafka_WebAPI/Kafka_WebAPI.csproj", "Kafka_WebAPI/"]
RUN dotnet restore "Kafka_WebAPI/Kafka_WebAPI.csproj"
COPY . .
WORKDIR "/src/Kafka_WebAPI"
RUN dotnet build "Kafka_WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Kafka_WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kafka_WebAPI.dll"]