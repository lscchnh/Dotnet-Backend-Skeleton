FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BackendSkeleton.csproj", "./"]
RUN dotnet restore "BackendSkeleton.csproj"
COPY . .
RUN dotnet publish "BackendSkeleton.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BackendSkeleton.dll"]
