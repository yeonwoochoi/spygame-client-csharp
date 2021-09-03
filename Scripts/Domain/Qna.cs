using System;
using UnityEngine;

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

        [SerializeField] public string difficulty;
        [SerializeField] public string question;
        [SerializeField] public string[] correctAnswers;
        [SerializeField] public string[] wrongAnswers;

        #endregion

        #region Getter

        public QnaDifficulty GetDifficulty()
        {   
            return difficulty.Contains("Easy") ? QnaDifficulty.Easy : QnaDifficulty.Hard;
        }

        #endregion
    }
}