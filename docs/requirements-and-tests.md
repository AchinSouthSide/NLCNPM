# Yêu cầu và kiểm thử

## Trạng thái chức năng

| ID | Chức năng | Trạng thái |
|---|---|---|
| FR-01–03 | Đăng ký, đăng nhập, đăng xuất | Có (Identity) |
| FR-04 | Khôi phục mật khẩu qua email | Chưa có email/UI |
| FR-05 | Hồ sơ cá nhân | Chưa triển khai |
| FR-06–07 | Quản trị người dùng và quyền | Role nền tảng có; CRUD UI chưa có |
| FR-08–09 | Quản lý/upload tài liệu | Có tạo tài liệu; sửa/xóa chưa đủ |
| FR-10–11, 22 | Danh mục, tag, môn/chủ đề/năm học | Chưa triển khai |
| FR-12–14 | Tìm, chi tiết, tải | Có; tìm theo tiêu đề/mô tả, tải chỉ Approved |
| FR-15–16 | Bình luận và đánh giá | Có; rating 1–5, duy nhất theo user/resource |
| FR-17–18 | Phiên bản và nhật ký | Có phiên bản; log cơ bản |
| FR-19 | Kiểm duyệt | Duyệt/từ chối/ẩn/lưu trữ |
| FR-20–21 | Dashboard/báo cáo | Dashboard tổng quan cơ bản có; báo cáo chi tiết và trang nhật ký chưa có |

## Test cases đề xuất

Kết quả thực tế phải được điền sau khi chạy. Chưa có test nào được đánh dấu Pass.

| ID | Tiền điều kiện/dữ liệu | Bước | Kết quả mong đợi | Kết quả thực tế |
|---|---|---|---|---|
| AUTH-01 | Email mới, mật khẩu hợp lệ | Đăng ký | Tạo Member và đăng nhập | Chưa chạy |
| AUTH-02 | Mật khẩu sai 5 lần | Đăng nhập lặp | Từ chối/lockout theo policy | Chưa chạy |
| AUTH-03 | Khách | POST tạo tài liệu | Chuyển sang đăng nhập | Chưa chạy |
| UP-01 | PDF dưới 25 MB | Đăng tài liệu | Lưu GUID ngoài wwwroot, trạng thái Pending | Chưa chạy |
| UP-02 | EXE hoặc file trên 25 MB | Đăng tài liệu | Từ chối, không tạo Resource | Chưa chạy |
| MOD-01 | Pending, tài khoản Admin | Duyệt | Chuyển Approved và hiện công khai | Chưa chạy |
| MOD-02 | Pending, tài khoản Member | Gọi Moderation | HTTP 403 | Chưa chạy |
| DL-01 | Approved, file tồn tại | Tải | Trả file, tăng lượt, ghi record | Chưa chạy |
| DL-02 | Pending hoặc Hidden | Tải | Không trả file, HTTP 404 | Chưa chạy |
| VER-01 | Chủ sở hữu đăng version mới | Gửi version | File cũ còn, version tăng, chờ duyệt | Chưa chạy |
| VER-02 | User khác đăng version | POST | Bị từ chối | Chưa chạy |
| RATE-01 | Member, tài liệu Approved | Gửi 5 rồi gửi 3 | Chỉ một dòng rating, điểm là 3 | Chưa chạy |
| CMT-01 | Bình luận chứa markup | Mở trang chi tiết | Razor encode nội dung | Chưa chạy |
| SEARCH-01 | Nhiều tài liệu Approved | Tìm và chuyển trang | Giữ query, chỉ hiển thị tài liệu duyệt | Chưa chạy |

## Kiểm tra yêu cầu phi chức năng

- NFR-01/02: Identity hashing và role checks; thử truy cập action Admin khi là Member.
- NFR-03: seed dữ liệu lớn, đo tìm kiếm/phân trang; hiện chưa có full-text index.
- NFR-04: kiểm tra giới hạn tệp/trạng thái, bảo đảm App_Data không được phục vụ tĩnh.
- NFR-05: xác nhận FK, UNIQUE version/rating và CHECK điểm 1–5.
- NFR-06/07: rà soát ranh giới service, migrations và logs khi thêm chức năng.
- NFR-08/09: kiểm tra Chrome/Edge/Firefox và kích thước 360px, 768px, 1440px.
- NFR-10: backup SQL Server cùng upload directory, restore ở môi trường kiểm thử và đối chiếu số lượng/hash file.
