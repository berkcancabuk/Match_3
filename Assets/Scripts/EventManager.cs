using System;

public abstract class EventManager
{
    public static Action OnTimeSet;
    public static Action OnPlaySound;
    public static Action<int> OnAddScore;
}