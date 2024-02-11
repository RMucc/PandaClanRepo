using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableClass : MonoBehaviour, IInteractable
{
    public bool InteractTaskOpen = false;

    public enum interactPhases
    {
        end,
        firstToEnd,
        secondToEnd,
        thirdToEnd,
        fourthToEnd
    }

    public bool CallUse()
    {
        Use();
        return false;
    }

    public abstract void Use();


    public bool showNotiUI()
    {
        return InteractTaskOpen;
    }
}
