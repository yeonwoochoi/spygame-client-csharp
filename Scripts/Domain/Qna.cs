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

        public string difficulty; // ??
        public string question;
        public string[] correctAnswers;
        public string[] wrongAnswers;

        #endregion
    }
}