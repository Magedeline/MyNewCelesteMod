FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-ltsc2022 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-ltsc2022 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Source/Desolo_Zantas.csproj", "Source/"]
RUN dotnet restore "Source\Desolo_Zantas.csproj"
COPY . .
WORKDIR "/src/Source"
RUN dotnet build "Desolo_Zantas.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Desolo_Zantas.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Desolo_Zantas.dll"]
