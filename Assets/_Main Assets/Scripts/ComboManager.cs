using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GPHive.Core;

public class ComboManager : Singleton<ComboManager>
{

    public enum Mods
    {
        normal, fever
    }

    public Mods mods;
    enum Stages
    {
        stage0, stage1, stage2, stage3
    }
    [SerializeField] Stages stage = Stages.stage0;
    [SerializeField] Image fillerImage;

    [SerializeField] float fillReductionSeccond = 7;
    [SerializeField] float[] stagesAmounts = new float[4];

    public GameObject feverModBackground, BoomGO;
    [SerializeField] GameObject[] ComboXes = new GameObject[2];
    Animator animator;
    float time;

    public bool BoomBool;


    private void Start()
    {
        fillerImage.fillAmount = stagesAmounts[0];
        stage = Stages.stage0;
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (GameManager.Instance.GameState == GameState.End)
        {
            if (gameObject.activeSelf)
            {
                gameObject.gameObject.SetActive(false);
            }

        }
    }

    public void ChangeStage()
    {
        fillerImage.DOKill();
        switch (stage)
        {
            case Stages.stage0:
                stage++;
                Filling();
                break;
            case Stages.stage1:

                ComboSpriteShowMeth(ComboXes[0]);
                stage++;
                Filling();
                break;
            case Stages.stage2:

                stage++;
                fillerImage.DOFillAmount(stagesAmounts[(int)stage], .1f);
                ComboSpriteShowMeth(ComboXes[1]);
                mods = Mods.fever;
                break;
            case Stages.stage3:
                stage = Stages.stage0;
                fillerImage.DOFillAmount(stagesAmounts[0], .5f);
                mods = Mods.normal;
                break;

        }

        if (mods == Mods.normal)
        {
            if (feverModBackground.activeSelf)
            {
                feverModBackground.SetActive(false);
                BoomBool = true;
            }
            animator.enabled = false;
        }
        else
        {
            animator.enabled = true;

        }
    }

    void Filling()
    {
        fillerImage.DOFillAmount(stagesAmounts[(int)stage], .1f).OnComplete(() =>
        {
            ReductionFill();
        });

    }
    void ReductionFill()
    {
        if ((int)stage != 0)
        {
            fillerImage.DOFillAmount(stagesAmounts[(int)stage - 1], fillReductionSeccond).SetEase(Ease.Linear).OnComplete(() => { stage--; ReductionFill(); });
        }


    }
    GameObject Closeing;
    void ComboSpriteShowMeth(GameObject _closeing)
    {
        if (Closeing != null)
        {
            ComboSpriteHideMeth();
        }
        Closeing = _closeing;
        Closeing.SetActive(true);
        Invoke("ComboSpriteHideMeth", 1);
        Invoke("feverModBackgroundOpenMeth", .6f);

    }

    void ComboSpriteHideMeth()
    {
        Closeing.SetActive(false);
        Closeing = null;
    }
    void feverModBackgroundOpenMeth()
    {
        if (Closeing == ComboXes[1])
            feverModBackground.SetActive(true);
    }





}
