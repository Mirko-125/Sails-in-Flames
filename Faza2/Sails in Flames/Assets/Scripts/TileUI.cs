using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // Start is called before the first frame update

    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private Image _renderer;
    [SerializeField] private GameObject shipTile;
    [SerializeField] private GameObject sunkTile;
    bool friendly = false;
    private int x, y;

    void Start()
    {
        
    }

    public void Init(bool friendly, bool isOffset, int x, int y)
    {
        this.friendly = friendly;
        this.x = x;
        this.y = y;
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }

    public void WaterSink()
    {
        _renderer.color = new Color(0, 0.1f, 0.3f, _renderer.color.a);
    }

    public void ShipSink()
    {
        shipTile.SetActive(false);
        sunkTile.SetActive(true);
    }

    public void FightSetup(int state)
    {
        switch (state)
        {
            case 1:
                WaterSink();
                break;
            case 2:
                shipTile.SetActive(true);
                break;
            case 3:
                WaterSink();
                sunkTile.SetActive(true);
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!friendly)
        {
            print(y.ToString() + ", " + x.ToString());

            FightManager fm = FindFirstObjectByType<FightManager>();
            if (fm != null)
            {
                fm.DetectShot(x, y);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, _renderer.color.a - 0.2f);

        DragManager dm = FindFirstObjectByType<DragManager>();

        if (dm != null)
        {
            dm.UpdateHover(transform, x, y);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, _renderer.color.a + 0.2f);

        DragManager dm = FindFirstObjectByType<DragManager>();

        if (dm != null)
        {
            dm.EndHover();
        }
    }
}
