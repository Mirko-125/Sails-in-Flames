using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    [SerializeField] private Camera mainCamera;
    private Transform oldParent;
    [SerializeField] private Transform newParent;

    [SerializeField] private int length;
    [SerializeField] private GameObject inertPrefab;

    private int rotation = 0;

    private GameObject[] instances;

    private Point pt;

    private Vector3 startpos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("BeginDrag");
        //oldParent = transform.parent;
        //transform.parent = newParent.parent;
        image.raycastTarget = false;

        DragManager dm = FindFirstObjectByType<DragManager>();

        if (dm != null)
        {
            dm.RemovePlace(pt);
        }

        pt = new Point(-1, -1);
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

        image.raycastTarget = true;

        DragManager dm = FindFirstObjectByType<DragManager>();

        if (dm != null)
        {
            Point p = dm.TryPlace(length, rotation, transform);
            if (p.x == -1)
            {
                transform.localPosition = startpos;
            }
            else
            {
                pt = p;
            }
        }
    }

    public void Rotate()
    {
        rotation = (rotation + 1) % 4;

        UpdateState();
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(1)) && !image.raycastTarget)
        {
            Rotate();
        }
    }

    public Vector2 ReturnRotationPosition()
    {
        switch(rotation)
        {
            case 0:
                return new Vector2(32, 0);
            case 1:
                return new Vector2(0, -32);
            case 2:
                return new Vector2(-32, 0);
            case 3:
                return new Vector2(0, 32);
            default:
                return new Vector2(0, 0);
        }
    }

    void Start()
    {
        pt = new Point(-1, -1);
        startpos = transform.localPosition;
        length--;
        UpdateState();
    }

    public void UpdateState()
    {
        if (instances == null)
        {
            instances = new GameObject[length];
            instances[0] = Instantiate(inertPrefab, ReturnRotationPosition(), Quaternion.identity, transform);
            for (int i = 1; i < length; i++)
            {
                instances[i] = Instantiate(inertPrefab, ReturnRotationPosition(), Quaternion.identity, instances[i-1].transform);
            }
        }
        for (int i = 0; i < length; i++)
        {
            instances[i].transform.localPosition = ReturnRotationPosition();
        }
        
    }
}
