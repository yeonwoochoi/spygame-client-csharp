using System;
using Manager.Data;

namespace Domain
{
    #region Enum

    public enum EControlType
    {
        Mouse,
        KeyBoard
    }

    #endregion

    [Serializable]
    public class EControlManager
    {
        #region Public Variable

        public EControlType eControlType;

        #endregion

        #region Static Method

        public static EControlManager Create()
        {
            var controlInfo = new EControlManager
            {
                eControlType = EControlType.KeyBoard
            };
            return controlInfo;
        }

        #endregion
    }
}