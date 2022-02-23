using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingDoor : MonoBehaviour
{
    private float maxSize;
    [SerializeField] private float movingSpeed;
    [SerializeField] private bool toClose;
    public bool isClosing;
    private Transform doorTrans;
    private Transform handTrans;

    private void Awake()
    {
        doorTrans = transform.GetChild(1);
        handTrans = transform.GetChild(2); 
        maxSize = doorTrans.localScale.x;
        movingSpeed = 0.5f;
        if (!toClose)
        {
            var doorScale = transform.localScale;
            doorScale.x = 0;
            doorTrans.localScale = doorScale;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
        var doorScale = doorTrans.localScale;
        doorScale.x += (isClosing ? -1 : 1)  * movingSpeed * Time.deltaTime;
        doorScale.x = Mathf.Clamp(doorScale.x, 0, maxSize);
        doorTrans.localScale = doorScale;

        var dorCHild = doorTrans.GetChild(0);
        var handPos = handTrans.localPosition;
        handPos.x = dorCHild.localPosition.x * 2 * doorScale.x + 0.5f;
        handTrans.localPosition = handPos;
    }

    public void TurnOn()
    {
        isClosing = toClose;
    }
    
    public void TurnOff()
    {
        isClosing = !toClose;
    }
}
