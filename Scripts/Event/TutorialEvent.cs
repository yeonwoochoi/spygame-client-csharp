using System;

namespace Event
{
    public enum TutorialExitType
    {
        Success, TimeOver, Failure
    }
    
    public class ExitTutorialEventArgs : EventArgs
    {
        public TutorialExitType tutorialExitType;
    }
    
    public class StartTutorialGameEventArgs: EventArgs
    {
        
    }
}