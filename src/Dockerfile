FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DotnetBackendSkeleton.csproj", "./"]
RUN dotnet restore "DotnetBackendSkeleton.csproj"
COPY . .
RUN dotnet publish "DotnetBackendSkeleton.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DotnetBackendSkeleton.dll"]
