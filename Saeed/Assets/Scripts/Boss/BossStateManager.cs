using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossStateManager : MonoBehaviour {
    [Header("UI References")]
    public TextMeshProUGUI stageTitle;

    public enum Stage { Tutorial, Face, Arms, Legs, Outro }
    public Stage currentStage;

    Animator _animator;
    BossMovement bossMovement;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        bossMovement = GetComponent<BossMovement>();
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
                _animator.enabled = false;
                break;

            case Stage.Face:
                StartCoroutine(SetTitle("Stage 1: Face Reveal"));
                transform.position = new Vector2(transform.position.x, -6);
                _animator.enabled = true;
                bossMovement.StartLoop(1);
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

    public void SetAnimationState(string value)
    {
        switch (value)
        {
            default:
            case "Reset":
                _animator.SetInteger("State", 0);
                _animator.SetTrigger("Reset");
            break;

            case "Hide":
                _animator.SetInteger("State", 1);
            break;

            case "Conjure":
                _animator.SetInteger("State", 2);
            break;

            case "ChargeJump":
                _animator.SetInteger("State", 3);
            break;

            case "On":
                _animator.SetInteger("State", 0);
                _animator.SetTrigger("On");
            break;

            case "Off":
                _animator.SetInteger("State", 0);
                _animator.SetTrigger("Off");
            break;
        }
    }
}
