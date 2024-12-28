```mermaid
graph LR
A[Bắt đầu] --> B[Lấy node bắt đầu và kết thúc]
B --> C[Tạo 24 hoán vị thứ tự duyệt láng giềng]
C --> D[Khởi tạo danh sách đường đi]
D --> E{Duyệt từng hoán vị}
E --> F[Áp dụng hoán vị cho các node]
F --> G[Thực hiện BFS tìm đường]
G --> H{Tìm thấy đường?}
H -->|Có| I[Thêm vào danh sách đường đi]
H -->|Không| J{Còn hoán vị?}
I --> J
J -->|Có| E
J -->|Không| K[Kiểm tra tính hợp lệ các đường]
K --> L{Có đường hợp lệ?}
L -->|Có| M[Trả về đường đi đầu tiên hợp lệ]
L -->|Không| N[Trả về null]
M --> O[Kết thúc]
N --> O