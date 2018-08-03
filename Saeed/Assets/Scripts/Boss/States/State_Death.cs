using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Death : IBossState
{
    Animator animator;
    BossController bossController;

    public State_Death(BossController bossController, Animator animator)
    {
        this.bossController = bossController;
        this.animator = animator;
    }

    public void Enter()
    {
        animator.SetTrigger("Die");
    }

    IEnumerator Execute()
    {
        yield return null;
    }
}
