# MY Academy IdentityMail Projesi
ASP.NET Core MVC ve ASP.NET Core Identity ile geliştirilmiş, kullanıcıların birbirine mesaj gönderebildiği bir iç mesajlaşma (mail benzeri) web uygulamasıdır. Rol tabanlı yetkilendirme, taslak yönetimi, kategori sistemi ve yönetici paneli içerir.
## Özellikler
### Kullanıcı
- Kayıt olma, giriş yapma ve çıkış yapma
- Türkçe hata mesajları (özelleştirilmiş `IdentityErrorDescriber`)
- **Gelen kutusu:** okundu/okunmadı filtreleme ve sayfalama
- **Gönderilenler** ve **Taslaklar** listeleri
- Mesaj yazma, taslağa kaydetme ve taslağı düzenleyip gönderme
- Mesajı önemli olarak işaretleme
- Mesaj detay sayfası (açıldığında otomatik "okundu" olur)
- Mesajlara kategori atama (isteğe bağlı)
- Menüde okunmamış mesaj sayısı göstergesi
- Profil yönetimi: kullanıcı bilgilerini güncelleme ve şifre değiştirme
### Yönetici (Admin)
- Kullanıcı listesi ve her kullanıcının gönderdiği mesaj sayısı
- Mesaj istatistikleri
- Kategori yönetimi: ekleme, aktif/pasif yapma (pasif kategoriler yeni mesajlarda seçilemez, eski mesajlarda görünmeye devam eder)

## Kullanılan Teknolojiler
 
- ASP.NET Core MVC
- Entity Framework Core (Code First)
- ASP.NET Core Identity
- SQL Server
- Bootstrap

## Güvenlik Notları
 
- Kimlik doğrulama için cookie tabanlı Identity kullanılır; `SecurityStamp` her istekte doğrulanır.
- Controller'lar `[Authorize(Roles = "...")]` ile korunur. Yönetim sayfalarına yalnızca `Admin` rolü erişebilir.
- Formlar DTO'lar üzerinden bağlanır; böylece fazla veri gönderimi (over-posting) engellenir.
- Yönlendirmelerde `returnUrl` için `Url.IsLocalUrl` kontrolü yapılır (open redirect koruması).
- Kullanıcı daima `GetUserAsync(User)` ile alınır; kullanıcı adı değişse bile oturum doğru kullanıcıyı tanır.


## Kurulum
### Gereksinimler
- .NET SDK (projenin hedeflediği sürüm)
- SQL Server (LocalDB veya tam sürüm)
- Visual Studio 2022 veya VS Code

### Adımlar
1. Depoyu klonlayın:
```bash
   git clone https://github.com/yunusemrekartal/MyAcademy_IdentityMailProject
   cd IdentityMail
```
2. `appsettings.json` içindeki bağlantı cümlesini kendi SQL Server bilgilerinize göre düzenleyin:
```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.;Database=IdentityMailDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
```
3. Veritabanını oluşturun:
   Package Manager Console:
```
   Update-Database
```
   veya .NET CLI:
```bash
   dotnet ef database update
```
 
4. Uygulamayı çalıştırın:
```bash
   dotnet run
```
Uygulama ilk açıldığında `DbSeeder` sınıfı `Admin` ve `User` rollerini ve varsayılan yönetici hesabını otomatik oluşturur.
 
## Varsayılan Yönetici Hesabı
 
| Alan | Değer |
|---|---|
| E-posta | `admin@identitymail.com` |
| Şifre | `Admin123!` |
