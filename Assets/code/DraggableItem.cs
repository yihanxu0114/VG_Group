using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Item Info")]
    public ValuableBlock.Type itemType; 

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    // 1. start dragging
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.position; // record original position in case we need to snap back
        canvasGroup.alpha = 0.6f; // become semi-transparent to indicate dragging
        canvasGroup.blocksRaycasts = false; // important: allow raycasts to pass through so we can detect drop targets
    }

    // 2. during dragging
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition; // let the item follow the mouse cursor
    }

    // 3. end dragging
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f; 
        canvasGroup.blocksRaycasts = true; 

        rectTransform.position = originalPosition;
    }
}