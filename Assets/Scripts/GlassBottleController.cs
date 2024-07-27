using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlassBottleController : MonoBehaviour
{
    private Animator animator;
    bool picked = false;

    //moi lan cong or tru 0.4

    public Button myButton;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }


    void Start()
    {
        if (myButton != null)
        {
            myButton.onClick.AddListener(OnButtonClicked);
        }
    }

    void OnButtonClicked()
    {
        if (!picked)
        {
            Picked();
            picked = true;
        }
        else
        {
            UnPick();
            picked = false;
        }
    }

    public void Picked()
    {

        animator.SetTrigger("Picked");

    }

    public void UnPick()
    {

        animator.SetTrigger("UnPick");

    }
}
