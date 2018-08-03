using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StageTitle : MonoBehaviour {

    public TextMeshProUGUI stageCard;

	void Start () {
        stageCard.enabled = false;
    }
	
	public void SetCard(int stageIndex)
    {
        switch (stageIndex)
        {
            case 0:
                StartCoroutine(SetTitle("Stage 0: Tutorial"));
                break;

            case 1:
                StartCoroutine(SetTitle("Stage 1: Face Reveal"));
                break;

            case 2:
                StartCoroutine(SetTitle("Stage 2: Armed and Ready"));
                break;

            case 3:
                StartCoroutine(SetTitle("Stage 3: Leg day!"));
                break;

            default:
                StartCoroutine(SetTitle("You win."));
                break;
        }
    }

    IEnumerator SetTitle(string text)
    {
        stageCard.text = text;
        stageCard.enabled = true;
        yield return new WaitForSeconds(3);
        stageCard.enabled = false;
    }
}
