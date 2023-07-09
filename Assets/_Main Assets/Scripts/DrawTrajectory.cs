using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GPHive.Game;
using GPHive.Core;
using MoreMountains.NiceVibrations;

public class DrawTrajectory : Singleton<DrawTrajectory>
{


    List<GameObject> LinePoints = new List<GameObject>();
    [SerializeField][Range(20, 50)] private int lineSegmentCount = 50;
    [SerializeField] LayerMask trajectoryCollisionLayer;
    [SerializeField] GameObject TrajetktoryBallPrefeb;
    [SerializeField] GameObject FinalPointTrajetktoryBall;
    Panye panye;
    GameObject Enemy, TutorialGo;

    Player player;
    float _aimAsistLimit;
    private void Start()
    {

        FinalPointTrajetktoryBall.SetActive(false);

        for (int i = 0; i < lineSegmentCount; i++)
        {
            LinePoints.Add(Instantiate(TrajetktoryBallPrefeb, Vector3.zero, Quaternion.identity));
        }
        foreach (var item in LinePoints)
        {
            item.SetActive(false);
        }

        _aimAsistLimit = Shooter.Instance.aimAsistLimit;
        player = Player.Instance;
    }

    Vector3 _panyeOutlineCloserPreviousLinePosition, _panyeOutlineCloserNextLinePosition;



    public void Trajectory(float seccond, Vector3 startPos, Vector3 helperPos, Vector3 endPos)
    {
        CreatPoints();
        for (int i = 0; i < lineSegmentCount; i++)
        {
            Vector3 _nextLinePosition = ExtensionMethods.QuadraticCurvePoint(startPos, helperPos, endPos, (float)i / lineSegmentCount);
            Vector3 _previousLinePosition = startPos;
            if (i > 0)
                _previousLinePosition = ExtensionMethods.QuadraticCurvePoint(startPos, helperPos, endPos, (float)(i - 1) / lineSegmentCount);

            LinePoints[i].transform.position = _nextLinePosition;

            RaycastHit hit;
            if (Physics.Linecast(_previousLinePosition, _nextLinePosition, out hit, 1 << LayerMask.NameToLayer("Default")))
            {

                for (int j = i; j < LinePoints.Count; j++)
                {
                    LinePoints[j].SetActive(false);
                }

                FinalPointTrajetktoryBall.SetActive(true);
                FinalPointTrajetktoryBall.transform.position = hit.point;

                if (player == null)
                    player = Player.Instance;

                if (player.Tutorial)
                {
                    if (hit.collider.gameObject == player.triggerObjeckts[player.triggerCount] && TutorialGo == null)
                    {
                        TutorialGo = player.triggerObjeckts[player.triggerCount];
                        player.selecktArrowObj[player.triggerCount].SetActive(false);

                        Shooter.Instance.aimAsist = player.triggerObjeckts[player.triggerCount].transform.position;

                        Shooter.Instance.aimAsistLimit = 100;

                        CustomEventManager.TriggerEvent("TutorialTargetTaken");
                    }

                }
                else
                {
                    if (hit.collider.tag == "Panye")
                    {
                        _panyeOutlineCloserPreviousLinePosition = _previousLinePosition;
                        _panyeOutlineCloserNextLinePosition = _nextLinePosition;

                        panye = hit.collider.GetComponent<Panye>();

                        Shooter.Instance.aimAsist = panye.transform.position;

                        panye.Board.GetComponent<Renderer>().material.SetFloat("_OutlineSize", 190);

                        if (!panye.BasketZoon.activeSelf)
                        {
                            MMVibrationManager.Haptic(HapticTypes.SoftImpact);
                            panye.BasketZoon.SetActive(true);
                            panye.BasketZoon.GetComponent<ParticleSystem>().Play();
                        }
                    }
                    else if (hit.collider.tag == "EnemyOutline")
                    {
                        Enemy = hit.collider.gameObject;
                        Enemy.GetComponent<Renderer>().material.SetFloat("_OutlineSize", 100);
                    }
                    else
                    {

                        BasketZoonAreaCloser();
                        FinalPointTrajetktoryBall.SetActive(false);

                    }
                }
                break;
            }
            else
            {
                FinalPointTrajetktoryBall.SetActive(false);
                BasketZoonAreaCloser();
            }

        }
    }
    void TutorialZoonAreaCloser()
    {


        if (player.Tutorial && TutorialGo != null)
        {
            TutorialGo = null;
            Shooter.Instance.aimAsistLimit = _aimAsistLimit;
            player.selecktArrowObj[player.triggerCount].SetActive(false);

            if (player.triggerCount < player.selecktArrowObj.Length - 1)
            {
                player.triggerCount++;
                CustomEventManager.TriggerEvent("TutorialReleaseCloser");
                player.selecktArrowObj[player.triggerCount].SetActive(true);
            }
            else
            {
                player.Tutorial = false;
                GameManager.Instance.Tutorial.SetActive(false);
            }
        }
    }

    void BasketZoonAreaCloser()
    {
        RaycastHit hit;

        if (_panyeOutlineCloserPreviousLinePosition != Vector3.zero)
        {
            if (Physics.Linecast(_panyeOutlineCloserPreviousLinePosition, _panyeOutlineCloserNextLinePosition, out hit, 1 << LayerMask.NameToLayer("Default")))
            {
                if (panye != null && hit.collider.tag == "Panye")
                {

                    if (panye.BasketZoon.activeSelf && Shooter.Instance.aimAsist == Vector3.zero)
                    {
                        panye.Board.GetComponent<Renderer>().material.SetFloat("_OutlineSize", 0);
                        panye.BasketZoon.SetActive(false);
                        panye = null;
                    }
                }
            }
        }

        if (Enemy != null)
        {
            FinalPointTrajetktoryBall.SetActive(false);
            Enemy.GetComponent<Renderer>().material.SetFloat("_OutlineSize", 0);
            Enemy = null;
        }

    }
    void BasketZoonAreaCloser(bool test)
    {
        RaycastHit hit;

        if (_panyeOutlineCloserPreviousLinePosition != Vector3.zero)
        {
            if (Physics.Linecast(_panyeOutlineCloserPreviousLinePosition, _panyeOutlineCloserNextLinePosition, out hit, 1 << LayerMask.NameToLayer("Default")))
            {
                if (panye != null && hit.collider.tag == "Panye")
                {

                    if (panye.BasketZoon.activeSelf)
                    {
                        panye.Board.GetComponent<Renderer>().material.SetFloat("_OutlineSize", 0);
                        panye.BasketZoon.SetActive(false);
                        panye = null;
                    }
                }

            }
        }


        if (Enemy != null)
        {
            FinalPointTrajetktoryBall.SetActive(false);
            Enemy.GetComponent<Renderer>().material.SetFloat("_OutlineSize", 0);
            Enemy = null;
        }
    }



    public void CreatPoints()
    {
        foreach (var item in LinePoints)
        {
            item.SetActive(true);
        }
    }
    public void HideLine()
    {
        FinalPointTrajetktoryBall.SetActive(false);
        foreach (var item in LinePoints)
        {
            item.SetActive(false);
        }

        BasketZoonAreaCloser(true);
        TutorialZoonAreaCloser();
        FinalPointTrajetktoryBall.SetActive(false);

    }
}
