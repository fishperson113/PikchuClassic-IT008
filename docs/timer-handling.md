```mermaid
graph LR
A[Bắt đầu] --> B[Khởi tạo Timer]
B --> C[Set thời gian ban đầu 20s]
C --> D[Timer tick]
D --> E[Giảm thời gian]
E --> F{Thời gian > 0?}
F -->|Có| G[Cập nhật hiển thị thời gian]
F -->|Không| H[Kết thúc lượt]
G --> I{Có ghép được ô?}
I -->|Có| J[Tính điểm dựa trên loại đường đi]
J --> K[Cộng điểm cho người chơi hiện tại]
K --> L[Reset thời gian về 20s]
L --> D
H --> M[Chuyển lượt]
M --> N[Reset thời gian]
N --> D
I -->|Không| D