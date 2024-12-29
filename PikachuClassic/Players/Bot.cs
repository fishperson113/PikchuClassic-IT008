using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PikachuClassic
{
    public class Bot : Player
    {
        private Random random = new Random();
        private float IQ; // 0 đến 1

        public Bot(float IQ)
        {
            this.IQ = Math.Max(0, Math.Min(1, IQ)); // Đảm bảo IQ trong khoảng [0,1]
        }

        public async Task MakeMove(GridManager gridManager)
        {
            var matchedPairs = FindAllMatchingPairs(gridManager);
            Debug.WriteLine($"Số cặp khớp tìm thấy: {matchedPairs.Count}");

            if (matchedPairs.Count > 0)
            {
                // Sắp xếp các cặp theo điểm số
                var sortedPairs = matchedPairs.OrderByDescending(pair =>
                    (int)gridManager.Grid.GetScoreForImage(gridManager.GetOriginalImage(pair.Item1)))
                    .ToList();

                PictureBox firstBox, secondBox;

                // Xác suất chọn cặp tốt nhất dựa vào IQ
                if (random.NextDouble() < IQ)
                {
                    // Chọn một trong top 3 cặp có điểm cao nhất
                    int topIndex = random.Next(Math.Min(3, sortedPairs.Count));
                    var selectedPair = sortedPairs[topIndex];
                    firstBox = selectedPair.Item1;
                    secondBox = selectedPair.Item2;
                    Debug.WriteLine("Bot chọn theo IQ - Cặp có điểm cao");
                }
                else
                {
                    // Chọn ngẫu nhiên từ các cặp tìm được
                    var selectedPair = sortedPairs[random.Next(sortedPairs.Count)];
                    firstBox = selectedPair.Item1;
                    secondBox = selectedPair.Item2;
                    Debug.WriteLine("Bot chọn ngẫu nhiên");
                }

                // Thực hiện click
                await gridManager.BotClickCell(firstBox);
                await gridManager.BotClickCell(secondBox);
            }
        }

        private List<Tuple<PictureBox, PictureBox>> FindAllMatchingPairs(GridManager gridManager)
        {
            var matchingPairs = new List<Tuple<PictureBox, PictureBox>>();
            var pictureBoxes = gridManager.GetPictureBoxes();
            var rows = gridManager.Grid.GetRows();
            var cols = gridManager.Grid.GetCols();

            // Duyệt qua tất cả các cặp PictureBox có thể
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (!pictureBoxes[i, j].Visible) continue;

                    // Kiểm tra với tất cả các PictureBox còn lại
                    for (int m = 0; m < rows; m++)
                    {
                        for (int n = 0; n < cols; n++)
                        {
                            if (!pictureBoxes[m, n].Visible) continue;
                            if (i == m && j == n) continue; // Không so sánh với chính nó

                            // Kiểm tra nếu hai PictureBox có thể ghép cặp
                            if (gridManager.AreImagesMatching(pictureBoxes[i, j], pictureBoxes[m, n]))
                            {
                                matchingPairs.Add(new Tuple<PictureBox, PictureBox>(
                                    pictureBoxes[i, j], pictureBoxes[m, n]));
                            }
                        }
                    }
                }
            }

            return matchingPairs;
        }
    }
}