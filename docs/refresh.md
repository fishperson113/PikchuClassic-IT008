```mermaid
flowchart TD
A[Bắt đầu] --> B{Tất cả ô đã được ghép?}
B -->|Có| C[Kết thúc]
B -->|Không| D{Còn cặp hợp lệ?}
D -->|Có| E[Kết thúc]
D -->|Không| F[Xáo trộn lại vị trí các ô]
F --> G{Kiểm tra có cặp hợp lệ?}
G -->|Không| F
G -->|Có| H[Cập nhật hiển thị bảng]
H --> C