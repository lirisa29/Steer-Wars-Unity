using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonOutlineEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private AudioManager audioManager;

    void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioManager.PlaySound("SFX_ButtonHover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        audioManager.PlaySound("SFX_ButtonClick");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}
