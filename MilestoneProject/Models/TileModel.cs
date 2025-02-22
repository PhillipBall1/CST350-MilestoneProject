using System.Text.Json.Serialization;

namespace MilestoneProject.Models
{
    public class TileModel
    {
        public int row { get; set; }
        public int column { get; set; }
        public bool isBomb { get; set; }
        public int neighborBombs { get; set; }
        public bool isRevealed { get; set; }
        public bool isFlagged { get; set; }

        public TileModel() { }

        [JsonConstructor]
        public TileModel(int row, int column, bool isBomb)
        {
            this.row = row;
            this.column = column;
            this.isBomb = isBomb;
            this.isRevealed = false;
            this.isFlagged = false;
            this.neighborBombs = 0;
        }
    }
}
