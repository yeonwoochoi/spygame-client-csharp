using UnityEngine;
using Util;

namespace Domain.StageObj
{
    public enum ItemType
    {
        Time, 
        Hp 
    }
    
    public class Item
    {
        public int index;
        public ItemType type;
        private Qna qna;
        public string question => qna.question;
        public string answer => GetAnswer();
        public bool isCorrect;
        public int effect;

        public Item(int index, Qna qna)
        {
            this.index = index;
            this.qna = qna;
            type = (ItemType) Utils.GetRandomEnumValue<ItemType>();
            effect = type == ItemType.Hp ? 1 : 20;
            isCorrect = Random.Range(0, 2) == 0;
        }
        
        private string GetAnswer()
        {
            return isCorrect ? qna.correctAnswers[Random.Range(0, qna.correctAnswers.Length)] : qna.wrongAnswers[Random.Range(0, qna.wrongAnswers.Length)];
        }
    }
}