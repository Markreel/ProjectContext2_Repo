using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomeBehaviour : MonoBehaviour
{
    private Animator animator;
    [SerializeField] bool isDomeA = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    public void ActivateAnimator()
    {
        animator.enabled = true;
        if (!isDomeA) { animator.SetTrigger("Close"); }
    }
}
