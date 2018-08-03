using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventTrigger : MonoBehaviour {

    //temp
    public TitleManager titleManager;
    public int sceneIndex;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (titleManager) titleManager.StartBattle();
            BossController.currentStage = BossController.Stage.Tutorial;
        }
    }
}
