using UnityEngine;
using UnityEngine.EventSystems;

public class UIMouseBlocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public static bool MouseBlocked { get; private set; } = false;

  public void OnPointerEnter(PointerEventData _)
  {
		MouseBlocked = true;
  }

  public void OnPointerExit(PointerEventData _)
  {
   MouseBlocked = false;
  }
}
