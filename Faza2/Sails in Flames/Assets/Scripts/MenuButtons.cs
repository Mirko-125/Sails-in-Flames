using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    // Start is called before the first frame update
    private bool foundPlayer;
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        foundPlayer = false;
        while(!foundPlayer)
        {
            Debug.Log("Čeka igrača");
            foundPlayer = true;
        }
        // string buttonName = this.name;
        // string buttonNumber = Regex.Replace(buttonName, @"[^\d]", "");
        // int sceneNumber = int.Parse(buttonNumber) + 1;
        SceneManager.LoadScene(1);
    }

    public void GameSettings()
    {
        SceneManager.LoadScene(2);
    }

    public void GameExtras()
    {
        Debug.Log("Ovo Boggy i Mirko pravili");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
