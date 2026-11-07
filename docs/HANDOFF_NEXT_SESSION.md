# Handoff cho phiên tiếp theo

Cập nhật lần cuối: 07/11/2026.

## Trạng thái dự án

- Repo: https://github.com/AchinSouthSide/NLCNPM (public), nhánh `main`.
- Project ASP.NET Core MVC trên .NET 10; EF Core SQL Server; Identity; Bootstrap 5.
- `dotnet build --no-restore` đã thành công ở phiên trước: 0 warning, 0 error.
- Hai migration đã được áp dụng thành công vào LocalDB `NLCNPM` trên máy phát triển.
- Chưa có project kiểm thử tự động. Test cases trong `docs/requirements-and-tests.md` mới là kịch bản đề xuất, chưa chạy và chưa có kết quả Pass.
- Kiến trúc hiện là MVC project gọn: Controller → Service → EF Core DbContext; file nằm ngoài `wwwroot` trong `App_Data/uploads`.

## Đã triển khai

- Identity: đăng ký, đăng nhập, đăng xuất, mật khẩu hash, lockout; role `Admin`, `Lecturer`, `Member` được tạo ở môi trường Development.
- Tài nguyên: tạo tài liệu, danh sách, tìm tiêu đề/mô tả, phân trang, chi tiết; trạng thái Pending/Approved/Rejected/Hidden/Archived.
- Upload PDF/DOCX/PPTX/XLSX/TXT tối đa 25 MB; đặt tên GUID và lưu ngoài web root.
- Tải tài liệu chỉ khi Approved; tăng bộ đếm và tạo DownloadRecord.
- Tạo phiên bản mới không ghi đè file cũ; gửi phiên bản mới sẽ chuyển tài nguyên về Pending.
- Bình luận và đánh giá 1–5; mỗi người chỉ có một rating hiện hành trên mỗi tài nguyên.
- Admin có trang kiểm duyệt, dashboard thống kê cơ bản, danh sách tài khoản và khóa/mở khóa.
- Có migrations, hướng dẫn chạy và các tài liệu kiến trúc/ERD/test plan ở thư mục `docs`.

## Phần còn thiếu so với yêu cầu đồ án

### Chức năng ứng dụng

- FR-04: quên/đặt lại mật khẩu, token khôi phục và gửi email.
- FR-05: hồ sơ cá nhân, đổi mật khẩu, lịch sử hoạt động cá nhân.
- FR-06–07: tạo/sửa/khóa người dùng đầy đủ; gán role qua UI; quản lý permission chi tiết. Hiện mới có khóa/mở tài khoản; role được tạo ở Development nhưng chưa có màn hình quản lý.
- FR-08–11, FR-22: sửa/xóa tài nguyên theo chính sách; môn học, chủ đề, năm học, mức độ, loại tài liệu, tag và quan hệ của chúng.
- FR-12: lọc kết hợp theo môn/chủ đề/năm/mức độ/loại/tag/người đăng; hiện chỉ tìm tên và mô tả.
- FR-14/17–18: lịch sử tải theo người dùng và tải version cụ thể; trang lịch sử phiên bản; sửa cách ghi DownloadRecord hiện gán user `anonymous` do service chưa nhận userId.
- FR-15: sửa/xóa bình luận theo quyền và công cụ kiểm duyệt bình luận.
- FR-19: form báo cáo vi phạm, danh sách xử lý report, trạng thái/ghi chú xử lý.
- FR-20–21: dashboard đầy đủ theo thời gian/môn học, top tài liệu/top người dùng, trang ActivityLog và lọc log. Dashboard hiện chỉ có tổng user/resource, trạng thái, downloads, comments, rating trung bình.
- Thông báo, yêu thích, trang lỗi 403/404/500 tùy chọn theo đề bài.

### Quản trị, vận hành, bảo mật

- Trang cấu hình hệ thống, hồ sơ Admin, giao diện sao lưu/phục hồi; quy trình backup hiện mới mô tả trong tài liệu.
- Vai trò Lecturer chưa được gán qua UI. Production cần quy trình tạo role/seed role và cấp Admin rõ ràng; hiện ứng dụng chỉ seed role tự động trong Development.
- Upload mới kiểm tra extension và kích thước; chưa kiểm tra file signature/MIME thật hoặc quét malware. Cần rate limit và chính sách retention/quota nếu triển khai thật.
- Chốt chính sách khóa tài khoản với session đang hoạt động; hiện Login kiểm tra `IsActive`, nhưng cần bảo đảm cookie đang đăng nhập bị thu hồi kịp thời.
- Kiểm tra các FK còn thiếu ở DownloadRecord/ResourceReport và thiết lập migration cho mọi sửa đổi schema.

