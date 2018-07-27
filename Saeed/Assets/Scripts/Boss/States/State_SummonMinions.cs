using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_SummonMinions : IBossState
{
    int pattern;
    Animator animator;
    BossController bossController;
    Spawner spawner;
    ParticleSystem soundWavePS;

    public State_SummonMinions(
        BossController bossController, int pattern, Animator animator, Spawner spawner, ParticleSystem soundWavePS)
    {
        this.bossController = bossController;
        this.pattern = pattern;
        this.animator = animator;
        this.spawner = spawner;
        this.soundWavePS = soundWavePS;
    }

    public void Enter()
    {
        animator.SetTrigger("Summon");
        soundWavePS.Play();
        bossController.StartCoroutine(Execute());
    }

    IEnumerator Execute()
    {
        yield return new WaitForSeconds(.2f);
        PatternInterleaved();

        yield return new WaitForSeconds(2);
        animator.SetTrigger("Toggle");
        soundWavePS.Stop();
        bossController.CallNewAction();
    }

    private void PatternInterleaved()
    {
        for(int i = 0; i < 10; i++)
        {
            Vector2 position = new Vector2((i*2) - 9, 5);
            GameObject miniMushroom = spawner.GetFromPool(1, position + Vector2.up * 1);
            miniMushroom.GetComponent<MiniMushroom>().PrepareToFall(position, (i % 2 == 0) ? 0 : 2);
        }
    }
}
