using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_HideAndShow : IBossState
{
    int fakeOuts;
    Transform transform;
    Animator animator;
    ParticleSystem groundShakeParticles;
    BossController bossController;

    public State_HideAndShow(
    BossController bossController, int fakeOuts, Transform transform, Animator animator, ParticleSystem groundShakeParticles)
    {
        this.bossController = bossController;
        this.fakeOuts = fakeOuts;
        this.transform = transform;
        this.animator = animator;
        this.groundShakeParticles = groundShakeParticles;
    }

    public void Enter()
    {
        bossController.StartCoroutine(Execute());
    }

    IEnumerator Execute()
    {
        for (int i = -1; i < fakeOuts; i++)
        {
            animator.SetTrigger("Hide");
            yield return new WaitForSeconds(1);
            List<int> possibleX = new List<int>{ -6, 0, 6 };
            if(transform.position.x < -3) {
                possibleX.RemoveAt(0);
            } else if(transform.position.x < 3) {
                possibleX.RemoveAt(1);
            } else {
                possibleX.RemoveAt(2);
            }

            int xSpawnPosition = possibleX[Random.Range(0, possibleX.Count)];
            groundShakeParticles.transform.position = new Vector2(xSpawnPosition, groundShakeParticles.transform.position.y);
            groundShakeParticles.Play();

            yield return new WaitForSeconds(1.2f);
            groundShakeParticles.Stop();
            transform.position = new Vector2(xSpawnPosition, transform.position.y);
            animator.SetTrigger("Toggle");
        }

        yield return new WaitForSeconds(1);
        bossController.CallNewAction();
    }
}
