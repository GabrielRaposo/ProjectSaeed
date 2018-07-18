using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [Header("External Info")]
    public TransitionScreen transitionScreen;

    static public GameManager instance;

	private void Awake ()
    {
        instance = this;
    }

    public void ResetScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        transitionScreen.CallTransition(index);
    }

}
