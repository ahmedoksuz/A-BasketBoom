using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panye : MonoBehaviour
{
    public GameObject BasketZoon, Board;

    Vector3 basketZoonStartScale;

    [SerializeField] float basketZoonScaleMultipler;
    private void Start()
    {
        basketZoonStartScale = BasketZoon.transform.localScale;
    }

    private void Update()
    {
        if (ComboManager.Instance.mods == ComboManager.Mods.fever)
        {
            BasketZoon.transform.localScale = new Vector3(basketZoonScaleMultipler, basketZoonScaleMultipler, basketZoonScaleMultipler);
        }
        else
        {
            BasketZoon.transform.localScale = basketZoonStartScale;
        }

    }
}
