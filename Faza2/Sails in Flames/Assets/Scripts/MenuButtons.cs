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
        UnityHub hub = FindFirstObjectByType<UnityHub>();

        if (hub != null)
        {
            hub.LookForGame(PlayerPrefs.GetString("userID"));

            GameObject.Find("WaitingOverlay").GetComponent<NetLoader>().Reveal();
        }
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
        //SceneManager.LoadScene(3);

        UnityHub hub = FindFirstObjectByType<UnityHub>();

        ShopLogic shop = FindFirstObjectByType<ShopLogic>();

        if (hub != null && shop != null)
        {
            hub.AcceptWeapons(PlayerPrefs.GetString("userID"), shop.ReturnWeaponList());

            GameObject.Find("StartRoundTwo").GetComponent<Button>().interactable = false;
            GameObject.Find("StartRoundTwo").GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);

            GameObject.Find("WaitingOverlay").GetComponent<NetLoader>().Reveal();
        }

        
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
