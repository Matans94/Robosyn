using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;

public class MovingRobotManager : MonoBehaviour
{
    [SerializeField] private GameObject rail;
    [SerializeField] private Transform platform;
    private bool platformDir;
    private float platformSpeed = 1f;
    private Vector3 platformZeroPos;
    private AudioSource _audio;

    private float railXborder;

    private bool rayHit;
    
    private raycastReflect _rrScript;
    private LineRenderer _lr;
    
    private Animator _animator;

    private void Awake()
    {
        _rrScript = GetComponent<raycastReflect>();
        _lr = GetComponent<LineRenderer>();
        _animator = transform.Find("Sprite").GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
        OnLaserMiss();
        // _rrScript.dir = raycastReflect.Direction.Up;
    }

    // Start is called before the first frame update
    void Start()
    {
        railXborder = rail.GetComponent<MeshRenderer>().bounds.size.x / 2;
        platformZeroPos = rail.transform.position;

        _rrScript.maxLength = 0;
        _lr.enabled = false;
    }

    void Update()
    {
        if (!rayHit) return;
        var platPos = platform.position;

        platPos.x += (platformDir ? -1 : 1) * platformSpeed * Time.deltaTime;
        platPos.x = Mathf.Clamp(platPos.x, platformZeroPos.x - railXborder, platformZeroPos.x + railXborder);
        platform.position = platPos;
        // if (platPos.x == platformZeroPos.x + railXborder || platPos.x == platformZeroPos.x - railXborder)
        if (platPos.x >= platformZeroPos.x + railXborder - 0.3f || platPos.x <= platformZeroPos.x - railXborder + 0.3f)
            platformDir = !platformDir;
        rayHit = false;
    }
    

    
    private void OnLaserMiss()
    {
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !_animator.GetCurrentAnimatorStateInfo(0).IsName("GoingOutUse")) _animator.Play("GoingOutUse");
        if (_audio.isPlaying) _audio.Stop();
        // _rrScript.maxLength = 0;
        // _lr.enabled = false;
        
        rayHit = false;
    }
    
    private void OnLaserHit(raycastReflect.RayParams pRayParams)
    {
        if(!_animator.GetCurrentAnimatorStateInfo(0).IsName("IdleInUse") && !_animator.GetCurrentAnimatorStateInfo(0).IsName("GoingIntoUse")) _animator.Play("GoingIntoUse");
        if (!_audio.isPlaying) _audio.Play();
        // var dir = pRayParams.dir; // the direction the ray hit the object from
        // var cr = pRayParams.cr; // the color of the ray that hit
        //
        // _rrScript.SetRayColor(cr);
        // _rrScript.dir = dir;
        // _rrScript.maxLength = 100;
        // _lr.enabled = true;
        
        rayHit = true;
    }
    
    private void VictoryAnimation()
    {
        _animator.Play("Good");
    }

    private void LoseAnimation()
    {
        _animator.Play("Bad");
    }
    
}
