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
        private GridSection gridSection;

        public Bot(float IQ)
        {
            this.IQ = Math.Max(0, Math.Min(1, IQ)); // Đảm bảo IQ trong khoảng [0,1]
        }

        public async Task MakeMove(GridManager gridManager)
        {
            // Khởi tạo GridSection nếu chưa có
            if (gridSection == null)
            {
                var grid = gridManager.Grid;
                int rows = grid.GetRows();
                int cols = grid.GetCols();
                gridSection = new GridSection(rows, cols);
            }

            // Lấy các section cần kiểm tra dựa trên IQ
            var sectionsToCheck = gridSection.GetSectionsForIQ(IQ);
            var matchedPairs = FindMatchingPairsInSections(gridManager, sectionsToCheck);

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
                await Task.Delay(300);
                await gridManager.BotClickCell(secondBox);
            }
        }

        private List<Tuple<PictureBox, PictureBox>> FindMatchingPairsInSections(
            GridManager gridManager, 
            List<GridSection.Section> sections)
        {
            var matchedPairs = new List<Tuple<PictureBox, PictureBox>>();
var checkedBoxes = new HashSet<PictureBox>(); // Tránh check trùng

            foreach (var section in sections)
            {
                // Lấy các PictureBox trong section hiện tại
                var boxesInSection = GetPictureBoxesInSection(gridManager, section);

                foreach (var box1 in boxesInSection)
                {
                    if (!box1.Visible || checkedBoxes.Contains(box1)) continue;

                    foreach (var box2 in boxesInSection)
                    {
                        if (!box2.Visible || box1 == box2 || checkedBoxes.Contains(box2)) continue;

                        if (gridManager.AreImagesMatching(box1, box2))
                        {
                            matchedPairs.Add(Tuple.Create(box1, box2));
                        }
                    }
                    checkedBoxes.Add(box1);
                }
            }

            return matchedPairs;
        }

        private List<PictureBox> GetPictureBoxesInSection(
            GridManager gridManager, 
            GridSection.Section section)
        {
            var result = new List<PictureBox>();
            var pictureGrid = gridManager.GetPictureBoxes();

            for (int i = section.StartRow; i <= section.EndRow; i++)
            {
                for (int j = section.StartCol; j <= section.EndCol; j++)
                {
                    if (pictureGrid[i, j].Visible)
                    {
                        result.Add(pictureGrid[i, j]);
                    }
                }
            }

            return result;
        }
    }
}