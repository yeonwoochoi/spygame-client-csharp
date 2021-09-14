using System;

namespace Event
{
    public class ExitTutorialEventArgs : EventArgs
    {
        public bool isSuccess;
    }
    
    public class StartTutorialGameEventArgs: EventArgs
    {
        
    }
}