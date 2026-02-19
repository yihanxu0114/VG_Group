using UnityEngine;
using UnityEngine.EventSystems;

public class SellSlot : MonoBehaviour, IDropHandler
{
    // when an item is dropped into this slot, we will sell it
    public void OnDrop(PointerEventData eventData)
    {
        // get the dropped object
        GameObject droppedObj = eventData.pointerDrag;

        if (droppedObj != null)
        {
            DraggableItem item = droppedObj.GetComponent<DraggableItem>();
            if (item != null)
            {
                // 1. find the player's inventory
                PlayerInventory inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
                Shop shopScript = FindObjectOfType<Shop>();
                if (inventory != null)
                {
                    // 2. decrease the item count in the inventory and increase the player's money
                    inventory.SellOneItem(item.itemType);
                    Debug.Log("Sell Successfully£¡");
                    if (shopScript != null)
                    {
                        shopScript.UpdateShopDisplay(inventory);
                    }
                }
            }
        }
    }
}