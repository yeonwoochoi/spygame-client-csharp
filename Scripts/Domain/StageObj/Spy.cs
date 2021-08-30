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
        public string question => qna.question; // TODO(??)
        public string answer => GetAnswer(); // TODO(??)
        public bool isSpy;

        #endregion

        #region Private Variable

        private Qna qna;

        #endregion

        #region Constructor

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

        #endregion

        #region Getter

        private string GetAnswer()
        {
            return isSpy ? qna.wrongAnswers[Random.Range(0, qna.wrongAnswers.Length)] : qna.correctAnswers[Random.Range(0, qna.correctAnswers.Length)];
        }

        #endregion
    }
}