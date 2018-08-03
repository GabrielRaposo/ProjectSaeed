using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public AudioSource battleMusic;

    [Header("Extern Info")]
    public PlayerMovement player;
    public TransitionScreen transitionScreen;
    public Image whiteScreen;
    public ResultsScreen resultScreen;

    public ScreenShaker screenShaker;
    public StageTitle stageTitle;
    public AudioSource tempBossVoice;

    DateTime battleStart, battleEnd;

    static public GameManager instance;
    
	private void Awake ()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) ResetScene();
        if (Input.GetKey(KeyCode.E)) player.transform.position += Vector3.left * .1f;
    }

    public void ResetScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        transitionScreen.CallTransition(index);
    }

    public void CallScene(int index)
    {
        transitionScreen.CallTransition(index);
    }

    public void CallStageTransition(int transitionToStage, BossHealth bossHealth)
    {
        switch (transitionToStage)
        {
            default:
                stageTitle.SetCard(transitionToStage);
                bossHealth.StartCoroutine(bossHealth.Refill());
            break;

            case 1: //stage 1
                StartCoroutine(Stage1Transition(bossHealth));
            break;

            case 3: //stage 3
                StartCoroutine(BattleEnd(bossHealth));
                break;
        }

    }

    IEnumerator Stage1Transition(BossHealth bossHealth)
    {
        player.StartCoroutine(player.GoToThePositonAndLookCenter(8));
        yield return TimeSlowDown();
        yield return new WaitForSeconds(.5f);

        screenShaker.Call(10, 3);
        yield return new WaitWhile(() => screenShaker.active);

        Transform bossTransform = bossHealth.transform;
        while (bossTransform.position.y > -16)
        {
            yield return new WaitForFixedUpdate();
            bossTransform.position += Vector3.down * .3f;
        }

        yield return new WaitForSeconds(2f);

        Animator bossAnimator = bossHealth.GetComponent<Animator>();
        bossAnimator.enabled = true;
        bossAnimator.SetTrigger("Show");
        tempBossVoice.PlayDelayed(.1f);
        bossTransform.position = new Vector2(bossTransform.position.x, -6);

        yield return new WaitForSeconds(2f);
        bossAnimator.enabled = false;
        battleMusic.Play();
        yield return new WaitForSeconds(.5f);

        battleStart = DateTime.Now;
        stageTitle.SetCard(1);
        player.EndCutscene();
        bossAnimator.enabled = true;
        bossAnimator.SetTrigger("Summon");
        yield return new WaitForSeconds(.3f);

        screenShaker.Call(15, 3);
        bossHealth.StartCoroutine(bossHealth.Refill());
        yield return new WaitForSeconds(2);
        bossAnimator.SetTrigger("Toggle");

    }

    IEnumerator BattleEnd(BossHealth bossHealth)
    {
        bossHealth.GetComponent<BossController>().SetDeathState();
        //disable components
        yield return new WaitForSeconds(.1f);

        Time.timeScale = 0;
        Camera.main.backgroundColor = Color.white;
        battleMusic.Stop();
        yield return new WaitForSecondsRealtime(1f);

        player.StartCoroutine(player.GoToThePositonAndLookCenter(8));
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

        resultScreen.SetValues(clearTime, 0, player.GetComponent<PlayerHealth>().value);

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
