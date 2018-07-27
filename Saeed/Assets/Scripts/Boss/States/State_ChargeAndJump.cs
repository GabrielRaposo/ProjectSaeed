using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_ChargeAndJump : IBossState
{
    BossController bossController;
    Animator animator;

    public State_ChargeAndJump(BossController bossController, Animator animator)
    {
        this.bossController = bossController;
        this.animator = animator;
    }

    public void Enter()
    {
        animator.SetTrigger("Jump");
        bossController.StartCoroutine(Execute());
    }

    IEnumerator Execute()
    {
        yield return new WaitForSeconds(2);
        animator.SetTrigger("Toggle");

        yield return new WaitForSeconds(1.5f);
        bossController.CallNewAction();
    }
}
