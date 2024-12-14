using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Server.Core.Entities.Game
{
    public enum ScoreGroup
    {
        Group10 = 10,
        Group20 = 20,
        Group30 = 30,
        Group40 = 40,
        Group50 = 50,
        Group60 = 60,
    }

    public class Grid
    {
        #region Properties
        public int Rows { get; private set; }
        public int Cols { get; private set; }
        public int[,] Nodes { get; private set; } // 2D array to represent grid images
        private static Dictionary<int, ScoreGroup> imageScoreGroups = new Dictionary<int, ScoreGroup>();
        private static bool isScoreGroupsAssigned = false;
        #endregion

        public Grid(int rows, int cols)
        {
            this.Rows = rows;
            this.Cols = cols;
            this.Nodes = new int[rows, cols];
            AssignScoreGroups();
            AssignImagesToGrid();
        }

        private void AssignImagesToGrid()
        {
            int totalCells = Rows * Cols;
            if (totalCells % 2 != 0)
                throw new Exception("Number of cells must be even to form pairs.");

            var imageIds = GenerateImageIds();
            Shuffle(imageIds);

            int imageIndex = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    Nodes[i, j] = imageIds[imageIndex];
                    imageIndex++;
                }
            }
        }

        private List<int> GenerateImageIds()
        {
            var imageIds = new List<int>();
            int pairCount = (Rows * Cols) / 2;
            for (int i = 1; i <= pairCount; i++)
            {
                imageIds.Add(i);
                imageIds.Add(i);
            }
            return imageIds;
        }

        private void AssignScoreGroups()
        {
            // Assign ScoreGroup to each image
            // Example logic: every 5 images belong to the next group
            if (!isScoreGroupsAssigned)
            {
                // Giả sử mỗi nhóm có số lượng hình ảnh nhất định
                int imagesPerGroup = 5;
                int currentGroup = 1;

                for (int i = 1; i <= (Rows * Cols) / 2; i++)
                {
                    if (currentGroup > Enum.GetValues(typeof(ScoreGroup)).Length)
                        currentGroup = Enum.GetValues(typeof(ScoreGroup)).Length;

                    imageScoreGroups[i] = (ScoreGroup)(currentGroup * 10);
                    currentGroup++;
                }

                isScoreGroupsAssigned = true;
            }
        }

        private void Shuffle<T>(List<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public ScoreGroup GetScoreForImage(int imageId)
        {
            return imageScoreGroups.ContainsKey(imageId) ? imageScoreGroups[imageId] : ScoreGroup.Group10;
        }

        public bool AreAllPicturesMatched()
        {
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                    if (Nodes[i, j] != 0)
                        return false;
            return true;
        }
    }
}
   
    
