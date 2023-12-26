using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Tile : MonoBehaviour 
{
    private int x, y;
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
 
    public void Init(bool isOffset, int x, int y) 
    {
        this.x = x;
        this.y = y;
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }

    void OnMouseDown() 
    {
        SignalRController.Instance.SendSignalR("gamer", (Weapon)8, x, y);
    }

    void OnMouseEnter() 
    {
        _highlight.SetActive(true);
    }
 
    void OnMouseExit()
    {
        _highlight.SetActive(false);
    }
}