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
        originalPosition = rectTransform.position; 
        canvasGroup.alpha = 0.6f; 
        canvasGroup.blocksRaycasts = false; 
    }

    // 2. during dragging
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition; 
    }

    // 3. end dragging
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f; 
        canvasGroup.blocksRaycasts = true; 

        rectTransform.position = originalPosition;
    }
}