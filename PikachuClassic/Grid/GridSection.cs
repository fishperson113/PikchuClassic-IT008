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
            public int Priority { get; set; } // Độ ưu tiên của section (dựa vào khoảng cách)
        }

        private readonly int rows;
        private readonly int cols;
        private readonly List<Section> sections;
        private readonly int sectionsCount;

        public GridSection(int rows, int cols)
        {
            // Trừ đi viền ngoài
            this.rows = rows - 2;
            this.cols = cols - 2;

            // Tính số sections dựa trên kích thước grid
            this.sectionsCount = (int)Math.Ceiling(Math.Sqrt((this.rows * this.cols) / 25.0));
            this.sections = DivideSections();
        }

        private List<Section> DivideSections()
        {
            var result = new List<Section>();
            int rowsPerSection = (int)Math.Ceiling((double)rows / Math.Sqrt(sectionsCount));
            int colsPerSection = (int)Math.Ceiling((double)cols / Math.Sqrt(sectionsCount));

            for (int i = 0; i < Math.Sqrt(sectionsCount); i++)
            {
                for (int j = 0; j < Math.Sqrt(sectionsCount); j++)
                {
                    var section = new Section
                    {
                        // Cộng 1 để tính cả viền ngoài
                        StartRow = i * rowsPerSection + 1,
                        EndRow = Math.Min((i + 1) * rowsPerSection, rows) + 1,
                        StartCol = j * colsPerSection + 1,
                        EndCol = Math.Min((j + 1) * colsPerSection, cols) + 1,
                        Priority = i + j // Priority tăng dần theo khoảng cách từ góc trái trên
                    };
                    result.Add(section);
                }
            }
            return result;
        }

        public List<Section> GetSectionsForIQ(double IQ)
        {
            // Sắp xếp sections theo priority
            var sortedSections = sections.OrderBy(s => s.Priority).ToList();

            // Tính số sections cần quét dựa vào IQ
            int sectionsToCheck = Math.Max(1, (int)(sectionsCount * IQ));

            return sortedSections.Take(sectionsToCheck).ToList();
        }

        public bool IsInSection(int row, int col, Section section)
        {
            return row >= section.StartRow && row <= section.EndRow &&
                   col >= section.StartCol && col <= section.EndCol;
        }
    }
}
