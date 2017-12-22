using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PressButton : Button
{

    public float pressDelay = 0;
    public float interval = 0.02f;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        InvokeRepeating("RepeatClick", pressDelay, interval);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        CancelInvoke("RepeatClick");
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        CancelInvoke("RepeatClick");
    }

    private void RepeatClick()
    {
        onClick.Invoke();
    }
}
