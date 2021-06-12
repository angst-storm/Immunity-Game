using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NotesChanger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public string info;
    public Text text;
    public GameObject panel;

    public void OnPointerDown(PointerEventData eventData)
    {
        text.text = info;
        panel.SetActive(false);
        text.enabled = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        text.enabled = false;
        panel.SetActive(true);
    }
}