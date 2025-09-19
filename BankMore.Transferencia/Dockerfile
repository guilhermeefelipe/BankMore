FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
    WORKDIR /src
    COPY src/BankMore.Transferencia.sln .

    COPY src/*/*.csproj ./
    RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done

    RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages\ 
            dotnet restore BankMore.Transferencia.sln

FROM restore AS build
    ARG Version=1.0.0.0
    COPY src/ .
    RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages\ 
            dotnet build BankMore.Transferencia.sln -c Release /p:Version=$Version --no-restore

FROM build AS publish
    ARG Version=1.0.0.0
    RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages\ 
            dotnet publish /p:Version=$Version -c Release -o /app --no-build --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
    RUN unlink /etc/localtime && ln -s /usr/share/zoneinfo/Brazil/East /etc/localtime
    WORKDIR /app
    COPY --from=publish /app .

    USER $APP_UID
    EXPOSE 8080
    EXPOSE 8081

    ENTRYPOINT ["dotnet", "BankMore.Transferencia.Api.dll"]