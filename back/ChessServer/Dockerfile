# ���������� ����������� ����� .NET SDK ��� ������ ����������
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# �������� CSPROJ � ��������������� �����������
COPY *.csproj ./
RUN dotnet restore

# �������� ��� ����� � �������� ����������
COPY . ./
RUN dotnet publish -c Release -o out

# ���������� ����������� runtime-����� ��� ������� ����������
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .

# ����������� ���� 80
EXPOSE 5058 7208

# ��������� ����������
ENTRYPOINT ["dotnet", "ChessServer.dll"]
