using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GPHive.Core;

public class Spawner : MonoBehaviour
{

    public int spawnCount;
    [SerializeField] float SeccontPer;
    [SerializeField] float StartWaitTime;
    [SerializeField] Transform SpawnPoint;
    [SerializeField] GameObject EnemyPrefeb;

    bool startWaitTimeBoll = false;

    float time;
    [HideInInspector] public bool StageStart;
    void Update()
    {
        if (GameManager.Instance.GameState == GameState.Playing && StageStart)
        {
            time += Time.deltaTime;
            if (startWaitTimeBoll || time > StartWaitTime)
            {
                startWaitTimeBoll = true;

                if (spawnCount > 0)
                {
                    if (time >= SeccontPer)
                    {
                        time = 0;
                        SpawnerMeth();
                    }
                }
            }
        }
    }

    void SpawnerMeth()
    {
        Instantiate(EnemyPrefeb, SpawnPoint.position, EnemyPrefeb.transform.rotation).GetComponent<Enemy>().StageStart = true;

        spawnCount--;
    }
}
