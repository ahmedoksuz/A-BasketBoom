using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using GPHive.Core;
using MoreMountains.NiceVibrations;


public class Enemy : MonoBehaviour
{
    Transform playerTransform;
    [SerializeField] float proximityDistance;
    NavMeshAgent navMesh;
    Animator animator;
    [SerializeField] Rigidbody SpineRigidbody;
    [SerializeField] GameObject[] Outfits;
    [SerializeField] GameObject Angry, Death, Fall;

    [SerializeField] SkinnedMeshRenderer[] SkindMaterialMeshs;
    [SerializeField] MeshRenderer[] MaterialMeshs;
    [SerializeField] Material deathMaterial;
    [SerializeField] ParticleSystem fallingParticle;
    [SerializeField] Material NormalMaterial, NiggaMaterial;
    [SerializeField] SkinnedMeshRenderer MainSikindMesh;

    // NavMeshObstacle navMeshObstacle;

    LevelSettings levelSettings;

    bool Finish;
    [HideInInspector] public bool StageStart;

    bool TriggerActive = true;
    bool didIExplode = false;


    void Start()
    {
        playerTransform = FindObjectOfType<Player>().transform;
        navMesh = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Angry.SetActive(true);
        Outfits[Random.Range(0, Outfits.Length)].SetActive(true);
        levelSettings = GameObject.FindObjectOfType<LevelSettings>();
        levelSettings.Enemies.Add(gameObject);
        transform.parent = null;

        int rnd = Random.Range(0, 100);
        if (rnd > 30)
            MainSikindMesh.material = NormalMaterial;
        else
            MainSikindMesh.material = NiggaMaterial;
        // navMeshObstacle = GetComponent<NavMeshObstacle>();
    }


    void Update()
    {
        if (GameManager.Instance.GameState == GameState.Playing && StageStart)
        {

            if (navMesh.enabled)
            {
                if (animator.GetInteger("Walking") == 0)
                    animator.SetInteger("Walking", Random.Range(1, 3));

                if (gameObject.tag != "Untagged")
                {
                    if (Vector3.Distance(transform.position, playerTransform.position) < 1)
                    {
                        GameManager.Instance.LoseLevel();
                        animator.SetTrigger("Punch");
                        gameObject.tag = "Untagged";
                        animator.SetInteger("Walking", 0);
                        Invoke("RotateCam", 1.5f);
                    }
                    if (Vector3.Distance(transform.position, playerTransform.position) < proximityDistance)
                    {

                        navMesh.SetDestination(playerTransform.position);
                    }
                    else
                    {
                        navMesh.SetDestination(new Vector3(transform.position.x, transform.position.y, playerTransform.position.z));
                    }
                }



            }

        }
        else if (GameManager.Instance.GameState == GameState.End)
        {
            if (gameObject.tag != "Untagged")
            {
                navMesh.speed = 0;
                gameObject.tag = "Untagged";
                animator.SetTrigger("FinishLose");
                animator.SetInteger("Walking", 0);

            }
        }


    }


    void RotateCam()
    {
        playerTransform.DORotate(new Vector3(0, 0, -90), .4f).OnComplete(() =>
        {
            CinemachineShake.Instance.ShakeCamera(4, .3f);
            MMVibrationManager.Haptic(HapticTypes.Failure);
        });
    }

    public void Bomb()
    {
        if (!didIExplode)
        {

            gameObject.tag = "Untagged";
            fallingParticle.Stop();
            //  navMeshObstacle.enabled = true;
            navMesh.enabled = false;
            animator.enabled = false;
            SpineRigidbody.AddForce(Vector3.up * 500);
            Angry.SetActive(false);
            Death.SetActive(true);

            foreach (var item in SkindMaterialMeshs)
            {
                item.material = deathMaterial;
            }
            foreach (var item in MaterialMeshs)
            {
                item.material = deathMaterial;
            }
            didIExplode = true;
            TriggerActive = false;
            StageController.Instance.KillEnemy();
            Invoke("SetActiveFalseMeth", 5f);
        }


    }

    public void Falling()
    {
        if (TriggerActive)
        {
            TriggerActive = false;
            fallingParticle.Play();
            //  navMeshObstacle.enabled = true;
            navMesh.enabled = false;

            Angry.SetActive(false);
            Fall.SetActive(true);

            animator.SetTrigger("Fall");
            animator.SetInteger("Walking", 0);
            transform.DOMove(new Vector3(transform.position.x, transform.position.y, transform.position.z + 10), 2);
        }


    }

    public void GetUpAnimationEvents()
    {
        Invoke("GetUp", .35f);
    }
    void GetUp()
    {
        TriggerActive = true;
        fallingParticle.Stop();
        //  navMeshObstacle.enabled = false;
        Fall.SetActive(false);
        Angry.SetActive(true);
        navMesh.enabled = true;
        animator.SetInteger("Walking", Random.Range(1, 3));

    }

    void SetActiveFalseMeth()
    {
        gameObject.SetActive(false);
    }


}
