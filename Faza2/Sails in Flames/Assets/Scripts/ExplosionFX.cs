using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplosionFX : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] Sprite[] frames;
    [SerializeField] float frameDelay;
    [SerializeField] Image img;

    int frame = 0;

    void Start()
    {
        UpdateFrame();
    }

    // Update is called once per frame
    void UpdateFrame()
    {
        if (frame == frames.Length)
        {
            Destroy(gameObject);
        }
        else
        {
            img.sprite = frames[frame];
        }

        frame++;
        Invoke("UpdateFrame", frameDelay);
    }
}
