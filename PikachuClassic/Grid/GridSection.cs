using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PikachuClassic
{
    public class GridSection
    {
        public class Section
        {
            public int StartRow { get; set; }
            public int EndRow { get; set; }
            public int StartCol { get; set; }
            public int EndCol { get; set; }
            public int Priority { get; set; }
        }

        private readonly int rows;
        private readonly int cols;
        private readonly List<Section> sections;
        private const int SECTION_COUNT = 4; // Luôn chia thành 4 phần

        public GridSection(int rows, int cols)
        {
            // Trừ đi viền ngoài
            this.rows = rows - 2;
            this.cols = cols - 2;
            this.sections = DivideSections();
        }

        private List<Section> DivideSections()
        {
            var result = new List<Section>();

            // Tính điểm chia giữa
            int midRow = rows / 2;
            int midCol = cols / 2;

            // Tạo 4 sections
            result.Add(new Section  // Trái trên
            {
                StartRow = 1,
                EndRow = midRow + 1,
                StartCol = 1,
                EndCol = midCol + 1,
                Priority = 0
            });

            result.Add(new Section  // Phải trên
            {
                StartRow = 1,
                EndRow = midRow + 1,
                StartCol = midCol + 1,
                EndCol = cols + 1,
                Priority = 1
            });

            result.Add(new Section  // Trái dưới
            {
                StartRow = midRow + 1,
                EndRow = rows + 1,
                StartCol = 1,
                EndCol = midCol + 1,
                Priority = 1
            });

            result.Add(new Section  // Phải dưới
            {
                StartRow = midRow + 1,
                EndRow = rows + 1,
                StartCol = midCol + 1,
                EndCol = cols + 1,
                Priority = 2
            });

            return result;
        }

        public List<Section> GetSectionsForIQ(double IQ, Grid grid)
        {
            // Sắp xếp sections theo priority
            var sortedSections = sections.OrderBy(s => s.Priority).ToList();

            // Tính số sections tối đa cần quét dựa vào IQ (1-4 sections)
            int maxSectionsToCheck = Math.Max(1, Math.Min(4, (int)(SECTION_COUNT * IQ)));

            // Lấy tất cả các section có thể quét
            var availableSections = sortedSections.Take(maxSectionsToCheck).ToList();

            // Nếu không tìm thấy cặp ghép trong các section ưu tiên,
            // thêm các section còn lại vào để tìm kiếm
            if (!HasValidPairsInSections(availableSections, grid))
            {
                availableSections = sortedSections;
            }
            return availableSections;
        }

        public bool IsInSection(int row, int col, Section section)
        {
            return row >= section.StartRow && row <= section.EndRow &&
                   col >= section.StartCol && col <= section.EndCol;
        }

        public List<Tuple<PictureBox, PictureBox>> FindMatchingPairsInSections(Grid grid, List<Section> sectionsToCheck)
        {
            var matchingPairs = new List<Tuple<PictureBox, PictureBox>>();
            var pictureBoxes = grid.GetPictureBoxes();

            foreach (var section in sectionsToCheck)
            {
                var visibleBoxesInSection = GetVisibleBoxesInSection(pictureBoxes, section);

                // Tìm các cặp khớp trong section
                for (int i = 0; i < visibleBoxesInSection.Count - 1; i++)
                {
                    for (int j = i + 1; j < visibleBoxesInSection.Count; j++)
                    {
                        var box1 = visibleBoxesInSection[i];
                        var box2 = visibleBoxesInSection[j];

                        if (box1.Image== box2.Image && grid.HasPath(box1, box2))
                        {
                            matchingPairs.Add(new Tuple<PictureBox, PictureBox>(box1, box2));
                        }
                    }
                }
            }

            return matchingPairs;
        }

        private List<PictureBox> GetVisibleBoxesInSection(PictureBox[,] pictureBoxes, Section section)
        {
            var visibleBoxes = new List<PictureBox>();

            for (int i = section.StartRow; i <= section.EndRow; i++)
            {
                for (int j = section.StartCol; j <= section.EndCol; j++)
                {
                    if (pictureBoxes[i, j].Visible)
                    {
                        visibleBoxes.Add(pictureBoxes[i, j]);
                    }
                }
            }

            return visibleBoxes;
        }

        private bool HasValidPairsInSections(List<Section> sectionsToCheck, Grid grid)
        {
            return FindMatchingPairsInSections(grid, sectionsToCheck).Any();
        }
    }
}