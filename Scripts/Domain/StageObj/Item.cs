using UnityEngine;
using Util;


namespace Domain.StageObj
{
    #region Enum

    public enum ItemType
    {
        Time, 
        Hp 
    }

    #endregion

    public class Item
    {
        #region Public Variables

        public int index;
        public ItemType type;
        public bool isCorrect;
        public int effect;

        #endregion

        #region Private Variable

        private Qna qna;

        #endregion

        public string question => qna.question; // TODO(??)
        public string answer => GetAnswer(); // TODO(??)

        #region Constructor

        public Item(int index, Qna qna)
        {
            this.index = index;
            this.qna = qna;
            type = (ItemType) (Random.Range(0, 2));
            effect = type == ItemType.Hp ? 1 : 20;
            isCorrect = Random.Range(0, 2) == 0;
        }

        #endregion

        #region Getter

        private string GetAnswer()
        {
            return isCorrect 
                ? qna.correctAnswers[Random.Range(0, qna.correctAnswers.Length)] 
                : qna.wrongAnswers[Random.Range(0, qna.wrongAnswers.Length)];
        }

        #endregion
    }
}