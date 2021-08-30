namespace Manager.Data
{
    public class GlobalDataKey
    {
        #region Static Variables

        public static GlobalDataKey CHAPTER = new GlobalDataKey("CHAPTER");
        public static GlobalDataKey SOUND = new GlobalDataKey("SOUND");
        public static GlobalDataKey ECONTROL = new GlobalDataKey("ECONTROL");
        public static GlobalDataKey TUTORIAL = new GlobalDataKey("TUTORIAL");

        #endregion

        #region Public Variable

        public string key;

        #endregion

        #region Constructor

        private GlobalDataKey(string key)
        {
            this.key = key;
        }

        #endregion
    }
}