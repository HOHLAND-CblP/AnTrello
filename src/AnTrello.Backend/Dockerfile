FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Trello2.0.Backend/Trello2.0.Backend.csproj", "src/Trello2.0.Backend/"]
RUN dotnet restore "src/Trello2.0.Backend/Trello2.0.Backend.csproj"
COPY . .
WORKDIR "/src/src/Trello2.0.Backend"
RUN dotnet build "Trello2.0.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Trello2.0.Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Trello2.0.Backend.dll"]
