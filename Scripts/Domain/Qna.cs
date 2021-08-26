namespace Domain
{
    public enum QnaDifficulty
    {
        Easy, Hard
    }
    public class Qna
    {
        public string difficulty;
        public string question;
        public string[] correctAnswers;
        public string[] wrongAnswers;
    }
}