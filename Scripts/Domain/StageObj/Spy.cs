using Random = UnityEngine.Random;

namespace Domain.StageObj
{
    #region Enum

    public enum SpyType
    {
        Boss, Normal
    }

    #endregion

    public class Spy
    {
        #region Public Variables

        public int index;
        public SpyType type;
        public bool isSpy;

        #endregion

        #region Private Variable

        private Qna qna;

        #endregion

        #region Constructor

        public Spy(int index, SpyType type, Qna qna, bool isRandom = true, bool isSpy = true)
        {
            this.index = index;
            this.type = type;
            this.qna = qna;
            if (isRandom)
            {
                var random = Random.Range(0, 2);
                this.isSpy = random == 0;
            }
            else
            {
                this.isSpy = isSpy;
            }
        }

        #endregion

        #region Getter

        public string GetQuestion()
        {
            return qna.question;
        }
        
        public string GetAnswer()
        {
            return isSpy ? qna.wrongAnswers[Random.Range(0, qna.wrongAnswers.Length)] : qna.correctAnswers[Random.Range(0, qna.correctAnswers.Length)];
        }

        #endregion
    }
}