using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorDirectionPointer : MonoBehaviour
{
    private const float RadiusX = 2.4f;  // the original x radius of the ellipsoid 
    private const float RadiusY = 1.5f;  // the original y radius of the ellipsoid 

    [SerializeField] private float xRadius;
    [SerializeField] private float yRadius;
    [SerializeField] private float pointDistance = 1f; // distance between mirror to point
    [SerializeField] private Transform robotMirror;

    private Vector3 initPos = Vector3.back; //just for initialize when enter stage
    private Transform arc;
    private Transform point;
    private bool isStart = false;
    private GameObject mirrorParent;
    // [SerializeField] private float pointsStartAngle;

    private bool isActive;

    private void Awake()
    {
        mirrorParent = transform.parent.gameObject;
        arc = transform.GetChild(0);
        point = transform.GetChild(1);
        isStart = true;
        xRadius = (xRadius == 0) ? RadiusX : xRadius; // set default if needed
        yRadius = (yRadius == 0) ? RadiusY : yRadius;
        
        initPos = point.position;
        // point.rotation = robotMirror.rotation;
        // //initialize the point in front of the mirror
        // point.localPosition = robotMirror.localPosition + robotMirror.forward * 2;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isActive) return;
        var dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - arc.position;
        var rad = Mathf.Atan2(dir.y / yRadius, dir.x / xRadius); // angle between mouse and pivot of arc
        
        var mirrorDir = mirrorParent.GetComponent<RobotManager>().mirrorDir;
        if (CheckLimits(rad, mirrorDir)) return; // check if the mirror can move in angle
        
        
        
        var angle = rad * Mathf.Rad2Deg;// angle between point and pivot of arc in degrees
        
        if (mirrorDir == RobotManager.MirrorDirection.Left && angle >= 0) angle +=  360 ; //todo : make it smarter - left rotation
        
        var rotation = mirrorDir switch
        { // set rotation of point based on direction and angle 
            RobotManager.MirrorDirection.Left => Quaternion.AngleAxis(-90 + (angle - 180) / 2, Vector3.forward), 
            RobotManager.MirrorDirection.Right => Quaternion.AngleAxis((angle - 180) / 2, Vector3.forward),
            RobotManager.MirrorDirection.Down => Quaternion.AngleAxis(-90 - (angle - 90) / -2, Vector3.forward),
            RobotManager.MirrorDirection.Up => Quaternion.AngleAxis((angle - 90) / 2, Vector3.forward),
            _ => Quaternion.AngleAxis((angle - 90) / 2, Vector3.forward)
        };
        point.rotation = Quaternion.Slerp(point.rotation, rotation, 50f * Time.deltaTime);
        
        var offset = mirrorDir switch
        {  // set position of point based on direction and angle 
            RobotManager.MirrorDirection.Left => Vector3.right * yRadius * Mathf.Cos(rad) + Vector3.up * xRadius * Mathf.Sin(rad),
            RobotManager.MirrorDirection.Right => Vector3.right * yRadius * Mathf.Cos(rad) + Vector3.up * xRadius * Mathf.Sin(rad),
            RobotManager.MirrorDirection.Down => Vector3.right * xRadius * Mathf.Cos(rad) + Vector3.up * yRadius * Mathf.Sin(rad),
            RobotManager.MirrorDirection.Up => Vector3.right * xRadius * Mathf.Cos(rad) + Vector3.up * yRadius * Mathf.Sin(rad),
            _ => Vector3.right * xRadius * Mathf.Cos(rad) + Vector3.up * yRadius * Mathf.Sin(rad)
        };
        point.position = arc.position + offset;
        // apply new position
    }

    private bool CheckLimits(float rad, RobotManager.MirrorDirection dir)
    {
        return dir switch
        {
            RobotManager.MirrorDirection.Left => rad + Mathf.PI/2 is <= Mathf.PI and >= 0,
            RobotManager.MirrorDirection.Right => rad + Mathf.PI/2 is >= Mathf.PI or <= 0,
            RobotManager.MirrorDirection.Down => rad >= 0,
            RobotManager.MirrorDirection.Up => rad <= 0,
            _ => false
        };
    }

    void PointerClicked()
    {
        isActive = true;
    }
    
    void PointerReleased()
    {
        isActive = false;
    }

    public void ResetPoint()
    {
        if (!isStart) return;
        point.position = initPos;
        point.rotation = robotMirror.rotation;
        isActive = false;
    }

}
