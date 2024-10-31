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
        private float IQ;

        public Bot(float IQ)
        {
            this.IQ = IQ;
        }

        public async Task MakeMove(GridManager gridManager)
        {
            var unmatchedBoxes = gridManager.GetUnmatchedBoxes();

            Debug.WriteLine($"Số ô chưa khớp: {unmatchedBoxes.Count}");
            if (unmatchedBoxes.Count < 2) return;

            PictureBox firstBox = null;
            PictureBox secondBox = null;

            var matchedPairs = FindMatchingPairs(unmatchedBoxes);
            Debug.WriteLine($"Số cặp khớp tìm thấy: {matchedPairs.Count}");
            if (matchedPairs.Count > 0)
            {
                // Chọn ngẫu nhiên một cặp
                var pair = matchedPairs[random.Next(matchedPairs.Count)];
                firstBox = pair.Item1;
                secondBox = pair.Item2;
            }

            // Thực hiện hành động cho bot
            if (firstBox != null && secondBox != null)
            {
                await gridManager.BotClickCell(firstBox); // Sử dụng hàm trung gian
                await gridManager.BotClickCell(secondBox); // Sử dụng hàm trung gian
            }
        }

        private List<Tuple<PictureBox, PictureBox>> FindMatchingPairs(List<PictureBox> unmatchedBoxes)
        {
            var matchedPairs = new List<Tuple<PictureBox, PictureBox>>();
            // Tìm các cặp hình ảnh khớp nhau
            for (int i = 0; i < unmatchedBoxes.Count; i++)
            {
                for (int j = i + 1; j < unmatchedBoxes.Count; j++)
                {
                    if (GridManager.Instance.AreImagesMatching(unmatchedBoxes[i], unmatchedBoxes[j]))
                    {
                        matchedPairs.Add(Tuple.Create(unmatchedBoxes[i], unmatchedBoxes[j]));
                    }
                }
            }

            return matchedPairs;
        }
    }
}
