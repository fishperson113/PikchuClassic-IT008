```mermaid
flowchart TD
A[Bắt đầu lượt Bot] --> B[Lấy IQ của Bot]
B --> C[Chia bảng thành 4 section]
C --> D[Tính số section cần quét dựa vào IQ]
D --> E[Lấy danh sách các ô còn visible]
E --> F[Sắp xếp sections theo priority]
F --> G{Duyệt từng section theo IQ}
G --> H[Tìm tất cả cặp ô trong section]
H --> I{Có cặp ô ghép được?}
I -->|Có| J[Chọn cặp ô đầu tiên tìm thấy]
I -->|Không| K{Còn section khác?}
K -->|Có| G
K -->|Không| L[Không tìm thấy cặp nào]
J --> M[Click ô thứ nhất]
M --> N[Delay 500ms]
N --> O[Click ô thứ hai]
O --> P[Kết thúc lượt]
L --> P