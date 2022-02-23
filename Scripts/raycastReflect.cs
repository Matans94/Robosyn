using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class raycastReflect : MonoBehaviour
{

    public int reflections;
    public float maxLength;
    [SerializeField] private Transform laserParticalSystem;
    [SerializeField] private Transform BeamParticle;
    private LineRenderer _lr;
    private Ray _ray;
    private RaycastHit _hit;
    private bool laserIsFade;
    
    public enum RayColor { Black, Blue, Yellow, Green, Red, Purple, Orange, White }  //NOTE: the size and order of the enum matter!!
    [SerializeField] private RayColor color;
    private float _emission; // emission strength
    // Enum used to control the color of the ray
    
    public enum Direction {Left, Right, Up, Down}
    public Direction dir;
    // Enum used to control the direction the ray shoots out of object
    
    private GameObject[] _hitObject;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        _hitObject = new GameObject[20];
        _lr = GetComponent<LineRenderer>();
        _emission = Mathf.LinearToGammaSpace(15f);
        SetRayColor(color);
    }

    private void Start()
    {
        laserIsFade = false;
    }


    public class RayParams
    {/// <summary>
     /// This class is only used to send teh parameters of the ray through "SendMessage"  
     /// </summary>
        public readonly Direction dir;
        public readonly RayColor cr;
        // same as in the ray
        public RayParams(Direction dir, RayColor cr)
        {
            this.dir = dir;
            this.cr = cr;
        }
    }

    // Update is called once per frame
    void Update()
    {

        _ray = new Ray(transform.position, GetDirection());
        _lr.SetPosition(0, transform.position);
        _lr.positionCount = 1;
        float remainingLength = maxLength;
        
            
        for (int i = 0; i < reflections; i++)
        {
            
            
            if (Physics.Raycast(_ray.origin, _ray.direction, out _hit, remainingLength))
            {
                _lr.positionCount ++;
                _lr.SetPosition(_lr.positionCount -1, _hit.point);
                remainingLength -= Vector3.Distance(_ray.origin, _hit.point);

                laserParticalSystem.parent.transform.position = _lr.GetPosition(_lr.positionCount - 1);
                

                if (_hit.collider)
                {
                    laserParticalSystem.forward = _hit.normal;
                }

                if (_hit.collider.CompareTag("FunctionRobot"))
                { // notice: parent is used here
                    var newHitObject = _hit.transform.parent.gameObject;
                    if (_hitObject[i] == null) newHitObject.SendMessage("OnLaserHit", new RayParams(TranslateDirection(_hit.transform.position - _hit.point), color));
                    else
                    {
                        // _hitObject[i].SendMessage("OnLaserMiss");
                        newHitObject.SendMessage("OnLaserHit", new RayParams(TranslateDirection(_hit.transform.position - _hit.point), color));
                    }
                    _hitObject[i] = newHitObject;
                }
                
                else if (_hit.collider.CompareTag("PassThrough"))
                {
                    var newHitObject = _hit.transform.gameObject;
                    if (_hitObject[i] == null) newHitObject.SendMessage("OnLaserHit", new RayParams(TranslateDirection(_hit.transform.position - _hit.point), color));
                    else
                    {
                        // _hitObject[i].SendMessage("OnLaserMiss"); 
                        newHitObject.SendMessage("OnLaserHit", new RayParams(TranslateDirection(_hit.transform.position - _hit.point), color));
                    }
                    _hitObject[i] = newHitObject;
                }
                
                else if (_hit.collider.CompareTag("LaserTarget"))
                {
                    var newHitObject = _hit.transform.gameObject;
                    if (_hitObject[i] == null) newHitObject.SendMessage("OnLaserHit", color);
                    else
                    {
                        _hitObject[i].SendMessage("OnLaserMiss", color); 
                        newHitObject.SendMessage("OnLaserHit", color);
                    }
                    _hitObject[i] = newHitObject;
                }
                
                else if (_hit.collider.CompareTag("Prism3"))
                {
                    var newHitObject = _hit.transform.gameObject;
                    if (_hitObject[i] == null) newHitObject.SendMessage("OnLaserHit", new RayParams(TranslateDirection(_hit.transform.position - _hit.point), color));
                    else if (_hitObject[i] != newHitObject) _hitObject[i].SendMessage("OnLaserMiss", color);
                    else {
                        // _hitObject[i].SendMessage("OnLaserMiss", color); 
                        newHitObject.SendMessage("OnLaserHit", new RayParams(TranslateDirection(_hit.transform.position - _hit.point), color));
                    }
                    _hitObject[i] = newHitObject;
                }
              
                
                else if (_hitObject[i] != null)
                {
                    if (_hitObject[i].CompareTag("Prism3") || _hitObject[i].CompareTag("LaserTarget")) _hitObject[i].SendMessage("OnLaserMiss", color); 
                    else _hitObject[i].SendMessage("OnLaserMiss");
                    _hitObject[i] = null;
                }


                if (_hit.collider.CompareTag("Enemy") ) 
                {
                    BeamParticle.GetComponent<ParticleSystem>().Stop();
                    //laserParticalSystem.GetComponent<ParticleSystem>().Stop();
                    StartCoroutine(GameManager.Instance.EnemyHit(_hit));
                }
                else if (_hit.collider.CompareTag("Mirror"))
                {
                    _ray = new Ray(_hit.point, Vector3.Reflect(_ray.direction, _hit.normal));
                }

               
            }
            
            else
            {
                _lr.positionCount += 1;
                _lr.SetPosition(_lr.positionCount -1 , _ray.origin + _ray.direction * remainingLength);
            }
        }
    }

    public void SendMiss()
    {
        if (_hitObject == null) return;
        for (int i = 0; i < 20; i++)
        {
            if (_hitObject[i] == null) continue;
            if (_hitObject[i].CompareTag("Prism3") || _hitObject[i].CompareTag("LaserTarget")) _hitObject[i].SendMessage("OnLaserMiss", color); 
            else _hitObject[i].SendMessage("OnLaserMiss");
            _hitObject[i] = null; 
        }
    }

    /// <summary>
    /// Get the direction this ray is coming out of the gameObject according to its @dir parameter
    /// </summary>
    /// <returns>The direction appropriate to gameObject</returns>
    private Vector3 GetDirection()
    {
        return dir switch
        {
            Direction.Right => transform.up,
            Direction.Left => (-1) * transform.up,
            Direction.Up => (-1) * transform.forward,
            Direction.Down => transform.forward,
            _ => Vector3.forward
        };
    }
    /// <summary>
    /// Given 3 dimensional vector, projected into a 2d plane and encored to origin, returns the direction given
    /// as one of the four possible faces of the unit square.
    /// </summary>
    /// <param name="dirc">Direction to translate</param>
    /// <returns>Left/Right/up/Down based on direction</returns>
    private static Direction TranslateDirection(Vector3 dirc)
    {
        var x = Mathf.Abs(dirc.x);
        var y = Mathf.Abs(dirc.y);
        if (x > y) return (dirc.x < 0) ? Direction.Left : Direction.Right;
        return (dirc.y < 0) ? Direction.Down : Direction.Up;
    }
    /// <summary>
    /// Set the color of the ray to one of the 6 possible colors defined by the RayColor enum
    /// </summary>
    /// <param name="cr">The color to set</param>
    public void SetRayColor(RayColor cr)
    {
        color = cr;
        _lr.material.SetColor(EmissionColor, GetColor());
    }

    /// <summary>
    /// Using the @_color field of the ray, calculates and return a Color value with emission
    /// </summary>
    /// <returns>Color value of ray with emission</returns>
    private Color GetColor()
    {
        var cr = color switch
        {
            RayColor.White => Color.white * _emission * _emission,
            RayColor.Red => Color.red * _emission * _emission,
            RayColor.Blue => Color.blue * _emission * _emission,
            RayColor.Yellow => Color.yellow * _emission,
            RayColor.Green => Color.green * _emission,
            RayColor.Purple => new Color( 140,20,252),
            RayColor.Orange => new Color( 249,105,14),
            _ => Color.white,
        };
        
        return cr;
    }

    public IEnumerator FadeLaser()
    {
        laserIsFade = true;
        GameManager.Instance._disableKeys = true;
        var newcr = _lr.material.GetColor("_Color");

        // while (newcr.a > 0)
        // {
        //     newcr.a -= Time.deltaTime;
        //     Debug.Log(newcr.a);
        //     
        //     
        //     _lr.material.color = newcr;
        //     //Debug.Log(_lr.material.GetColor("_Color"));
        // }
        var cr = _lr.material.GetColor("_Color");
        var orCr = cr * _emission * _emission;
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            orCr.a = i;
            _lr.material.color = orCr;
            yield return null;
        }

        maxLength = 0;
        TurnLaserOff();
        _lr.material.color = cr;
        GameManager.Instance._disableKeys = false;
        laserIsFade = false;
    }

    public void TurnLaserOff()
    {
        BeamParticle.GetComponent<ParticleSystem>().Stop();
        laserParticalSystem.GetComponent<ParticleSystem>().Stop();
        _lr.enabled = false;
        maxLength = 0;
    }

    public void TurnLaserOn()
    {
        _lr.enabled = true;
        maxLength = 100f;
        BeamParticle.GetComponent<ParticleSystem>().Play();
        laserParticalSystem.GetComponent<ParticleSystem>().Play();
    }

    public IEnumerator startLaser()
    {
        _lr.enabled = true;
        maxLength = 0;
        BeamParticle.GetComponent<ParticleSystem>().Stop();
        laserParticalSystem.GetComponent<ParticleSystem>().Stop();
        for (float i = 0; i <= 12.2; i += Time.deltaTime*4)
        {
            maxLength = i;
            yield return null;
        }
        BeamParticle.GetComponent<ParticleSystem>().Play();
        laserParticalSystem.GetComponent<ParticleSystem>().Play();
        maxLength = 100;
    }
}
