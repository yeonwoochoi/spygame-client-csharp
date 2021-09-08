namespace Manager.Data
{
    public class GlobalDataKey
    {
        #region Public Variable

        public readonly string key;

        #endregion
        
        #region Static Variables

        public static readonly GlobalDataKey STAGE_SCORE = new GlobalDataKey("STAGE_SCORE");
        public static readonly GlobalDataKey SOUND = new GlobalDataKey("SOUND");
        public static readonly GlobalDataKey ECONTROL = new GlobalDataKey("ECONTROL");
        public static readonly GlobalDataKey TUTORIAL = new GlobalDataKey("TUTORIAL");

        #endregion

        #region Constructor

        private GlobalDataKey(string key)
        {
            this.key = key;
        }

        #endregion
    }
}