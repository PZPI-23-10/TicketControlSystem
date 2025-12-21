# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["TicketControlSystem.csproj", "./"]
RUN dotnet restore "TicketControlSystem.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TicketControlSystem.csproj" -c Release -o /app/build

# Этап публикации
FROM build AS publish
RUN dotnet publish "TicketControlSystem.csproj" -c Release -o /app/publish

# Этап запуска
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TicketControlSystem.dll"]
