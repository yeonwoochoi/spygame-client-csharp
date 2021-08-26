using System;
using Manager.Data;

namespace Domain
{
    public enum EControlType
    {
        Mouse,
        KeyBoard
    }
    
    [Serializable]
    public class EControlManager
    {
        public EControlType eControlType;

        public static EControlManager Create()
        {
            var controlInfo = new EControlManager
            {
                eControlType = EControlType.KeyBoard
            };
            return controlInfo;
        }
    }
}