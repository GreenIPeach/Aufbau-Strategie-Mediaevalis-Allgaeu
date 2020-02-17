using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomHoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public UnityEvent onHoverEnter;
	public UnityEvent onHoverExit;
	
	public void OnPointerEnter(PointerEventData pointerEventData)
	{
		onHoverEnter.Invoke();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		onHoverExit.Invoke();
	}
}
