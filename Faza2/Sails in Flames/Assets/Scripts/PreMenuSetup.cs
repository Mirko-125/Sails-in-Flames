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
        serverAddress = "http://204.216.216.204:7333/gameHub";
        playerAlias = "SailSailor";
    }

    void Update()
    {
        
    }

    public void ToggleServer(TMP_Text serverText)
    {
        serverToggle = !serverToggle;
        if(!serverToggle)
        {// localhost
            serverAddress = "http://localhost:5165/gameHub";
            Debug.Log(serverAddress);
            serverText.text = "LocalHost";
        }
        else
        {// sailserver
            serverAddress = "http://204.216.216.204:7333/gameHub";
            Debug.Log(serverAddress);
            serverText.text = "SailServer";
        }
        PlayerPrefs.DeleteKey("userID");
    }
    public void Submit(TMP_InputField field)
    {
        playerAlias = field.text;
        Debug.Log(playerAlias);
        PlayerPrefs.DeleteKey("userID");
    }

    public void AttemptConnection()
    {
        UnityHub conn = FindFirstObjectByType<UnityHub>();
        if (conn != null)
        {
            conn.username = playerAlias;
            PlayerPrefs.SetString("player", playerAlias);
            conn.Initialise(serverAddress);
        }
    }
}
