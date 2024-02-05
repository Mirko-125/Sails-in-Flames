using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class BackButtons : MonoBehaviour
{
    [SerializeField] private AudioSource sound;
    [SerializeField] private AudioClip soundFile;
    [SerializeField] private GameObject mainMenuButton;
    [SerializeField] private GameObject question;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                question.SetActive(true);
                mainMenuButton.SetActive(false);
            }
        }
    }

    public void GoBackToMenu()
    {
        sound.Play();
        StartCoroutine(_GoBackToMenu());
    }
    private IEnumerator _GoBackToMenu()
    {
        yield return new WaitForSeconds(soundFile.length);
        SceneManager.LoadScene(1);
    }
    public void LeaveGameMenu()
    {
        GoBackToMenu();
    }

    public void ResumeGame()
    {
        question.SetActive(false);
        mainMenuButton.SetActive(true);
    }
}