### Phân tích, tài liệu, kiểm thử

- Use case còn ở mức khái quát, chưa có đủ sơ đồ theo Khách/Member/Lecturer/Admin, include/extend và đặc tả use case đầy đủ.
- Activity diagram hiện mới mô tả upload/kiểm duyệt; còn thiếu đăng ký, đăng nhập, tìm kiếm, tải, bình luận/đánh giá, version, quản lý tài khoản.
- Sequence diagram hiện mới mô tả tải xuống; còn thiếu đăng nhập, upload và version.
- Chưa có Class Diagram đồng bộ đầy đủ với entity hiện tại; tài liệu ERD cần cập nhật theo từng migration.
- Chưa có ma trận CRUD/nghiệp vụ đầy đủ, tiêu chí đo NFR cụ thể, Unit/Integration/Functional/Security/UI tests hay kết quả test thật.
- Các phần danh mục/role/comment/report trong `docs/requirements-and-tests.md` phản ánh tiến độ chưa hoàn chỉnh; cập nhật bảng này mỗi lần hoàn tất module.

## Thứ tự làm tiếp đề xuất

1. **Danh mục học tập end-to-end:** entity/quan hệ Subject, Topic, AcademicYear, DifficultyLevel, ResourceType, Tag; migration; CRUD Admin; ViewModel/forms; resource upload/edit; filter tìm kiếm kết hợp.
2. **Quản trị tài khoản và quyền:** UI gán role Lecturer/Admin an toàn, trạng thái hoạt động/session, profile và lịch sử cá nhân.
3. **Tương tác/nội dung:** edit/delete comment theo quyền, report resource/comment và màn hình xử lý.
4. **Phiên bản/download/log:** ghi đúng user, ràng buộc FK, xem lịch sử và tải version được phép.
5. **Password recovery, dashboard/report, backup/deploy** theo phạm vi đồ án.
6. **Kiểm thử và tài liệu:** tạo test project, chạy test thật và ghi kết quả; hoàn thiện diagrams, ma trận quyền, NFR, checklist bảo mật, hướng dẫn backup/restore.

## Prompt dùng cho phiên sau

```text
Tiếp tục dự án NLCNPM từ repo hiện tại: https://github.com/AchinSouthSide/NLCNPM, workspace C:\Users\AChin\Desktop\NLCNPM. Đọc README.md, docs/HANDOFF_NEXT_SESSION.md và docs/requirements-and-tests.md trước khi sửa. Không tạo lại project và không đánh dấu test Pass nếu chưa chạy thật.

Hãy triển khai milestone tiếp theo end-to-end: danh mục học tập gồm Subject, Topic, AcademicYear, DifficultyLevel, ResourceType và Tag. Rà soát entity/ERD hiện có, thiết kế khóa ngoại/index/unique phù hợp, cập nhật EF Core entities, Fluent API, DbContext và tạo migration; xây CRUD Admin có authorization phía server; nối dữ liệu danh mục vào tạo/sửa tài nguyên; thêm lọc kết hợp và phân trang ở trang tìm kiếm. Dùng ViewModel, giữ controller mỏng và không phá luồng upload/duyệt hiện tại.

Trước khi sửa: kiểm tra git status/branch và đọc code liên quan; giữ nguyên thay đổi chưa commit. Sau khi sửa: build project, chạy các kiểm thử phù hợp (tạo test project nếu cần), không tuyên bố pass nếu không thực thi; cập nhật ERD, bảng tiến độ chức năng, test case và handoff này. Kiểm tra migration/schema nhất quán. Cuối cùng báo rõ file thay đổi, lệnh đã chạy, kết quả, phần còn thiếu và commit/push lên cùng repo public nếu working tree sạch và thay đổi đã được kiểm tra.

Sau milestone danh mục, tiếp tục các mục còn thiếu theo thứ tự trong phần “Thứ tự làm tiếp đề xuất” của handoff và cập nhật tài liệu tiến độ sau mỗi milestone.
```
