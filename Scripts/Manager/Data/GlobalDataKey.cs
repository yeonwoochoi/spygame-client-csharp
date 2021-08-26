namespace Manager.Data
{
    public class GlobalDataKey
    {
        public static GlobalDataKey CHAPTER = new GlobalDataKey("CHAPTER");
        public static GlobalDataKey SOUND = new GlobalDataKey("SOUND");
        public static GlobalDataKey ECONTROL = new GlobalDataKey("ECONTROL");
        public string key;

        private GlobalDataKey(string key)
        {
            this.key = key;
        }
    }
}