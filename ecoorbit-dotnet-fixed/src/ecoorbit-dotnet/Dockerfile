FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ecoorbit-dotnet/ecoorbit-dotnet.csproj", "src/ecoorbit-dotnet/"]
RUN dotnet restore "src/ecoorbit-dotnet/ecoorbit-dotnet.csproj"
COPY . .
WORKDIR "/src/src/ecoorbit-dotnet"
RUN dotnet build "ecoorbit-dotnet.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ecoorbit-dotnet.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ecoorbit-dotnet.dll"]
