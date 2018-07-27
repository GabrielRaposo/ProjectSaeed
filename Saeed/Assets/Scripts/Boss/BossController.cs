using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BossController : MonoBehaviour {

    public enum Stage { Tutorial, Face, Arms, Legs, Outro }
    public Stage currentStage;

    enum ActionState { Stand, HideAndShow, ConjureBombs, ChargeAndJump, SummonMinions }
    List<ActionState> actionList, actionQueue; 

    [Header("Effects References")]
    public ParticleSystem particleSystemGroundShake;
    public ParticleSystem particleSystemSoundWave;

    [Header("UI References")]
    public TextMeshProUGUI stageTitle;
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
                StartCoroutine(SetTitle("Stage 0: Tutorial"));
                transform.position = new Vector2(transform.position.x, -7.5f);
                animator.enabled = false;
                break;

            case Stage.Face:
                StartCoroutine(SetTitle("Stage 1: Face Reveal"));
                transform.position = new Vector2(transform.position.x, -6);
                animator.enabled = true;
                CallNewAction();
                break;

            case Stage.Arms:
                StartCoroutine(SetTitle("Stage 2: Armed and Ready"));
                transform.position = new Vector2(transform.position.x, -5);
                break;

            case Stage.Legs:
                StartCoroutine(SetTitle("Stage 3: Leg day!"));
                break;

            case Stage.Outro:
                StartCoroutine(SetTitle("You win."));
                Destroy(gameObject);
                break;
        }
        GetComponent<BossHealth>().SetStageIndex((int)currentStage);
    }

    IEnumerator SetTitle(string text)
    {
        stageTitle.text = text;
        stageTitle.enabled = true;
        yield return new WaitForSeconds(3);
        stageTitle.enabled = false;
    }

    void SetActionLists()
    {
        actionList = new List<ActionState>();
        actionList.Add(ActionState.HideAndShow);
        actionList.Add(ActionState.ConjureBombs);
        actionList.Add(ActionState.SummonMinions);

        actionQueue = new List<ActionState>();
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

        yield return new WaitForSeconds(Random.Range(2f, 3f));

        PickAction();
        //stateMachine.ChangeState(new State_HideAndShow(this, 2, transform, animator, particleSystemGroundShake));
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
                stateMachine.ChangeState(new State_SummonMinions(this, 0, animator, Spawner.instance, particleSystemSoundWave));
                break;
        }

        actionQueue.Add(randomAction);
        actionList.RemoveAt(randomPick);
        actionList.Add(actionQueue[0]);
        actionQueue.RemoveAt(0);
    }

}
