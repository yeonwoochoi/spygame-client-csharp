using Random = UnityEngine.Random;

namespace Domain.StageObj
{
    public enum SpyType
    {
        Boss, Normal
    }
    
    public class Spy
    {
        public int index;
        public SpyType type;
        private Qna qna;
        public string question => qna.question;
        public string answer => GetAnswer();
        public bool isSpy;

        public Spy(int index, SpyType type, Qna qna, bool isRandom = true)
        {
            this.index = index;
            this.type = type;
            this.qna = qna;
            if (isRandom)
            {
                var random = Random.Range(0, 2);
                isSpy = random == 0;
            }
            else
            {
                isSpy = true;
            }
        }

        private string GetAnswer()
        {
            return isSpy ? qna.wrongAnswers[Random.Range(0, qna.wrongAnswers.Length)] : qna.correctAnswers[Random.Range(0, qna.correctAnswers.Length)];
        }
    }
}