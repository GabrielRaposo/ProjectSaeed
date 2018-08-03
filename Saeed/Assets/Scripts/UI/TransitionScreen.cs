using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TransitionScreen : MonoBehaviour {

    [Range(10, 30)] public float fadeOutSpeed;
    [Range(10, 30)] public float fadeInSpeed;

    private void Start()
    {
        StartCoroutine(fadeInAnimation());
    }

    public void CallTransition(int buildIndex)
    {
        StartCoroutine(fadeOutAnimation(buildIndex));
    }

    IEnumerator fadeInAnimation()
    {
        Time.timeScale = 1;

        Image _image = GetComponent<Image>();
        if (_image)
        {
            Color color = Color.black;
            color.a = 1;
            _image.color = color;

            while (color.a > 0)
            {
                yield return new WaitForEndOfFrame();
                color.a -= fadeInSpeed / 600;
                _image.color = color;
            }
            color.a = 0;
        }
    }

    IEnumerator fadeOutAnimation(int index)
    {
        Time.timeScale = .1f;

        AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync(index);
        sceneLoadOperation.allowSceneActivation = false;

        Image _image = GetComponent<Image>();
        if (_image)
        {
            Color color = Color.black;
            color.a = 0;
            _image.color = color;

            while(color.a < 1)
            {
                yield return new WaitForEndOfFrame();
                color.a += fadeOutSpeed / 600;
                _image.color = color;
            }
            color.a = 1;
        }

        sceneLoadOperation.allowSceneActivation = true;
    }
}
