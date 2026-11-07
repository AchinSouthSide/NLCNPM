# Kiến trúc và luồng nghiệp vụ

Ứng dụng dùng kiến trúc phân lớp gọn trong một MVC project phù hợp nhóm hai sinh viên. Controller xử lý HTTP và kiểm tra quyền; `IResourceService` xử lý nghiệp vụ; `ApplicationDbContext` truy cập SQL Server; Identity quản lý tài khoản/vai trò; file vật lý lưu ngoài web root. DI được cấu hình trong `Program.cs`.

```mermaid
flowchart LR
  Browser[Browser + Bootstrap] --> MVC[MVC Controllers]
  MVC --> Svc[Resource Service]
  MVC --> Id[ASP.NET Core Identity]
  Svc --> DbCtx[EF Core DbContext]
  Id --> DbCtx
  DbCtx --> SQL[(SQL Server)]
  Svc --> Files[(App_Data/uploads)]
```

## Use case tổng quát

```mermaid
flowchart LR
  Guest[Khách] --> Browse((Duyệt tài liệu công khai))
  Guest --> Search((Tìm kiếm))
  Guest --> Signup((Đăng ký))
  Member[Thành viên] --> Browse
  Member --> Search
  Member --> Download((Tải tài liệu đã duyệt))
  Member --> Upload((Gửi tài liệu))
  Member --> Comment((Bình luận / đánh giá))
  Lecturer[Giảng viên] --> Upload
  Lecturer --> Version((Gửi phiên bản mới))
  Admin[Admin] --> Moderate((Kiểm duyệt tài liệu))
  Admin --> AdminOps((Quản trị hệ thống))
```

## Activity: gửi và duyệt tài liệu

```mermaid
flowchart TD
  A([Bắt đầu]) --> B[Đăng nhập]
  B --> C{Đã xác thực?}
  C -- Không --> Z([Kết thúc])
  C -- Có --> D[Nhập thông tin và chọn tệp]
  D --> E{Định dạng hợp lệ, dưới 25 MB?}
  E -- Không --> D
  E -- Có --> F[Đặt tên GUID, lưu ngoài web root]
  F --> G[Tạo Resource và Version 1 ở trạng thái Pending]
  G --> H[Admin kiểm duyệt]
  H --> I{Quyết định}
  I -- Duyệt --> J[Approved]
  I -- Từ chối --> K[Rejected]
  J --> Z
  K --> Z
```

## Sequence: tải xuống

```mermaid
sequenceDiagram
  actor U as Người dùng
  participant C as ResourcesController
  participant S as ResourceService
  participant DB as SQL Server
  participant FS as File Storage
  U->>C: GET /Resources/Download/{id}
  C->>S: OpenDownloadAsync(id)
  S->>DB: Kiểm tra Resource Approved và version hiện hành
  DB-->>S: Metadata
  S->>FS: Mở file tên GUID
  FS-->>S: Stream
  S->>DB: Ghi download record, tăng bộ đếm
  S-->>C: Stream + content type + tên tải
  C-->>U: Tệp đính kèm
```

## Ma trận quyền rút gọn

| Nghiệp vụ | Khách | Member | Lecturer | Admin |
|---|---:|---:|---:|---:|
| Xem/tìm tài liệu đã duyệt | Có | Có | Có | Có |
| Tải tài liệu đã duyệt | Có | Có | Có | Có |
| Gửi tài liệu, bình luận, đánh giá | Không | Có | Có | Có |
| Gửi phiên bản của tài liệu mình sở hữu | Không | Có | Có | Chưa có UI |
| Kiểm duyệt trạng thái | Không | Không | Không | Có |
| Quản lý người dùng/danh mục | Không | Không | Không | Chưa triển khai |

Kiểm tra quyền thực thi ở Controller bằng `[Authorize]` và `[Authorize(Roles=...)]`; ẩn liên kết UI không phải biện pháp bảo mật.
