using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [Header("Gameplay")]
    public GameObject player;
    public GameObject boss;

    [Header("Extern Info")]
    public TransitionScreen transitionScreen;
    public Image whiteScreen;
    public ResultsScreen resultScreen;
    public ScreenShaker screenShaker;
    public StageTitle stageTitle;
    public AudioSource tempBossVoice;

    static DateTime battleStart;

    static public GameManager instance;
    
	private void Awake ()
    {
        instance = this;
    }

    private void Start()
    {
        int index;
        BossController bossController = boss.GetComponent<BossController>();
        if ((index = bossController.GetStageIndex()) < 1) {
            PlayerHealth.deathCount = 0;
        } else  {
            BGMPlayer.Play();
            stageTitle.SetCard(index);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) ResetScene();
        if (Input.GetKey(KeyCode.E)) player.transform.position += Vector3.left * .1f;
    }

    public void ResetScene()
    {
        PlayerHealth.deathCount++;
        int index = SceneManager.GetActiveScene().buildIndex;
        transitionScreen.CallTransition(index);
    }

    public void CallScene(int index)
    {
        transitionScreen.CallTransition(index);
    }

    public void CallStageTransition(int transitionToStage)
    {
        switch (transitionToStage)
        {
            default:
                stageTitle.SetCard(transitionToStage);
                BossHealth bossHealth = boss.GetComponent<BossHealth>();
                bossHealth.StartCoroutine(bossHealth.Refill());
            break;

            case 1: //stage 1
                StartCoroutine(Stage1Transition());
            break;

            case 3: //stage 3
                StartCoroutine(BattleEnd());
                break;
        }

    }

    IEnumerator Stage1Transition()
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.StartCoroutine(playerMovement.GoToThePositonAndLookCenter(8));
        yield return TimeSlowDown();

        screenShaker.Call(10, 3);
        yield return new WaitWhile(() => screenShaker.active);

        Transform bossTransform = boss.transform;
        while (bossTransform.position.y > -16)
        {
            yield return new WaitForFixedUpdate();
            bossTransform.position += Vector3.down * .3f;
        }

        yield return new WaitForSeconds(.5f);

        Animator bossAnimator = boss.GetComponent<Animator>();
        bossAnimator.enabled = true;
        bossAnimator.SetTrigger("Show");
        tempBossVoice.PlayDelayed(.1f);
        bossTransform.position = new Vector2(bossTransform.position.x, -6);

        yield return new WaitForSeconds(2f);
        bossAnimator.enabled = false;
        BGMPlayer.Play();
        yield return new WaitForSeconds(.5f);

        battleStart = DateTime.Now;
        stageTitle.SetCard(1);
        playerMovement.EndCutscene();
        bossAnimator.enabled = true;
        bossAnimator.SetTrigger("Summon");
        yield return new WaitForSeconds(.3f);

        screenShaker.Call(15, 3);
        BossHealth bossHealth = boss.GetComponent<BossHealth>();
        bossHealth.StartCoroutine(bossHealth.Refill());
        yield return new WaitForSeconds(2);
        bossAnimator.SetTrigger("Toggle");

    }

    IEnumerator BattleEnd()
    {
        boss.GetComponent<BossController>().SetDeathState();
        //disable components
        yield return new WaitForSeconds(.1f);

        Time.timeScale = 0;
        Camera.main.backgroundColor = Color.white;
        BGMPlayer.Stop();
        yield return new WaitForSecondsRealtime(1f);

        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.StartCoroutine(playerMovement.GoToThePositonAndLookCenter(8));
        Time.timeScale = 1;
        screenShaker.Call(10, 5);
        yield return new WaitForSeconds(3);

        yield return FadeToWhite();
        yield return new WaitForSeconds(1);
        resultScreen.gameObject.SetActive(true);

        TimeSpan battleTime;
        string clearTime;
        if(battleStart != null) {
            battleTime = (DateTime.Now - battleStart);
            clearTime =
                battleTime.Minutes.ToString("00") + ":" +
                battleTime.Seconds.ToString("00") + ":" +
                battleTime.Milliseconds.ToString("000");
        } else {
            clearTime = "X:XX:XXX";
        }

        resultScreen.SetValues(clearTime, PlayerHealth.deathCount, player.GetComponent<PlayerHealth>().value);

        yield return new WaitUntil(() => resultScreen.allowRestart);
        yield return new WaitUntil(() => Input.GetButtonDown("Jump"));

        CallScene(0);
    }

    IEnumerator TimeSlowDown(){
        Time.timeScale = .1f;

        while (Time.timeScale < 1)
        {
            yield return new WaitForEndOfFrame();
            Time.timeScale += .01f;
        }

        Time.timeScale = 1;
    }

    IEnumerator FadeToWhite()
    {
        Color color = color = whiteScreen.color;

        while (whiteScreen.color.a < 1)
        {
            yield return new WaitForEndOfFrame();
            color.a += .01f;
            whiteScreen.color = color;
        }

        whiteScreen.color = color;
    }

}
