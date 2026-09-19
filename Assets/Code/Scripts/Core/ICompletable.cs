using System;
using UnityEngine;

public interface ICompletable
{
        bool IsComplete{get;}
        event Action OnComplete;
}
