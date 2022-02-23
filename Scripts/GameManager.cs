using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const int SKIP = 1, BACK = 2, MENU = 3;
    
    [SerializeField] private StageManager[] stages;  // array of all of the stages in the games in order of appearance
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject endScreen;
    private PlayButtonManager _playButton ;  // The play button in the start screen controlling the starting animation 
    [SerializeField] private FadeManager fadeOutScreen; // The object controlling the fade animation between stages 
    [SerializeField] private int startStage; // which stage the game starts on
    [SerializeField] private bool skipStart; // flag indicates whether to skip the intro
    public bool sandboxMode; // starts game in sandbox stage
    [SerializeField] private GameObject sandboxStage;
    private bool gameFreeze;
    private Vector3 _camStartScreenPos; // The Position the camera starts with the start screen
    private Vector3 _camGameScreenPos;  // The position the camera should be during stages 
    private Transform _camTrans;

    private StageManager _curStage;
    private int _curStageNum;  // the index of the current stage in the stage array @stages

    public bool _disableKeys;

    private AudioSource[] _audio;
    [SerializeField] private AudioClip[] sfx;
    private bool stopSound;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        _playButton = startScreen.transform.GetComponentInChildren<PlayButtonManager>();
        _audio = GetComponents<AudioSource>();
        
        _camGameScreenPos = Vector3.back * 11; // (0,0,-11)
        _camStartScreenPos = _camGameScreenPos + Vector3.up * 20;  // (0,20,-11)
        _camTrans = Camera.main.transform;

        skipStart = skipStart || sandboxMode;
        gameFreeze = false;
    }

    private void Start()
    {
        if (skipStart)
        { // set camera to its position during stages
            startScreen.SetActive(false);
            _camTrans.position = _camGameScreenPos;
        }
        else
        { // set camera position to start screen
            startScreen.SetActive(true);
            _playButton.InitParameters(_camTrans, _camStartScreenPos, _camGameScreenPos);
        }
        
        foreach (var stage in stages) stage.gameObject.SetActive(false);
        
        sandboxStage.SetActive(sandboxMode);
        if (sandboxMode) return;
        
        _curStageNum = startStage;
        SetStage(startStage, true);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))  NevigateLevel(MENU);
        if (_disableKeys) return;
        if (Input.GetKey(KeyCode.Alpha1)) SetStage(0);
        if (Input.GetKey(KeyCode.Alpha2)) SetStage(1);
        if (Input.GetKey(KeyCode.Alpha3)) SetStage(2);
        if (Input.GetKey(KeyCode.Alpha4)) SetStage(3);
        if (Input.GetKey(KeyCode.Alpha5)) SetStage(4);
        if (Input.GetKey(KeyCode.Alpha6)) SetStage(5);
        if (Input.GetKey(KeyCode.Alpha7)) SetStage(6);
        if (Input.GetKey(KeyCode.Alpha8)) SetStage(7);
        if (Input.GetKey(KeyCode.R)) RestartLevel();
        if (Input.GetKeyDown(KeyCode.LeftBracket)) NevigateLevel(BACK);
        if (Input.GetKeyDown(KeyCode.RightBracket)) NevigateLevel(SKIP);
    }
    

    //Skip or prev level nevigator
    private void NevigateLevel(int action)
    {
        _disableKeys = true;
        fadeOutScreen.gameObject.SetActive(true);
        _curStage.LaserOff();
        StartCoroutine(fadeOutScreen.FadeImage(false));
        AllRobotsOff();

        if (action == MENU)
        {
            endScreen.SetActive(false);
            skipStart = false;
            _camTrans.position = _camStartScreenPos;
            Start();
            _disableKeys = false;
            return;
        }
        
        else if (action == SKIP && _curStageNum + 1 < stages.Length)
            _curStageNum++;
        
        else if (action == BACK && _curStageNum - 1 >= 0)
            _curStageNum--;

        //SetStage(_curStageNum);
        RestartLevel();
    }

    public void RestartLevel()
    {
        _disableKeys = true;
        fadeOutScreen.gameObject.SetActive(true);
        StartCoroutine(fadeOutScreen.FadeImage(true));
        _curStage.LaserOff();
        StartCoroutine(fadeOutScreen.FadeImage(false));
        AllRobotsOff();
        SetStage(_curStageNum);
        
        _disableKeys = false;
    }


    private void SetStage(int stageID = 0, bool isFirst = false)
    {
        if (_curStageNum == stages.Length)
        {
            _disableKeys = true;
            _curStage.gameObject.SetActive(false);
            endScreen.SetActive(true);
            return;
        }
        if (sandboxMode) return; //TODO
        gameFreeze = false;
        
        _curStageNum = stageID;
        if (!isFirst) _curStage.gameObject.SetActive(false);
        _curStage = stages[stageID];
        _curStage.gameObject.SetActive(true);
        _curStage.StageReset(stageID);
    }

    public void AllRobotsOff()
    {
        _curStage.AllRobotsOff();
    }


    public IEnumerator StageComplete()
    {
        if (stopSound) yield break;
        stopSound = true;
        _disableKeys = true;
        if(!_audio[1].isPlaying)
        {
            _audio[1].clip = sfx[0];
            _audio[1].Play();
        }
        _curStage.VictoryAnimations();
        fadeOutScreen.gameObject.SetActive(true);
        StartCoroutine(fadeOutScreen.FadeImage(true));
        _curStage.FadeLaserAndTurnOff();
        yield return new WaitForSeconds(3f);
        StartCoroutine(fadeOutScreen.FadeImage(false));
        _curStageNum += 1;
        SetStage(_curStageNum);
        stopSound = false;
        _disableKeys = false;
    }


    public IEnumerator EnemyHit(RaycastHit other)
    { //TODO game over or something
        _disableKeys = true;
        if(!_audio[1].isPlaying)
        {
            _audio[1].clip = sfx[1];
            _audio[1].Play();
        }
        other.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        _curStage.FadeLaserAndTurnOff();
        other.transform.gameObject.GetComponent<Animator>().Play("Bad");

        if (gameFreeze)
        {
            yield break;
        }
        //freeze robot
        _curStage.AllRobotFreeze(true);
        //sad animations - 1. enemy (with particles) 2.plants 3. robots.
        StartCoroutine(_curStage.LoseAnimations());
        
        yield return new WaitForSeconds(0f);
        gameFreeze = true;
    }

    public void PlayButtonPressed()
    {
        startScreen.SetActive(false);
    }
}
