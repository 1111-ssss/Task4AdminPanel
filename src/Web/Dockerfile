FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Web/Web.csproj", "src/Web/"]
COPY ["src/Business/Business.csproj", "src/Business/"]
COPY ["src/Data/Data.csproj", "src/Data/"]

RUN dotnet restore "src/Web/Web.csproj"

COPY . .

WORKDIR "/src/src/Web"
RUN dotnet build "Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.dll"]