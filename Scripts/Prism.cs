using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prism : MonoBehaviour
{
    private Transform separateLaser, blendLaser;
    private bool isHitSplit, isHitBlend;

    private void Awake()
    {
        separateLaser = gameObject.transform.Find("SplitEye");
        blendLaser = gameObject.transform.Find("BlendEye");
        
        
        for (int i = 0; i < 3; i++)
            separateLaser.transform.Find("Lasers").gameObject.transform.GetChild(i).GetComponent<LineRenderer>().
                    enabled = false;
        blendLaser.transform.Find("Laser").GetComponent<LineRenderer>().enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        isHitBlend = false; isHitSplit = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHitSplit)
        {
            for (int i = 0; i < 3; i++)
            {
                separateLaser.Find("Lasers").gameObject.transform.GetChild(i).GetComponent<LineRenderer>().enabled =
                    true;
            }

            isHitSplit = false;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                separateLaser.Find("Lasers").gameObject.transform.GetChild(i).GetComponent<LineRenderer>().enabled =
                    false;            }
        }

        if (isHitBlend)
        {
            blendLaser.transform.Find("Laser").GetComponent<LineRenderer>().enabled = true;
            isHitBlend = false;
        }
        else
        {
            blendLaser.transform.Find("Laser").GetComponent<LineRenderer>().enabled = false;
        }
    }
    
    /**
     * should recognize witch laser hit him, what is his color and make one laser with blend color. 
     */
    private void OnLaserHitBlendEye()
    {
        isHitBlend = true;
    }
    
    private void OnLaserMissBlendEye()
    {
        
    }

    /**
     * turn on 3 lasers, prism like
     */
    private void OnLaserHitSplitEye()
    {
        isHitSplit = true;
    }

    private void OnLaserMissSplitEye()
    {
     }
    
    
}
