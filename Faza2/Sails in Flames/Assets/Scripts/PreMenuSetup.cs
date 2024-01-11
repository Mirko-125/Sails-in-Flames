using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PreMenuSetup : MonoBehaviour
{
    private string serverAddress;
    private string playerAlias;
    private bool serverToggle;
    void Start()
    {
        serverToggle = true;
        serverAddress = "204.216.216.204:7333";
    }

    void Update()
    {
        
    }

    public void ToggleServer(TMP_Text serverText)
    {
        serverToggle = !serverToggle;
        if(!serverToggle)
        {// localhost
            serverAddress = "localhost:5165";
            Debug.Log(serverAddress);
            serverText.text = "LocalHost";
        }
        else
        {// sailserver
            serverAddress = "204.216.216.204:7333";
            Debug.Log(serverAddress);
            serverText.text = "SailServer";
        }
    }
    public void Submit(TMP_InputField field)
    {
        playerAlias = field.text;
        Debug.Log(playerAlias);
    }
}
