using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RobotManager : MonoBehaviour
{
    private bool _isAwake; // is this robot is the one currently being controlled 
    [SerializeField] private float rotationSpeed = 30f; // the speed in which the mirror rotates on a button press
    [SerializeField] private Transform mirror;
    // [SerializeField] private Transform leftAngle, rightAngle;
    // private Vector3 leftVector, rightVector;
    private Animator headAnim;

    [SerializeField] private Transform particles;
    //private Transform _mirror;  // the mirror this robot is holding
    private Quaternion _mirrorInitRot;
    private float _mirrorInitZ;
    private SpriteRenderer _bodyRd;
    private AudioSource _audio;
    
    private Vector3 _prevMousePos;
    
    public bool isFreeze;

    [SerializeField] private Transform pointer; // the object that old both the arc and pointer 
    
    public enum MirrorDirection {Up, Down, Left, Right}
    public MirrorDirection mirrorDir; // which direction the arc (i.e. pointer) will face
    

    private void Awake()
    {
        headAnim = mirror.GetComponent<Animator>();
        _bodyRd = mirror.GetComponent<SpriteRenderer>();
        _audio = GetComponent<AudioSource>();
        isFreeze = false;
        _mirrorInitRot = mirror.localRotation;
        _mirrorInitZ = _mirrorInitRot.eulerAngles.z;
        if (_mirrorInitZ > 180f) _mirrorInitZ -= 360f;

        pointer.gameObject.SetActive(false); // pointer starts off not appearing
    }
    
    
    void Update()
    {
        if (!_isAwake || isFreeze) return;  // skip if robot is not active

        mirror.rotation = pointer.GetChild(1).rotation;
        // the rotation of the dot will be calculated and used as the rotation of the mirror
    }
    

    public void TurnOff()
    {
        _isAwake = false;
        headAnim.Play("Idle");
        pointer.gameObject.SetActive(false);
    }

    private void TurnOn()
    {
        _isAwake = true;
        headAnim.Play("press");
        pointer.gameObject.SetActive(true);
    }

    public void ResetRotation()
    {
        mirror.localRotation = _mirrorInitRot;
        //bug - when first start stage the pointer gameobject is off. 
        pointer.GetComponent<MirrorDirectionPointer>().ResetPoint();
    }
    
    
    private void OnMouseDown()
    {
        if (GameManager.Instance.sandboxMode)
        { // if sandbox there is not stageManager active
            foreach (Transform sibling in transform.parent)
            {
                sibling.gameObject.GetComponent<RobotManager>().TurnOff();
            }
        }
        else if (isFreeze)
        {
            return;
        }
        else if (_isAwake)
        {
            TurnOff();
            return;
        }
        else GameManager.Instance.AllRobotsOff();
        TurnOn();
        _audio.Play();
    }

    private void OnMouseEnter()
    {
        if (isFreeze) return;
        if(!headAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) return;
        headAnim.Play("OnHover");
    }

    private void OnMouseExit()
    {
        if (isFreeze) return;
        if(!headAnim.GetCurrentAnimatorStateInfo(0).IsName("OnHover")) return;
        headAnim.Play("Idle");
    }
    
    private void VictoryAnimation()
    {
        headAnim.Play("Good");
        isFreeze = true;
        if (_isAwake) pointer.gameObject.SetActive(false);
    }

    private void LoseAnimation()
    {
        headAnim.Play("Bad");
        isFreeze = true;
        if (_isAwake) pointer.gameObject.SetActive(false);
    }
}
