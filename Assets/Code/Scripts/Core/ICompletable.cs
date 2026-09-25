using System;

public interface ICompletable
{
        bool IsComplete{get;}
        event Action OnComplete;
}
