# 1. Aşama: Derleme (Build) ortamı
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Proje dosyasını kopyala
COPY ["Backend/DigitalLoanSystem.API/DigitalLoanSystem.API.csproj", "Backend/DigitalLoanSystem.API/"]

# Tüm dosyaları kopyala
COPY . .

# Paketleri geri yükle (Restore)
WORKDIR "/src/Backend/DigitalLoanSystem.API"
RUN dotnet restore "DigitalLoanSystem.API.csproj"

# Projeyi derle
RUN dotnet build "DigitalLoanSystem.API.csproj" -c Release -o /app/build

# 2. Aşama: Yayınlama (Publish)
FROM build AS publish
RUN dotnet publish "DigitalLoanSystem.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 3. Aşama: Çalıştırma (Runtime) ortamı
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Yayınlanan dosyaları çalışma ortamına kopyala
COPY --from=publish /app/publish .

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "DigitalLoanSystem.API.dll"]