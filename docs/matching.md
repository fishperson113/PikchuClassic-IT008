```mermaid
flowchart TD
A[Bắt đầu] --> B{Đã chọn ô đầu tiên?}
B -->|Chưa| C[Lưu ô được chọn làm ô thứ nhất]
B -->|Rồi| D{Kiểm tra ô thứ 2}
C --> E[Highlight ô được chọn]
E --> B
D -->|Cùng ô| G[Bỏ chọn ô]
D -->|Ô khác| H{Kiểm tra hình giống nhau?}
H -->|Không| I[Reset lựa chọn]
H -->|Có| J[Kiểm tra đường đi]
J -->|Không tìm thấy| I
J -->|Tìm thấy| K[Xóa 2 ô]
K --> L[Cập nhật điểm số]
L --> M[Reset lựa chọn]
G --> B
I --> B
M --> B