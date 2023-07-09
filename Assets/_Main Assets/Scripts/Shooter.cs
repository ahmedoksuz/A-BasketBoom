using UnityEngine;
using System.Collections;
using GPHive.Game;
using GPHive.Core;
using MoreMountains.NiceVibrations;

public class Shooter : Singleton<Shooter>
{


    public GameObject ballPrefab;
    public Transform shotPoint;

    GameObject objBall;
    float startTime;

    [SerializeField] private float trajectoryMultiplier;

    [SerializeField]
    Transform startPoint, endPoint, helperPoint, aimAsistPoint, aimAsistHelperPoint;
    Vector3 endStartPos, helperStartPos;
    public float sensitivitiy, aimAsistLimit;

    [SerializeField] private Vector2 limitX;
    [SerializeField] private Vector2 limitY;

    [HideInInspector] public Vector3 aimAsist;
    [SerializeField] float ballMovmentMaxTime, ballMovmentMinTime;
    [SerializeField] Vector3 Test;

    [HideInInspector] public bool ShootStop = false;
    private void OnEnable()
    {
        objBall = null;
        ObjectPooling.Instance.DepositAll();
        ChargeBall();
        endStartPos = endPoint.localPosition;
        helperStartPos = helperPoint.localPosition;
        Shooter.Instance.ShootStop = false;
    }


    private void Update()
    {
        if (Input.touchCount > 0 && objBall != null && !ShootStop)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                if (aimAsist != Vector3.zero)
                {
                    if (Vector2.Distance(Vector2.zero, touch.deltaPosition * sensitivitiy) < aimAsistLimit)
                        AimAsistMeth(touch);
                    else
                    {
                        aimAsist = Vector3.zero;
                    }

                }

                CurveCalculation(touch);

                Trajectory();

            }
            else if (touch.phase == TouchPhase.Ended)
            {

                Shoot();
                aimAsist = Vector3.zero;
            }

        }
    }

    void CurveCalculation(Touch _touch)
    {
        startPoint.position = objBall.transform.position;
        Vector2 _endDeltaPosition = _touch.deltaPosition * sensitivitiy;
        Vector2 _helperDeltaPosition = _touch.deltaPosition * sensitivitiy;
        _endDeltaPosition.y /= 2f;

        if ((endPoint.position + (Vector3)_endDeltaPosition).y > limitY.x && (endPoint.position + (Vector3)_endDeltaPosition).y < limitY.y &&
        (endPoint.position + (Vector3)_endDeltaPosition).x > transform.parent.position.x + limitX.x && (endPoint.position + (Vector3)_endDeltaPosition).x < transform.parent.position.x + limitX.y
        )
        {
            endPoint.Translate(_endDeltaPosition);
            helperPoint.position = endPoint.position;
            helperPoint.position = new Vector3(((endPoint.position - startPoint.position) / 4).x + transform.parent.position.x, endPoint.position.y * 1.01f, ((endPoint.position - startPoint.position) / 4).z + transform.parent.position.z);
            Test = helperPoint.position;
        }
    }

    void AimAsistMeth(Touch _touch)
    {
        startPoint.position = objBall.transform.position;
        aimAsistPoint.position = aimAsist;
        aimAsistHelperPoint.position = aimAsistPoint.position;
        aimAsistHelperPoint.position = new Vector3(((aimAsistPoint.position - startPoint.position) / 4).x + transform.parent.position.x, aimAsistPoint.position.y * 1.05f, ((aimAsistPoint.position - startPoint.position) / 4).z + transform.parent.position.z);

    }

    void Trajectory()
    {
        float second = Mathf.Lerp(ballMovmentMaxTime, ballMovmentMinTime, (endPoint.position.y - (limitY.x - 1)) / ((limitY.y + 1) - (limitY.x - 1)));

        if (aimAsist == Vector3.zero)
            DrawTrajectory.Instance.Trajectory(second, startPoint.position, helperPoint.position, endPoint.position);
        else
            DrawTrajectory.Instance.Trajectory(second, startPoint.position, aimAsistHelperPoint.position, aimAsistPoint.position);

    }

    void Shoot()
    {
        DrawTrajectory.Instance.HideLine();
        CustomEventManager.TriggerEvent("Shooted");

        float second = Mathf.Lerp(ballMovmentMaxTime, ballMovmentMinTime, (endPoint.position.y - (limitY.x - 1)) / ((limitY.y + 1) - (limitY.x - 1)));

        MMVibrationManager.Haptic(HapticTypes.SoftImpact);

        if (objBall.activeInHierarchy)
        {


            if (aimAsist == Vector3.zero)
                objBall.GetComponent<Ball>().ShootCoroutine(second, startPoint.position, helperPoint.position, endPoint.position);
            else
                objBall.GetComponent<Ball>().ShootCoroutine(second, startPoint.position, aimAsistHelperPoint.position, aimAsistPoint.position);

            objBall = null;
            Invoke("ChargeBall", .3f);
            endPoint.localPosition = endStartPos;
            helperPoint.localPosition = helperStartPos;
        }
    }

    public void ChargeBall()
    {
        objBall = ObjectPooling.Instance.GetFromPool("Ball");
        objBall.SetActive(true);
        objBall.transform.position = shotPoint.transform.position;
    }






}


