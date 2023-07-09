using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GPHive.Core;
using DG.Tweening;
using MoreMountains.NiceVibrations;

public class StageController : Singleton<StageController>
{
    public StageStruck[] Stages;
    [HideInInspector] public int activeStage = 0;
    int EnemyCount;
    public int killedEnemy;
    Vector3[] Points;
    void Start()
    {

        for (int i = 0; i < Stages.Length; i++)
        {
            Stages[i].stageEnemyCount += Stages[i].Enemies.Length;

            for (int j = 0; j < Stages[i].spawner.Length; j++)
            {
                Stages[i].stageEnemyCount += Stages[i].spawner[j].spawnCount;
            }
        }

        foreach (var item in Stages)
        {
            EnemyCount += item.stageEnemyCount;
        }
        StartStage(Stages[activeStage]);

    }
    private void OnEnable()
    {
        GameManager.Instance.kilCount = 0;
        GameManager.Instance.moneyCount = 0;
    }
    public void KillEnemy()
    {
        killedEnemy++;
        GameManager.Instance.kilCount += 1;
        GameManager.Instance.moneyCount += 50;

        if (killedEnemy == Stages[activeStage].stageEnemyCount)
        {
            activeStage++;
            killedEnemy = 0;

            if (activeStage > Stages.Length - 1)
            {
                DrawTrajectory.Instance.HideLine();
                GameManager.Instance.WinLevel();
            }
            else
            {

                Points = new Vector3[0];
                Points = new Vector3[Stages[activeStage].StagePoint.Length];

                for (int i = 0; i < Stages[activeStage].StagePoint.Length; i++)
                {
                    Points[i] = Stages[activeStage].StagePoint[i].position;

                }

                MMVibrationManager.Haptic(HapticTypes.SoftImpact);
                DrawTrajectory.Instance.HideLine();
                Shooter.Instance.ShootStop = true;
                transform.DOPath(Points, 3).OnComplete(() => { StartStage(Stages[activeStage]); Shooter.Instance.ChargeBall(); Shooter.Instance.ShootStop = false; });

            }
        }

    }

    void StartStage(StageStruck stage)
    {
        for (int i = 0; i < stage.Enemies.Length; i++)
        {
            stage.Enemies[i].StageStart = true;
        }

        for (int i = 0; i < stage.spawner.Length; i++)
        {
            stage.spawner[i].StageStart = true;
        }
    }
    [System.Serializable]
    public class StageStruck
    {
        public Transform[] StagePoint;
        public Spawner[] spawner;
        public Enemy[] Enemies;
        public int stageEnemyCount;

    }

}
