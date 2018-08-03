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
        switch (pattern)
        {
            default:
            case 0:
                PatternInterleaved(false);
                break;
            case 1:
                PatternInterleaved(true);
                break;
            case 2:
                PatternLeftAndRight(false);
                break;
            case 3:
                PatternLeftAndRight(true);
                break;
            case 4:
                PatternSidesToCenter(false);
                break;
            case 5:
                PatternSidesToCenter(true);
                break;
        }

        yield return new WaitForSeconds(2.5f);
        animator.SetTrigger("Toggle");
        soundWavePS.Stop();
        bossController.CallNewAction();
    }

    private void PatternInterleaved(bool mirror)
    {
        int firstTimer, secondTimer;
        if (mirror) {
            firstTimer = 0;
            secondTimer = 2;
        } else {
            firstTimer = 2;
            secondTimer = 0;
        }

        for(int i = 0; i < 10; i++)
        {
            Vector2 position = new Vector2((i*2) - 9, 5);
            GameObject miniMushroom = spawner.GetFromPool(1, position + Vector2.up * 1);
            miniMushroom.GetComponent<MiniMushroom>().PrepareToFall(position, (i % 2 == 0) ? firstTimer : secondTimer);
        }
    }

    private void PatternLeftAndRight(bool mirror)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 position = new Vector2((i * 2) - 9, 5);
            GameObject miniMushroom = spawner.GetFromPool(1, position + Vector2.up * 1);
            if(i < 5) {
                miniMushroom.GetComponent<MiniMushroom>().PrepareToFall(position, mirror ? 0 : 2);
            } else {
                miniMushroom.GetComponent<MiniMushroom>().PrepareToFall(position, mirror ? 2 : 0);
            }
        }
    }

    private void PatternSidesToCenter(bool mirror)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 position = new Vector2((i * 2) - 9, 5);
            GameObject miniMushroom = spawner.GetFromPool(1, position + Vector2.up * 1);

            int j;
            if (i < 5)
                j = i - 5;
            else
                j = i - 4;

            float time;
            if (mirror) 
                time = .5f * (Mathf.Abs(j) - 1);
            else
                time = 2f - .5f * (Mathf.Abs(j) - 1);
            
            miniMushroom.GetComponent<MiniMushroom>().PrepareToFall(position, time);
        }
    } 
}
