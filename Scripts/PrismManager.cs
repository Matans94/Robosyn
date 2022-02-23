using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrismManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem tripleBeam;
    [SerializeField] private ParticleSystem singleBeam;
    private raycastReflect[] _tripleSideScript;
    private raycastReflect _singleSideScript;
    private bool isParticlesPlay;
    private LineRenderer[] _tripleSideLn;
    private LineRenderer _singleSideLn;
    // above "single" refers to the side one ray shoot and "triple" to the side 3 ray shoot

    private Animator anim;
    
    private const int R = 4;
    private const int Y = 2;
    private const int B = 1;
    // const values of 3 basic colors used to calculate the color of the ray
    private int _singleColor;
    // the current color of teh "Single" ray, should be in 0-7

    [SerializeField] private bool flip;
    private bool gate;
    // private bool stopAnim;
    
    private void Awake()
    {
        _singleSideScript = transform.GetChild(1).GetComponent<raycastReflect>();
        _tripleSideScript = transform.GetChild(0).GetComponentsInChildren<raycastReflect>();
        
        _singleSideScript.dir = raycastReflect.Direction.Left;
        foreach (var ray in _tripleSideScript) ray.dir = raycastReflect.Direction.Right;

        _singleSideLn = transform.GetChild(1).GetComponent<LineRenderer>();
        _tripleSideLn = transform.GetChild(0).GetComponentsInChildren<LineRenderer>();

        anim = GetComponent<Animator>();
        isParticlesPlay = false;
        // OnLaserMiss();
        foreach (var ray in _tripleSideScript) ray.maxLength = 0;
        foreach (var ln in _tripleSideLn) ln.enabled = false;
        _singleSideLn.enabled = false;
        _singleSideScript.maxLength = 0;
    }

    private void Start()
    {
        _singleSideScript.SetRayColor(raycastReflect.RayColor.White);
        
        _tripleSideScript[0].SetRayColor(raycastReflect.RayColor.Red);
        _tripleSideScript[1].SetRayColor(raycastReflect.RayColor.Blue);
        _tripleSideScript[2].SetRayColor(raycastReflect.RayColor.Yellow);
    }

    private void playParticles(bool play)
    {
        if (play && !isParticlesPlay)
        {
            singleBeam.Play();
            tripleBeam.Play();
            isParticlesPlay = true;
        }
        else if (!play)
        {
            singleBeam.Stop();
            tripleBeam.Stop();
            isParticlesPlay = false;
        }

    }
    /// <summary>
    /// When a ray misses the object change the color and turn off accordingly
    /// </summary>
    /// <param name="cr">The color of the ray that missed, white as default since it means nothing</param>
    private void OnLaserMiss(raycastReflect.RayColor cr = raycastReflect.RayColor.White)
    {
        
        
        if (gate) return;
        gate = true;
        playParticles(false);
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !anim.GetCurrentAnimatorStateInfo(0).IsName("PrismFromUse")) anim.Play("PrismFromUse");
        
        RemoveColorToSingle(cr);
        // _singleSideScript.maxLength = 0;
        foreach (var ray in _tripleSideScript)
        {
            //ray.maxLength = 0;
            ray.TurnLaserOff();
            ray.SendMiss();
        }

        // _singleSideLn.enabled = false;
        //foreach (var lr in _tripleSideLn) lr.enabled = false;

        gate = false;
    }
    
    private void OnLaserHit(raycastReflect.RayParams pRayParams)
    {
        var dir = pRayParams.dir; // the direction the ray hit the object from
        var cr = pRayParams.cr; // the color of the ray that hit

        dir = flip switch
        {
            true when dir == raycastReflect.Direction.Right => raycastReflect.Direction.Left,
            true when dir == raycastReflect.Direction.Left => raycastReflect.Direction.Right,
            _ => dir
        };

        switch (dir)
        {
            case raycastReflect.Direction.Down:
                OnLaserMiss(cr);
                break;                      // The prism works on Left/Right so Up/Down counts as miss 
            case raycastReflect.Direction.Up:
                OnLaserMiss(cr);
                break;
            case raycastReflect.Direction.Right:
                playParticles(true);
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("PrismUseIdle")) break;
                if(!anim.GetCurrentAnimatorStateInfo(0).IsName("PrismToUse")) anim.Play("PrismToUse");

                foreach (var ray in _tripleSideScript) ray.TurnLaserOn();
                //foreach (var lr in _tripleSideLn) lr.enabled = true;
                
                break; // turn on the right side (i.e. the 3 color rays)
            case raycastReflect.Direction.Left:
                playParticles(true);
                if(!anim.GetCurrentAnimatorStateInfo(0).IsName("PrismUseIdle") && !anim.GetCurrentAnimatorStateInfo(0).IsName("PrismToUse")) anim.Play("PrismToUse");
                
                _singleSideScript.TurnLaserOn();
                //_singleSideScript.maxLength = 100;
                //_singleSideLn.enabled = true; // turn on the left side (i.e. the 1 single ray)
                AddColorToSingle(cr); // change the color of the single ray to represent the new ray hitting
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }
    /// <summary>
    /// Give a RayColor value, change the color of the single ray to reflect the addition 
    /// </summary>
    /// <param name="cr">The color to add to the ray</param>
    private void AddColorToSingle(raycastReflect.RayColor cr)
    {
        var ryb = cr switch
        {// Only 3 basic colors matter, the rest dont change anything
            raycastReflect.RayColor.Red => R,
            raycastReflect.RayColor.Blue => B,
            raycastReflect.RayColor.Yellow => Y,
            _ => 0
        };
        _singleColor |= ryb;  //Bitwise or
        _singleSideScript.SetRayColor((raycastReflect.RayColor) _singleColor);
    }
    /// <summary>
    /// Give a RayColor value, change the color of the single ray to reflect the subtraction 
    /// </summary>
    /// <param name="cr">The color to add to the ray</param>
    private void RemoveColorToSingle(raycastReflect.RayColor cr)
    {
        var ryb = cr switch
        {// Only 3 basic colors matter, the rest dont change anything
            raycastReflect.RayColor.Red => R,
            raycastReflect.RayColor.Blue => B,
            raycastReflect.RayColor.Yellow => Y,
            _ => 0
        };
        _singleColor &= ~ryb; // Bitwise and/not
        
        if (_singleColor == 0)
        { // color 0 means turn off
            //_singleSideScript.maxLength = 0;
            _singleSideScript.TurnLaserOff();
            _singleSideScript.SendMiss();
            //_singleSideLn.enabled = false;
        }
        else _singleSideScript.SetRayColor((raycastReflect.RayColor) _singleColor);
    }

    private void VictoryAnimation()
    {
        playParticles(false);
        anim.Play("good");
    }
    
    private void LoseAnimation()
    {
        playParticles(false);
        anim.Play("sad");
    }

    public void FadeAndTurnOffPrism()
    {
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(transform.GetChild(0).GetChild(i).GetComponent<raycastReflect>().FadeLaser());
        }
        StartCoroutine(transform.GetChild(1).GetComponent<raycastReflect>().FadeLaser());
    }
    
}

