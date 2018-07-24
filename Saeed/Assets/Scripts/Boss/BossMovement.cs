using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossMovement : MonoBehaviour {
    const float 
        HIDE_Y = -9f; //a altura ótima parada posicionar o Boss de forma que ele fique escondido completamente no chão

    [Header("UI References")]
    public TextMeshProUGUI stageTitle;

    [Header("External References")]
    public ParticleSystem groundShake;
    public string layerToIgnore;

    public enum Stage { Tutorial, Face, Arms, Legs, Outro }
    public Stage currentStage;

    Animator _animator;
    SpriteRenderer _renderer;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer(layerToIgnore), true);
        SetStage();
    }

    public void EndStage()
    {
        currentStage++;
        SetStage();
    }

    void SetStage() {
        switch (currentStage)
        {
            default:
            case Stage.Tutorial:
                StartCoroutine(SetTitle("Stage 0: Tutorial"));
                transform.position = new Vector2(transform.position.x, -7);
                _animator.enabled = false;
                break;

            case Stage.Face:
                StartCoroutine(SetTitle("Stage 1: Face Reveal"));
                transform.position = new Vector2(transform.position.x, -5);
                _animator.enabled = true;
                StartCoroutine(Stage1Loop());
                break;

            case Stage.Arms:
                StartCoroutine(SetTitle("Stage 2: Armed and Ready"));
                transform.position = new Vector2(transform.position.x, -4);
                break;

            case Stage.Legs:
                StartCoroutine(SetTitle("Stage 3: Leg day!"));
                break;

            case Stage.Outro:
                StartCoroutine(SetTitle("Stage X: |     || \n                ||    |_"));
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

    IEnumerator WaitToAct(float min, float max)
    {
        float time = Random.Range(min, max);
        yield return new WaitForSeconds(time);
    }

    IEnumerator Stage1Loop()
    {
        while (true)
        {
            yield return WaitToAct(1f, 3f);

            //select action
            yield return ACTION_SpawnBombs();
            yield return ACTION_HideAndShow();
        }
    }


    IEnumerator ACTION_HideAndShow()
    {
        //pre-hide
        _renderer.color = Color.blue;
        yield return new WaitForSeconds(2);
        _renderer.color = Color.white;

        //hide
        float originalY = transform.position.y;
        transform.position += Vector3.up * .2f;
        for(int i = 0; i < 6; i++) yield return new WaitForEndOfFrame();
        while (transform.position.y > HIDE_Y)
        {
            yield return new WaitForEndOfFrame();
            transform.position += Vector3.down * .4f;
        }
        yield return new WaitForSeconds(.5f);

        //pre-show
        float xSpawnPosition = Random.Range(-5f, 5f);
        groundShake.transform.position = new Vector2(xSpawnPosition, groundShake.transform.position.y);
        groundShake.Play();
        yield return new WaitForSeconds(1.2f);
        groundShake.Stop();

        //show
        transform.position = new Vector2(xSpawnPosition, HIDE_Y);
        while (transform.position.y < originalY)
        {
            yield return new WaitForEndOfFrame();
            transform.position += Vector3.up * .4f;
        }
        transform.position = new Vector2(xSpawnPosition, originalY);
    }

    IEnumerator ACTION_SpawnBombs()
    {
        //charge
        _renderer.color = Color.red;
        yield return new WaitForSeconds(1);

        //spawn
        SetBomb(Vector2.up * Random.Range(3f, 3.5f) + Vector2.left  * Random.Range(1, 6));
        SetBomb(Vector2.up * Random.Range(3f, 3.5f) + Vector2.right * Random.Range(1, 6));

        yield return new WaitForSeconds(1);
        _renderer.color = Color.white;
    }

    private void SetBomb(Vector2 position)
    {
        GameObject _bomb = Spawner.instance.GetFromPool(0, position);
        _bomb.GetComponent<Bomb>().SetToLaunch();
    }
}
