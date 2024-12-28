```mermaid
flowchart TD
A[Bắt đầu] --> B[Khởi tạo Grid Manager]
B --> C[Tạo Panel chứa grid]
C --> D[Tính toán kích thước ô]
D --> E[Tạo mảng PictureBox]
E --> F[Load hình ảnh PEEWEMON]
F --> G[Random vị trí các hình]
G --> H[Tạo đồ thị các node]
H --> I[Thiết lập các node láng giềng]
I --> J[Hiển thị grid]
J --> K[Kết thúc]
