using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour {

    public TransitionScreen transitionScreen;

    public void StartBattle()
    {
        transitionScreen.CallTransition(1);
    }
}
