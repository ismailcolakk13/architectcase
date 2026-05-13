# Digital Loan & Repayment Management System

Bu proje, modern bir bankacılık kredi yönetim sisteminin uçtan uca simülasyonudur. Müşterilerin kredi başvurusu yapabildiği, taksitlerini takip edip ödeyebildiği; banka personelinin ise müşterileri ve kredi onay süreçlerini yönetebildiği full-stack bir çözüm sunar.

## 🚀 Teknolojiler

### Backend
- **.NET 8 Web API** (C#)
- **Entity Framework Core** (ORM)
- **MySQL** (Veritabanı)
- **Clean Architecture** (Core, Application, Infrastructure, API)

### Frontend
- **React 18** (Vite)
- **Vanilla CSS** (Özel modern tasarım)
- **React Router Dom** (Yönlendirme)

## ✨ Öne Çıkan Özellikler

- **Müşteri Yönetimi:** Tam kapsamlı müşteri kayıt, düzenleme ve silme işlemleri.
- **Kredi Başvuru İş Akışı:**
  - Kategori bazlı (İhtiyaç, Konut, vb.) sabit faiz ve vade oranları.
  - Backend tarafında parametre doğrulaması.
  - Kredi skoru kontrolü (1000 puan altı otomatik red).
  - **Onay Bekliyor** statüsü ile başlayan manuel admin onay mekanizması.
- **Finansal Mantık:**
  - Otomatik aylık taksit planı oluşturma (Anapara + Faiz hesaplamalı).
  - Müşteri bakiye sistemi üzerinden taksit ödeme.
  - Yetersiz bakiye durumunda ödeme engelleme.
  - Borçların sadece "Aktif" krediler üzerinden hesaplanması.
- **Dashboard & Analiz:**
  - Müşteriler için toplam borç, kalan anapara ve gecikmiş taksit takibi.
  - Adminler için tüm kredi ve ödeme geçmişine genel bakış.

## 🛠️ Kurulum

### 1. Veritabanı
MySQL üzerinde `DigitalLoanDb` adında bir veritabanı oluşturun ve `Backend/DigitalLoanSystem.API/appsettings.json` içerisindeki bağlantı dizesini güncelleyin.

```bash
# Migration'ları uygulayın
cd Backend
dotnet ef database update --project DigitalLoanSystem.Infrastructure --startup-project DigitalLoanSystem.API
```

### 2. Backend Çalıştırma
```bash
cd Backend/DigitalLoanSystem.API
dotnet run
```

### 3. Frontend Çalıştırma
```bash
cd Frontend
npm install
npm run dev
```

## 📂 Proje Yapısı

- `DigitalLoanSystem.Core`: Entity'ler, Enum'lar ve Interface'ler (Bağımsız çekirdek).
- `DigitalLoanSystem.Application`: İş mantığı, Servisler ve DTO'lar.
- `DigitalLoanSystem.Infrastructure`: Veritabanı context'i, Repository'ler ve Dış servis simülasyonları.
- `DigitalLoanSystem.API`: HTTP Endpoint'leri ve Controller'lar.
- `Frontend`: React uygulaması ve CSS modern tasarım sistemi.

## 📝 Test Verileri (Mock)
Proje kök dizinindeki `mock_data.sql` dosyasını veritabanınızda çalıştırarak örnek müşteri ve kredi verilerini anında yükleyebilirsiniz.
- Ahmet Yılmaz (12345678901) - 25.000 TL Bakiye
- Mehmet Kaya (34567890123) - 500 TL Bakiye (Bakiye yetersiz testi için)
