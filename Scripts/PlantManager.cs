using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    [SerializeField] private raycastReflect.RayColor acceptedColor; // the color this plant counts as a goal
    [SerializeField] private Transform winnigParticles;
    private Transform _parentObject; // the object governing all of the plants of the stage
    private bool _isHit;  // is this plant hit with the right color
    public int id; // the id the parent object gives
    private bool _particleTurnedOn;
    private float _fillSpeed = 5f;
    private float _scale;

    private Animator animator;

    private void Awake()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        _parentObject = transform.parent;
    }

    private void OnEnable()
    {
        ResetPlant();
    }

    private void Update()
    {
        _scale += (_isHit ? 1 : -1) * _fillSpeed * Time.deltaTime;
        _scale = Mathf.Clamp(_scale, 0, 1);

        if (_scale == 1f)
            _parentObject.SendMessage("NotifyHit", id);
    }

    private void OnLaserMiss(raycastReflect.RayColor cr)
    {
        if (!_isHit || cr != acceptedColor) return; // only miss that matter are goal color
        _parentObject.SendMessage("NotifyMiss", id);
        _isHit = false;
        
    }
    
    private void OnLaserHit(raycastReflect.RayColor cr)
    {
        if (_isHit || cr != acceptedColor) return;// only hit that matter are goal color
        _isHit = true;
    }

    public void ResetPlant()
    {
        _scale = 0;
        _isHit = false;
        animator.Play(acceptedColor + "Idle");
        winnigParticles.transform.GetComponent<ParticleSystem>().Stop();
        _particleTurnedOn = false;
    }
    
    private void VictoryAnimation()
    {
        animator.Play(acceptedColor + "Grow");
        if (!_particleTurnedOn) winnigParticles.transform.GetComponent<ParticleSystem>().Play();
        _particleTurnedOn = true;
    }
    
    private void LoseAnimation()
    {
        animator.Play(acceptedColor + "Death");
    }
}
