using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserTragetManager : MonoBehaviour
{
    private StageManager _parentStage; // the stage this object belongs to
    private bool[] _hitChildArray;  // array with each item indicating whether appropriate child was hit

    private void Awake()
    {
        var tr = transform;
        _parentStage = tr.parent.GetComponent<StageManager>();
        
        _hitChildArray = new bool[tr.childCount];
        for (var i = 0; i < tr.childCount; i++) tr.GetChild(i).GetComponent<PlantManager>().id = i;
        // set id to all plant children to be able to differentiate   
    }

    private void NotifyHit(int id)
    {
        _hitChildArray[id] = true;
        if (_hitChildArray.All(plant => plant)) StartCoroutine(GameManager.Instance.StageComplete());
        // if all children are true (i.e. hit) stage is complete
        
        // if (_hitChildArray.Any(plant => !plant)) return;
    }
    
    private void NotifyMiss(int id)
    {
        _hitChildArray[id] = false;
    }

    private void VictoryAnimations()
    {
        var tr = transform;
        for (var i = 0; i < tr.childCount; i++)
        {
            var child = tr.GetChild(i);
            child.SendMessage("VictoryAnimation");
        }
    }
    
    private void LoseAnimations()
    {
        var tr = transform;
        for (var i = 0; i < tr.childCount; i++) tr.GetChild(i).SendMessage("LoseAnimation");
    }

    // public void ResetTargets()
    // {
    //     foreach (Transform plant in transform) plant.GetComponent<PlantManager>().ResetPlant();
    // }
}
