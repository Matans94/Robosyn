using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeManager : MonoBehaviour
{
    private SpriteRenderer _black;
    
    private void Awake()
    {
        _black = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _black.color = new Color(0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKey(KeyCode.P)) StartCoroutine(FadeImage(true));
        // if (Input.GetKey(KeyCode.O)) StartCoroutine(FadeImage(false));
    }
    
 
    public IEnumerator FadeImage(bool fadeAway, bool flash = true)
    {
        // fade from opaque to transparent
        if (!fadeAway) //fade to black screen
        {
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                _black.color = new Color(0, 0, 0, i);
                yield return null;
            }
            gameObject.SetActive(false);
        }
        else // fadeout - white flash and then fade to black screen
        {
            if (flash)
            {
                for (float i = 1; i >= 0; i -= Time.deltaTime * 2)
                { // flash white color
                  _black.color = new Color(1, 1, 1, i);
                yield return null;
                }
            }
            for (float i = 1; i >= 0; i -= Time.deltaTime * 3)
            { // pad time
                yield return null;
            }
            for (float i = 0f; i <= 1; i += Time.deltaTime)
            { // fade the black screen
                _black.color = new Color(0, 0, 0, i);
                yield return null;
            }
        }
    }
}
