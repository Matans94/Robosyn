using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    private GameObject parent;
    private AudioSource audioSource;

    private void Awake()
    {
        parent = transform.parent.gameObject;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnMouseDown()
    {
        parent.SendMessage("PointerClicked");
        audioSource.Play();
    }
    
    private void OnMouseUp()
    {
        parent.SendMessage("PointerReleased");
        audioSource.Stop();
    }
}