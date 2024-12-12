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
            var unmatchedBoxes = gridManager.GetUnmatchedBoxes();

            Debug.WriteLine($"Số ô chưa khớp: {unmatchedBoxes.Count}");
            if (unmatchedBoxes.Count < 2) return;

            var matchedPairs = FindMatchingPairs(unmatchedBoxes);
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
                    // Chọn ngẫu nhiên
                    var selectedPair = sortedPairs[random.Next(sortedPairs.Count)];
                    firstBox = selectedPair.Item1;
                    secondBox = selectedPair.Item2;
                    Debug.WriteLine("Bot chọn ngẫu nhiên");
                }

                // Thực hiện click
                await gridManager.BotClickCell(firstBox);
                await Task.Delay(300); // Độ trễ tự nhiên
                await gridManager.BotClickCell(secondBox);
            }
        }

        private List<Tuple<PictureBox, PictureBox>> FindMatchingPairs(List<PictureBox> unmatchedBoxes)
        {
            var matchedPairs = new List<Tuple<PictureBox, PictureBox>>();

            for (int i = 0; i < unmatchedBoxes.Count; i++)
            {
                for (int j = i + 1; j < unmatchedBoxes.Count; j++)
                {
                    var box1 = unmatchedBoxes[i];
                    var box2 = unmatchedBoxes[j];

                    if (GridManager.Instance.AreImagesMatching(box1, box2))
                    {
                        Node node1 = GridManager.Instance.Grid.GetNodeFromPictureBox(box1);
                        Node node2 = GridManager.Instance.Grid.GetNodeFromPictureBox(box2);

                        if (GridManager.Instance.Grid.HasPath(node1, node2))
                        {
                            matchedPairs.Add(Tuple.Create(box1, box2));
                        }
                    }
                }
            }

            return matchedPairs;
        }
    }
}
