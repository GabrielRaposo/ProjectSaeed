using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossMovement : MonoBehaviour {
    const float 
        HIDE_Y = -10f; //a altura ótima parada posicionar o Boss de forma que ele fique escondido completamente no chão

    [Header("References")]
    public SpriteRenderer bodyRenderer;
    public ParticleSystem groundShake;
    public string layerToIgnore;

    BossStateManager stateManager;

    private void Start()
    {
        stateManager = GetComponent<BossStateManager>();

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer(layerToIgnore), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(layerToIgnore), LayerMask.NameToLayer(layerToIgnore), true);
    }

    public void StartLoop(int stage)
    {
        switch (stage)
        {
            default:
            case 1:
                StartCoroutine(Stage1Loop());
            break;
        }
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
            yield return WaitToAct(1f, 2f);

            //select action
            int randomAction = Random.Range(0, 5);
            switch (randomAction)
            {
                default:
                case 0:
                case 1:
                    yield return ACTION_HideAndShow();
                break;

                case 2:
                case 3:
                    yield return ACTION_SpawnBombs();
                break;

                case 4:
                    yield return ACTION_Jump();
                break;
            }
                
        }
    }


    IEnumerator ACTION_HideAndShow()
    {
        //pre-hide
        stateManager.SetAnimationState("Hide");
        yield return new WaitForSeconds(1);

        //pre-show
        float xSpawnPosition = Random.Range(-5f, 5f);
        groundShake.transform.position = new Vector2(xSpawnPosition, groundShake.transform.position.y);
        groundShake.Play();
        yield return new WaitForSeconds(1.2f);
        groundShake.Stop();

        //show
        transform.position = new Vector2(xSpawnPosition, transform.position.y);
        stateManager.SetAnimationState("Off");
        yield return new WaitForSeconds(1);
    }

    IEnumerator ACTION_SpawnBombs()
    {
        stateManager.SetAnimationState("Conjure");

        SetBomb(Vector2.up * Random.Range(0f, 4f) + Vector2.right * Random.Range(-9, -5));
        yield return new WaitForSeconds(.3f);
        //SetBomb(Vector2.up * Random.Range(0f, 4f) + Vector2.right * Random.Range(-4, 4));
        //yield return new WaitForSeconds(.3f);
        SetBomb(Vector2.up * Random.Range(0f, 4f) + Vector2.right * Random.Range( 4, 9));
        yield return new WaitForSeconds(2);

        stateManager.SetAnimationState("Off");
        yield return new WaitForSeconds(1);
    }

    IEnumerator ACTION_Jump()
    {
        //charge
        stateManager.SetAnimationState("ChargeJump");
        yield return new WaitForSeconds(2);

        //jump
        stateManager.SetAnimationState("On");
        yield return new WaitForSeconds(1.5f);
    }

    private void SetBomb(Vector2 position)
    {
        GameObject _bomb = Spawner.instance.GetFromPool(0, position);
        _bomb.GetComponent<Bomb>().Spawn();
    }
}
