# NLCNPM — Quản lý và chia sẻ tài nguyên học tập

Ứng dụng ASP.NET Core MVC, EF Core, SQL Server và ASP.NET Core Identity cho đề tài NLCNPM23CS-06.

## Chạy ứng dụng

Yêu cầu .NET 10 SDK, Visual Studio ASP.NET workload và SQL Server LocalDB. Kiểm tra `ConnectionStrings:DefaultConnection` trong `appsettings.json` (mặc định là LocalDB), sau đó chạy `dotnet tool restore`, `dotnet restore`, `dotnet ef database update` và `dotnet run`. Development tự áp dụng migrations và tạo ba role `Admin`, `Lecturer`, `Member`. Tạo tài khoản đăng ký đầu tiên rồi cấp role Admin bằng SQL Server Object Explorer (bảng `AspNetUserRoles`); không có mật khẩu quản trị mặc định trong mã nguồn.

## Luồng đã triển khai

- Đăng ký, đăng nhập, đăng xuất bằng Identity; lockout sau đăng nhập lỗi.
- Tìm kiếm theo tiêu đề/mô tả, xem chi tiết, phân trang.
- Đăng tệp PDF/DOCX/PPTX/XLSX/TXT tối đa 25 MB; đổi tên GUID, lưu ngoài `wwwroot`, trạng thái chờ duyệt.
- Tải xuống chỉ tài liệu đã duyệt; ghi lượt tải và nhật ký cơ bản.
- Gửi phiên bản mới mà không xóa file cũ; gửi lại kiểm duyệt.
- Admin duyệt, từ chối, ẩn hoặc lưu trữ.
- Bình luận và điểm 1–5; một đánh giá hiện hành cho mỗi người/tài liệu.
- SQL Server schema qua EF migrations.

## An toàn vận hành

- Không đặt secrets production trong appsettings; dùng User Secrets/biến môi trường.
- File uploads không được serve tĩnh. Trước triển khai thật cần bổ sung xác minh signature/MIME và malware scanning.
- Razor encode HTML; MVC forms có antiforgery; EF Core truy vấn tham số hóa.
- Backup cả SQL Server và `App_Data/uploads`; thực hiện restore thử ở môi trường khác.

## Tài liệu

- [Kiến trúc và luồng](docs/architecture.md)
- [Cơ sở dữ liệu](docs/database.md)
- [Yêu cầu và kiểm thử](docs/requirements-and-tests.md)
- [Bàn giao và prompt phiên tiếp theo](docs/HANDOFF_NEXT_SESSION.md)

## Chưa triển khai

Danh mục môn/chủ đề/năm học/tag, quên mật khẩu email, hồ sơ, quản lý role và chỉnh sửa người dùng, báo cáo vi phạm đầy đủ, UI sao lưu, cùng kiểm thử tự động vẫn là phần việc tiếp theo. Dashboard số liệu cơ bản và khóa/mở tài khoản đã có.
