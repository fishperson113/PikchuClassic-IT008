using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<Section> GetSectionsForIQ(double IQ)
        {
            // Sắp xếp sections theo priority
            var sortedSections = sections.OrderBy(s => s.Priority).ToList();

            // Tính số sections cần quét dựa vào IQ (1-4 sections)
            int sectionsToCheck = Math.Max(1, Math.Min(4, (int)(SECTION_COUNT * IQ)));

            return sortedSections.Take(sectionsToCheck).ToList();
        }

        public bool IsInSection(int row, int col, Section section)
        {
            return row >= section.StartRow && row <= section.EndRow &&
                   col >= section.StartCol && col <= section.EndCol;
        }
    }
}