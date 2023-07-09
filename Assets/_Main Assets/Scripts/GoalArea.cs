using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalArea : MonoBehaviour
{
    [SerializeField] ParticleSystem BasketCountParticle,BombParticle;
    [SerializeField] Animator animator;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
        {
            animator.SetTrigger("Goal");
            BasketCountParticle.Play();
            other.GetComponent<Ball>().ReverseBomb();

            ComboManager.Instance.ChangeStage();
            Invoke("bombParticlePlay",.25f);

            if (ComboManager.Instance.BoomBool)
            {
                ComboManager.Instance.BoomGO.SetActive(true);
                Invoke("closer", 1.3f);

            }




        }
    }

    void closer()
    {
        ComboManager.Instance.BoomGO.SetActive(false);
        ComboManager.Instance.BoomBool = false;
    }

    void bombParticlePlay()
    {
        BombParticle.Play();
    }

}
