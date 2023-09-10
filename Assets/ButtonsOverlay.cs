using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonsOverlay : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private WGQPlayerGui _wgqPlayerGui;
    public void OnPointerClick(PointerEventData eventData)
    {
        _wgqPlayerGui.ResetAutoHideTime();
        Debug.Log("Button Overlay Clicked");
    }
}
