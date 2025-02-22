namespace MilestoneProject.Models
{
    public class GameModel
    {
        public int ID { get; set; }
        public string UserEmail { get; set; }
        public DateTime DateSaved { get; set; }
        public string? GameData { get; set; }

        public GameModel() { }

        public GameModel(int iD, string userEmail, DateTime dateSaved, string? gameData)
        {
            ID = iD;
            UserEmail = userEmail;
            DateSaved = dateSaved;
            GameData = gameData;
        }
    }

}