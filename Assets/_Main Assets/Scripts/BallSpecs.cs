using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Ball Specs", fileName = "Ball Specs")]
public class BallSpecs : ScriptableObject
{
    public LayerMask ballCollisionLayer;
    [Range(0, 1)] public float energyLost;
    public float BallDefaultBombRedius, BallBombMaxRedius, rotateSpeed;
    [HideInInspector] public float UsingBombRedius;

    private void Start()
    {
        UsingBombRedius = BallDefaultBombRedius;
    }
    public AnimationCurve pathCurve;

}
