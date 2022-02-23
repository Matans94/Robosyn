using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BUlbRobotManager : MonoBehaviour
{
    private raycastReflect _rrScript;
    private LineRenderer _lr;
    private Animator _animator;

    [SerializeField] private GameObject[] doors;
    private Animator[] _faceAnim;
    private AudioSource _audio;
    private MovingDoor[] _doorScript;
    
    private bool isParticlesPlay;
    [SerializeField] private ParticleSystem leftPart;
    [SerializeField] private ParticleSystem rightPart;

    private void Awake()
    {
        _rrScript = GetComponent<raycastReflect>();
        _lr = GetComponent<LineRenderer>();
        _animator = transform.Find("Sprite").GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();

        isParticlesPlay = false;

        _faceAnim = new Animator[doors.Length];
        _doorScript = new MovingDoor[doors.Length];
        for (var i = 0; i < doors.Length; i++)
        {
            _faceAnim[i] = doors[i].transform.GetChild(0).GetComponent<Animator>();
            _doorScript[i] = doors[i].GetComponent<MovingDoor>();
        }

        OnLaserMiss();
        _rrScript.dir = raycastReflect.Direction.Up;
    }

    private void OnLaserMiss()
    {
        playParticles(false);
        if (_audio.isPlaying) _audio.Stop();
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !_animator.GetCurrentAnimatorStateInfo(0).IsName("GoingOutUse")) _animator.Play("GoingOutUse");
        foreach (var face in _faceAnim)
        {
            if (!face.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !face.GetCurrentAnimatorStateInfo(0).IsName("GoingOutOfUse")) face.Play("GoingOutOfUse");
        }
        
        // _rrScript.maxLength = 0;
        // _lr.enabled = false;
        _rrScript.TurnLaserOff();
        
        foreach (var door in _doorScript) door.TurnOff();
    }
    
    private void playParticles(bool play)
    {
        if (play && !isParticlesPlay)
        {
            leftPart.Play();
            rightPart.Play();
            isParticlesPlay = true;
        }
        else if (!play && isParticlesPlay)
        {
            leftPart.Stop();
            rightPart.Stop();
            isParticlesPlay = false;
        }

    }
    
    private void OnLaserHit(raycastReflect.RayParams pRayParams)
    {
        var dir = pRayParams.dir; // the direction the ray hit the object from
        var cr = pRayParams.cr; // the color of the ray that hit

        if (dir is raycastReflect.Direction.Down or raycastReflect.Direction.Up) return;

        playParticles(true);
        if (!_audio.isPlaying) _audio.Play();
        if(!_animator.GetCurrentAnimatorStateInfo(0).IsName("IdleInUse") && !_animator.GetCurrentAnimatorStateInfo(0).IsName("GoingIntoUse")) _animator.Play("GoingIntoUse");
        foreach (var face in _faceAnim)
        {
            if(!face.GetCurrentAnimatorStateInfo(0).IsName("IdleInUse") && !face.GetCurrentAnimatorStateInfo(0).IsName("GoingIntoUse")) face.Play("GoingIntoUse");
        }
        
        _rrScript.SetRayColor(cr);
        _rrScript.dir = dir;
        _rrScript.TurnLaserOn();
        // _rrScript.maxLength = 100;
        // _lr.enabled = true;

        foreach (var door in _doorScript) door.TurnOn();
    }
    
    private void VictoryAnimation()
    {
        _animator.Play("Good");
        foreach (var face in _faceAnim) face.Play("Good");
    }
    
    private void LoseAnimation()
    {
        _animator.Play("Bad");
        foreach (var face in _faceAnim) face.Play("Bad");
    }
}
