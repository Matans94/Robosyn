using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    [SerializeField] private GameObject obstacles;
    [SerializeField] private GameObject enemies;
    [SerializeField] private GameObject robots;
    [SerializeField] private GameObject funcRobots;
    [SerializeField] private GameObject laserSource;
    [SerializeField] private GameObject laserTarget;
    

    private void OnEnable()
    {
        obstacles.SetActive(true);
        enemies.SetActive(true);
        robots.SetActive(true);
        funcRobots.SetActive(true);
        laserSource.SetActive(true);
        laserTarget.SetActive(true);
    }

    public void FadeLaserAndTurnOff()
    {
        StartCoroutine(laserSource.GetComponent<raycastReflect>().FadeLaser());
        for (int i = 0; i < funcRobots.transform.childCount; i++)
        {
            if (funcRobots.transform.GetChild(i).CompareTag("Prism3"))
                funcRobots.transform.GetChild(i).GetComponent<PrismManager>().FadeAndTurnOffPrism();
        }
    }
    
    public void LaserOff()
    {
        laserSource.GetComponent<raycastReflect>().TurnLaserOff();
    }
    
    

    public void AllRobotFreeze(bool freeze)
    {
        foreach (Transform robot in robots.transform)
            robot.gameObject.GetComponent<RobotManager>().isFreeze = freeze;
    }


    public void AllRobotsOff()
    {
        foreach (Transform robot in robots.transform)
            robot.gameObject.GetComponent<RobotManager>().TurnOff();
    }

    public void StageReset(int stage)
    {
        ResetRobotsRotation();
        AllRobotsOff();
        
        if (stage != 0) laserSource.GetComponent<raycastReflect>().TurnLaserOn();
        else StartCoroutine(laserSource.GetComponent<raycastReflect>().startLaser());
        
        AllRobotFreeze(false);
        // laserTarget.GetComponent<LaserTragetManager>().ResetTargets();
        DisableEnemyParticles();
    }

    public void DisableEnemyParticles()
    {
        foreach (Transform enemy in enemies.transform)
        {
            enemy.GetChild(0).GetComponent<ParticleSystem>().Stop();
        }
    }
    
    private void ResetRobotsRotation()
    {
        foreach (Transform robot in robots.transform)
        {
            robot.gameObject.GetComponent<RobotManager>().ResetRotation();
            robot.gameObject.GetComponent<RobotManager>().isFreeze = false;
        }
    }

    public void VictoryAnimations()
    {
        foreach (Transform robot in funcRobots.transform) robot.gameObject.SendMessage("VictoryAnimation");
        foreach (Transform robot in robots.transform) robot.gameObject.SendMessage("VictoryAnimation");
        foreach (Transform enemy in enemies.transform) enemy.gameObject.GetComponent<Animator>().Play("Good");
        laserTarget.gameObject.SendMessage("VictoryAnimations");
    }
    
    public IEnumerator LoseAnimations()
    {
        //By Order - 1. enemy (with particles) 2.plants 3. robots.
        foreach (Transform robot in robots.transform) robot.gameObject.SendMessage("LoseAnimation");
        laserTarget.SendMessage("LoseAnimations");
        yield return null;// new WaitForSeconds(1f);
        foreach (Transform robot in funcRobots.transform) robot.gameObject.SendMessage("LoseAnimation");
    }
}