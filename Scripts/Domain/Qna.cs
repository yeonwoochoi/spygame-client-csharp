namespace Domain
{
    #region Enum

    public enum QnaDifficulty
    {
        Easy, Hard
    }

    #endregion

    // TODO()
    public class Qna
    {
        #region Public Variables

        private string difficulty;
        public string question;
        public string[] correctAnswers;
        public string[] wrongAnswers;

        #endregion

        #region Getter

        public QnaDifficulty GetDifficulty()
        {
            return difficulty == "Easy" ? QnaDifficulty.Easy : QnaDifficulty.Hard;
        }

        #endregion
    }
}