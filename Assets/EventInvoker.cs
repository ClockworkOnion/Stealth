using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventInvoker : MonoBehaviour // For use in Timeline
{
    public UnityEvent[] eventList;

    public void InvokeElement(int index) {
        eventList[index].Invoke();
    }

    public void EmptyEvent() {} // Um die Timeline zu strecken
}
