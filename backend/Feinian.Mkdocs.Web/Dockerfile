FROM hub.feinian.net/dotnet/aspnet:8.0
ARG source
WORKDIR /app
EXPOSE 8080
COPY ${source:-.} .
ENTRYPOINT ["dotnet", "Feinian.Mkdocs.Web.dll"]
