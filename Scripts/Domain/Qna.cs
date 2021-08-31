using System;

namespace Domain
{
    #region Enum

    public enum QnaDifficulty
    {
        Easy, Hard
    }

    #endregion

    [Serializable]
    public class Qna
    {
        #region Public Variables

        public string difficulty;
        public string question;
        public string[] correctAnswers;
        public string[] wrongAnswers;

        #endregion

        #region Getter

        public QnaDifficulty GetDifficulty()
        {   
            return difficulty.Contains("Easy") ? QnaDifficulty.Easy : QnaDifficulty.Hard;
        }

        #endregion
    }
}