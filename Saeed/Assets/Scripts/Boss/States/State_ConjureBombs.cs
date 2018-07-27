using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_ConjureBombs : IBossState
{
    Animator animator;
    BossController bossController;
    Spawner spawner;

    public State_ConjureBombs(BossController bossController, Animator animator, Spawner spawner)
    {
        this.bossController = bossController;
        this.animator = animator;
        this.spawner = spawner;
    }

    public void Enter()
    {
        animator.SetTrigger("Conjure");
        bossController.StartCoroutine(Execute());
    }

    IEnumerator Execute()
    {
        yield return new WaitForSeconds(.5f);
        SetBomb(Vector2.up * Random.Range(0f, 4f) + Vector2.right * Random.Range(-8, -3));
        yield return new WaitForSeconds(.3f);
        SetBomb(Vector2.up * Random.Range(0f, 4f) + Vector2.right * Random.Range( 3,  8));

        yield return new WaitForSeconds(2);
        animator.SetTrigger("Toggle");
        bossController.CallNewAction();
    }

    private void SetBomb(Vector2 position)
    {
        GameObject _bomb = spawner.GetFromPool(0, position);
        _bomb.GetComponent<Bomb>().Spawn();
    }
}
