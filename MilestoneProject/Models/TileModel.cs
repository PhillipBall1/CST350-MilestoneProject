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

        public TileModel(int x, int y, bool bomb)
        {
            row = x;
            column = y;
            isBomb = bomb;
            isRevealed = false;
            isFlagged = false;
            neighborBombs = 0;
        }
    }
}
