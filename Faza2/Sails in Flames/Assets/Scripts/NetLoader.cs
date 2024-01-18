using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetLoader : MonoBehaviour
{
    public GameObject childPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reveal()
    {
        childPanel.SetActive(true);
    }

    public void Relabel(string text)
    {
        childPanel.GetComponentInChildren<TMP_Text>().text = text;
    }

    public void Hide()
    {
        childPanel.SetActive(false);
    }
}
