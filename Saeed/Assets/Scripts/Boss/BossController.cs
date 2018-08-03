using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BossController : MonoBehaviour {

    public enum Stage { Tutorial, Face, Arms, Legs, Outro }

    [Header("Component References")]
    public Stage currentStage;
    public Collider2D[] colliders;

    enum ActionState { Stand, HideAndShow, ConjureBombs, ChargeAndJump, SummonMinions }
    List<ActionState> actionList, actionQueue; 

    [Header("Effects References")]
    public ParticleSystem particleSystemGroundShake;
    public ParticleSystem particleSystemSoundWave;

    [Header("UI References")]
    public string layerToIgnore;

    Animator animator;
    Coroutine actionRoutine;
    BossStateMachine stateMachine = new BossStateMachine();


    private void Start()
    {
        animator = GetComponent<Animator>();

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer(layerToIgnore), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(layerToIgnore), LayerMask.NameToLayer(layerToIgnore), true);

        SetActionLists();
        SetStage();
    }

    public void EndStage()
    {
        currentStage++;
        SetStage();
    }

    void SetStage()
    {
        switch (currentStage)
        {
            default:
            case Stage.Tutorial:
                transform.position = new Vector2(transform.position.x, -7.5f);
                animator.enabled = false;
                break;

            case Stage.Face:
                transform.position = new Vector2(transform.position.x, -6);
                animator.enabled = true;
                CallNewAction();
                break;

            case Stage.Arms:
                transform.position = new Vector2(transform.position.x, -5);
                break;

            case Stage.Legs:
                transform.position = new Vector2(transform.position.x, -5);
                break;

            case Stage.Outro:
                Destroy(gameObject);
                break;
        }
        GetComponent<BossHealth>().SetStageIndex((int)currentStage);
    }

    void SetActionLists()
    {
        actionList = new List<ActionState>();
        actionList.Add(ActionState.HideAndShow);
        actionList.Add(ActionState.ConjureBombs);
        actionList.Add(ActionState.SummonMinions);

        actionQueue = new List<ActionState>();
        actionQueue.Add(ActionState.SummonMinions);
        actionQueue.Add(ActionState.ChargeAndJump);
        actionQueue.Add(ActionState.HideAndShow);
        actionQueue.Add(ActionState.ConjureBombs);
    }

    public void CallNewAction()
    {
        if (actionRoutine != null) StopCoroutine(actionRoutine);
        actionRoutine = StartCoroutine(Action((int)currentStage));
    }

    IEnumerator Action(int level)
    {
        stateMachine.ChangeState(new State_Stand(this, animator));

        yield return new WaitForSeconds(Random.Range(1f, 2f));

        PickAction();
        //stateMachine.ChangeState(new State_SummonMinions(this, Random.Range(4,6), animator, Spawner.instance, particleSystemSoundWave));
    }

    private void PickAction()
    {
        int randomPick = Random.Range(0, actionList.Count);
        ActionState randomAction = actionList[randomPick];
        switch (randomAction)
        {
            default:
            case ActionState.HideAndShow:
                stateMachine.ChangeState(new State_HideAndShow(this, Random.Range(0,3), transform, animator, particleSystemGroundShake));
                break;
            case ActionState.ConjureBombs:
                stateMachine.ChangeState(new State_ConjureBombs(this, animator, Spawner.instance));
                break;
            case ActionState.ChargeAndJump:
                stateMachine.ChangeState(new State_ChargeAndJump(this, animator));
                break;
            case ActionState.SummonMinions:
                stateMachine.ChangeState(new State_SummonMinions(this, Random.Range(0, 6), animator, Spawner.instance, particleSystemSoundWave));
                break;
        }

        actionQueue.Add(randomAction);
        actionList.RemoveAt(randomPick);
        actionList.Add(actionQueue[0]);
        actionQueue.RemoveAt(0);
    }

    public void SetDeathState()
    {
        StopAllCoroutines();
        foreach (Collider2D c in colliders) c.enabled = false;
        stateMachine.ChangeState(new State_Death(this, animator));
    }
}
