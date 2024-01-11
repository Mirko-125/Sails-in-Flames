using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography;
using CommsFunctions;

public class MenuButtons : MonoBehaviour
{  
    [SerializeField] private AudioSource sound;
    [SerializeField] private AudioClip soundFile;
    [SerializeField] private GameObject menuButtons;
    [SerializeField] private GameObject lobby;
    [SerializeField] private GameObject credits;
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
        foundPlayer = false; // mozda u konstruktor da ubacim?
        // string buttonName = this.name;
        // string buttonNumber = Regex.Replace(buttonName, @"[^\d]", "");
        // int sceneNumber = int.Parse(buttonNumber) + 1;
        sound.Play();
        StartCoroutine(_PlayGame(foundPlayer));
    }

    private IEnumerator _PlayGame(bool foundPlayer)
    {
        yield return new WaitForSeconds(soundFile.length);
        lobby.SetActive(true);
        while(!foundPlayer)
        {
            // mora da bude bolje napravljeno
            Debug.Log("Čeka igrača");
            foundPlayer = true;
        }
        SceneManager.LoadScene(2);
    }

    public void ConfirmRoundTwo()
    {
        // ovde nam treba logika da ceka igraca
        sound.Play();
        StartCoroutine(_ConfirmRoundTwo());
    }

    private IEnumerator _ConfirmRoundTwo()
    {
        yield return new WaitForSeconds(soundFile.length);
        SceneManager.LoadScene(3);
    }

    public void GameSettings()
    {
        sound.Play();
        StartCoroutine(_GameSettings());
    }

    private IEnumerator _GameSettings()
    {
        yield return new WaitForSeconds(soundFile.length);
        SceneManager.LoadScene(5);
    }

    public void GameExtras()
    {
        Debug.Log("Ovo Boggy i Mirko pravili");
        menuButtons.SetActive(false);
        credits.SetActive(true);
    }

    public void CloseExtras()
    {
        credits.SetActive(false);
        menuButtons.SetActive(true);
    }
    public void QuitGame()
    {
        Debug.Log("Exited.");
        Application.Quit();
    }
}
