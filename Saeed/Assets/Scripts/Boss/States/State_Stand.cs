using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Stand : IBossState
{
    Animator animator;
    BossController bossController;

    public State_Stand(BossController bossController, Animator animator)
    {
        this.animator = animator;
        this.bossController = bossController;
    }

    public void Enter()
    {
    }
}
