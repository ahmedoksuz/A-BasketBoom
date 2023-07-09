using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;
using GPHive.Core;

public class Ball : MonoBehaviour
{
    [SerializeField] GameObject bombRender;
    [SerializeField] GameObject ballRender;

    [SerializeField] PhysicMaterial BombPhysicMat, BallPhysicMat;

    [SerializeField] Rigidbody rigidbody;
    Coroutine ballMovementCoroutine;

    [SerializeField] BallSpecs ballSpecs;

    SphereCollider collider;
    [SerializeField] ParticleSystem bombParticle, toBombParticle, enemyStanParticle /*ballFlame*/;
    Vector3 previousFramePos;

    [SerializeField] Vector3 velocity;
    bool isRotate, ballCollisisionBool = true;

    public Vector3 Velocity { get { return velocity; } }
    private void OnEnable()
    {
        collider = GetComponent<SphereCollider>();
        previousFramePos = transform.position;
        gameObject.tag = "Ball";
        collider.material = BallPhysicMat;
        rigidbody.isKinematic = true;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //  ballFlame.gameObject.SetActive(true);
        bombRender.SetActive(false);
        ballRender.SetActive(true);
        collider.isTrigger = false;
        isRotate = false;
        ballCollisisionBool = true;
    }

    private void Update()
    {
        if (isRotate)
        {
            transform.Rotate(new Vector3(ballSpecs.rotateSpeed, 0, 0) * Time.deltaTime);
        }


        velocity = (transform.position - previousFramePos) / Time.fixedDeltaTime;
        previousFramePos = transform.position;


        if (ComboManager.Instance.mods == ComboManager.Mods.fever)
        {
            ballSpecs.UsingBombRedius = ballSpecs.BallBombMaxRedius;
            //  ballFlame.Play();
        }
        else
        {
            ballSpecs.UsingBombRedius = ballSpecs.BallDefaultBombRedius;
            //ballFlame.Stop();
        }

    }
    private void FixedUpdate()
    {
        // RaycastHit hit;
        // if (Physics.Linecast(transform.position, previousFramePos, out hit, 1 << LayerMask.NameToLayer("Default")))
        // {
        //     if (hit.collider.tag != "Ground" && hit.collider.tag != "EnemyOutline" && hit.collider.tag != "Panye")
        //     {
        //         if (ballMovementCoroutine != null)
        //         {
        //             StopAllCoroutines();

        //             if (hit.collider.tag == "Enemy")
        //                 hit.collider.gameObject.transform.root.GetComponent<Enemy>().Falling();

        //             Debug.Log(hit.collider.name);
        //         }

        //         transform.position = previousFramePos;
        //         // rigidbody.isKinematic = false;
        //         // BallCollision(hit.normal);
        //     }
        // }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("Bomb"))
        {
            if (other.gameObject.CompareTag("Ground"))
                BombMeth();
        }

    }

    private void OnCollisionEnter(Collision other)
    {
        isRotate = false;
        if (other.collider.CompareTag("Enemy") && gameObject.CompareTag("Ball") && GameManager.Instance.GameState == GameState.Playing)
        {
            MMVibrationManager.Haptic(HapticTypes.MediumImpact);
            enemyStanParticle.Play();
            other.transform.root.GetComponent<Enemy>().Falling();
        }

        BallCollision(other.contacts[0].normal);

        if (gameObject.CompareTag("Ball") && other.collider.CompareTag("Ground"))
        {

            // ballFlame.gameObject.SetActive(false);
            gameObject.tag = ("Untagged");
        }

    }

    public void ReverseBomb()
    {
        isRotate = false;

        //ballFlame.Stop();
        // ballFlame.gameObject.SetActive(false);
        gameObject.tag = ("Bomb");
        toBombParticle.Play();
        ballRender.SetActive(false);
        bombRender.SetActive(true);

        collider.material = BombPhysicMat;
        rigidbody.velocity = Vector3.down * 15;
        MMVibrationManager.Haptic(HapticTypes.SoftImpact);
        CinemachineShake.Instance.ShakeCamera(3, .3f);
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        collider.isTrigger = true;
    }

    void BombMeth()
    {
        //bombParticle.Play();
        gameObject.tag = ("Untagged");
        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, ballSpecs.UsingBombRedius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.tag == "Enemy")
            {
                hitCollider.transform.root.GetComponent<Enemy>().Bomb();
            }
        }
        bombRender.SetActive(false);
        rigidbody.isKinematic = true;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }



    private void BallCollision(Vector3 normal)
    {
        if (ballMovementCoroutine != null && ballCollisisionBool)
        {
            if (rigidbody.isKinematic == true)
            {
                ballCollisisionBool = false;
                StopCoroutine(ballMovementCoroutine);
                rigidbody.isKinematic = false;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
                Vector3 _reflection = (velocity - 2 * (Vector3.Dot(velocity, normal)) * normal).normalized;
                rigidbody.velocity = _reflection * (velocity.magnitude - velocity.magnitude * ballSpecs.energyLost);
            }
        }

    }

    public void ShootCoroutine(float timeInSeconds, Vector3 startPoint, Vector3 helperPoint, Vector3 endPoint)
    {
        isRotate = true;
        ballMovementCoroutine = StartCoroutine(Timer(timeInSeconds, startPoint, helperPoint, endPoint));
    }

    public IEnumerator Timer(float second, Vector3 startPoint, Vector3 helperPos, Vector3 endPos)
    {
        transform.GetComponent<Rigidbody>().isKinematic = true;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        float time = 0;
        while (time < second)
        {
            time += Time.deltaTime;

            transform.position = ExtensionMethods.QuadraticCurvePoint(startPoint, helperPos, endPos, ballSpecs.pathCurve.Evaluate(time / second));
            yield return new WaitForEndOfFrame();
        }

        ApplyVelocityAfterCoroutine();
    }

    public void ApplyVelocityAfterCoroutine()
    {
        isRotate = false;
        rigidbody.isKinematic = false;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rigidbody.velocity = velocity * 2;
    }


    bool DrawGizmos = false;
    private void OnDrawGizmos()
    {
        if (DrawGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, ballSpecs.UsingBombRedius);
        }

    }


}
