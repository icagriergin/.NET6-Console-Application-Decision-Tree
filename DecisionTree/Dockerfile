FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DecisionTree/DecisionTree.csproj", "DecisionTree/"]
RUN dotnet restore "DecisionTree/DecisionTree.csproj"
COPY . .
WORKDIR "/src/DecisionTree"
RUN dotnet build "DecisionTree.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DecisionTree.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DecisionTree.dll"]
