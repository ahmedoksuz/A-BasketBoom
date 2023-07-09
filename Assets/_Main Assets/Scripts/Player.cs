using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GPHive.Core;

public class Player : Singleton<Player>
{
    public bool Tutorial;

    public GameObject[] triggerObjeckts;
    public GameObject[] selecktArrowObj;

    public int triggerCount;

    [SerializeField] BallSpecs ballSpecs;

    float startBallDefaultBombRedius;



    private void Start()
    {
        startBallDefaultBombRedius = ballSpecs.BallDefaultBombRedius;
        if (Tutorial)
        {
            ballSpecs.BallDefaultBombRedius = 50;
            Time.timeScale = .5f;
            triggerCount = 0;
            selecktArrowObj[1].SetActive(false);
            selecktArrowObj[0].SetActive(true);

            GameManager.Instance.Tutorial.SetActive(true);
            GameManager.Instance.TapToStart.enabled = false;
        }
        else
        {
            GameManager.Instance.Tutorial.SetActive(false);
        }

    }
    private void Update()
    {
        if (triggerCount == selecktArrowObj.Length)
        {
            GameManager.Instance.TapToStart.enabled = true;
        }

    }


    private void OnEnable()
    {
        CustomEventManager.Subscribe("TutorialTargetTaken", TargetTaken);
        CustomEventManager.Subscribe("Shooted", shoot);
        CustomEventManager.Subscribe("TutorialReleaseCloser", TutorialReleaseCloser);
        EventManager.LevelFailed += nextLevel;
        EventManager.LevelSuccessed += nextLevel;
    }

    private void OnDisable()
    {
        CustomEventManager.Unsubscribe("TutorialTargetTaken", TargetTaken);
        CustomEventManager.Unsubscribe("Shooted", shoot);
        CustomEventManager.Unsubscribe("TutorialReleaseCloser", TutorialReleaseCloser);
        EventManager.LevelFailed -= nextLevel;
        EventManager.LevelSuccessed -= nextLevel;

    }

    void nextLevel()
    {
        GameManager.Instance.Tutorial.SetActive(false);
        Time.timeScale = 1;
        ballSpecs.BallDefaultBombRedius = 3;
    }


    void shoot()
    {
        if (Tutorial)
        {
            Time.timeScale = .5f;
            GameManager.Instance.Hold.SetActive(true);
        }


    }

    void TargetTaken()
    {
        GameManager.Instance.Relese.SetActive(true);
        GameManager.Instance.Hold.SetActive(false);

    }

    void TutorialReleaseCloser()
    {
        GameManager.Instance.Relese.SetActive(false);

    }










}
