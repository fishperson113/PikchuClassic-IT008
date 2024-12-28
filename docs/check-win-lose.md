```mermaid
flowchart TD
A[Bắt đầu] --> B{Còn thời gian?}
B -->|Không| C[Chuyển lượt]
B -->|Có| D{Còn ô trên bảng?}
D -->|Không| E[Kiểm tra điểm số]
D -->|Có| F{Còn nước đi hợp lệ?}
F -->|Có| G[Tiếp tục game]
F -->|Không| H[Làm mới bảng]
E --> I{So sánh điểm 2 người}
I -->|Player 1 > Player 2| J[Player 1 thắng]
I -->|Player 1 < Player 2| K[Player 2 thắng]
I -->|Player 1 = Player 2| L[Hòa]
J --> N[Hiển thị màn hình thắng]
K --> N
L --> N
H --> B
G --> B
C --> B
