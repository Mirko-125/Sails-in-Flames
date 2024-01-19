using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public UnityEngine.UI.Image image;
    [SerializeField] private Camera mainCamera;
    private Transform oldParent;
    [SerializeField] private Transform newParent;
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("BeginDrag");
        //oldParent = transform.parent;
        //transform.parent = newParent.parent;
        image.raycastTarget = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log(Input.mousePosition);
        Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(pos.x,pos.y,transform.position.z);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("EndDrag");
        //transform.parent = oldParent;
        image.raycastTarget = true;
    }
}
