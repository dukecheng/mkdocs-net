FROM mcr.microsoft.com/dotnet/aspnet:7.0
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-.} .
ENTRYPOINT ["dotnet", "Feinian.Mkdocs.Web.dll"]
