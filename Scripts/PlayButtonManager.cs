using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class PlayButtonManager : MonoBehaviour
{

    private const string InvalidInitError = "Error: Play button parameters weren't initialized properly";
    
    private float _currentTime = 0f;
    private float timeToMove = 5f;

    private bool _moving = false;
    private Transform _camTrans;
    private Vector3 _camInitPos;
    private Vector3 _camEndPos;

    public Volume vl; 
    private Vignette gameVignette;

    [SerializeField] private Sprite[] _sps;
    private SpriteRenderer _renderer;
    [SerializeField] private FadeManager fadeOutScreen;

    private void Awake()
    {
        InitParameters(null, Vector3.positiveInfinity, Vector3.positiveInfinity);
        _renderer = GetComponent<SpriteRenderer>();
        vl.profile.TryGet(out gameVignette);

        gameVignette.intensity.value = 0f;


    }

    // Update is called once per frame
    void Update()
    {
        if (!_moving || timeToMove < _currentTime) return;
        if (_camTrans == null || _camInitPos == Vector3.positiveInfinity || _camEndPos == Vector3.positiveInfinity)
            throw new Exception(InvalidInitError);
        
        var camPos = _camTrans.position;
        _currentTime += Time.deltaTime;
        camPos.y = Mathf.Lerp(_camInitPos.y, _camEndPos.y, _currentTime / timeToMove);
        _camTrans.position = camPos;

    }

    public void InitParameters(Transform trans, Vector3 init, Vector3 end)
    {
        _camTrans = trans;
        _camInitPos = init;
        _camEndPos = end;
    }

    // private IEnumerator StartCameraAnimation()
    // {
    //     _moving = true;
    //     yield return new WaitForSeconds(timeToMove);
    //     _moving = false;
    //     _currentTime = 0f;
    //     GameManager.Instance.PlayButtonPressed();
    // }
    
    private IEnumerator StartCameraAnimation()
    {
        fadeOutScreen.gameObject.SetActive(true);
        StartCoroutine(fadeOutScreen.FadeImage(true, false));
        yield return new WaitForSeconds(2f);
        _camTrans.position = _camEndPos;
        gameVignette.intensity.value = 0.36f;
        GameManager.Instance.RestartLevel();
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(fadeOutScreen.FadeImage(false));
    }
    
    
    private void OnMouseDown()
    {
        StartCoroutine(StartCameraAnimation());
    }

    private void OnMouseEnter()
    {
        _renderer.sprite = _sps[1];
    }

    private void OnMouseExit()
    {
        _renderer.sprite = _sps[0];
    }
}
