namespace Domain
{
    public enum QnaDifficulty
    {
        Easy, Hard
    }
    
    // TODO()
    public class Qna
    {
        public string difficulty; // ??
        public string question;
        public string[] correctAnswers;
        public string[] wrongAnswers;
    }
}