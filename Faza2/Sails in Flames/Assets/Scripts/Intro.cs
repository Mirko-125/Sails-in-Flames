using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    [SerializeField] private GameObject intro;
    [SerializeField] private GameObject setup; 
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject cnObj;

    public IEnumerator Setup(float delay)
    {
        yield return new WaitForSeconds(delay);
        intro.SetActive(false);
        setup.SetActive(true);
    }

    public IEnumerator Title(float delay)
    {
        yield return new WaitForSeconds(delay);
        title.SetActive(true);
        StartCoroutine(Setup(3f));
    }

    void Start()
    {
        intro.SetActive(true);
        StartCoroutine(Title(1f));
        UnityHub conn = FindFirstObjectByType<UnityHub>();
        if (conn == null)
        {
            GameObject instance = Instantiate(cnObj);
            DontDestroyOnLoad(instance);
        }
    }

    void Update()
    {
        
    }
}
