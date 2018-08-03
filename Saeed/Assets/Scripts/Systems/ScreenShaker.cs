using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShaker : MonoBehaviour {

    public ParticleSystem rocksPS;
    public AudioSource rocksSound;

    public bool active { get; private set; }
    Transform mainCamera;
    Vector3 originalPosition;
    Coroutine mainRoutine;

	void Start () {
        mainCamera = Camera.main.transform;
        originalPosition = mainCamera.position;
    }
	
    public void Call(float intensity, float duration)
    {
        if (mainRoutine != null)
        {
            StopCoroutine(mainRoutine);
        }
        mainRoutine = StartCoroutine(ScreenShakeEvent(intensity, duration));
    }

    IEnumerator ScreenShakeEvent(float intensity, float duration)
    {
        intensity /= 100;
        int delay = 2;
        int shakeCount = (int)(duration * 60 / delay);

        mainCamera.position = originalPosition + (Vector3.left * intensity / 2);
        active = true;
        if (rocksPS) rocksPS.Play();
        rocksSound.Play();

        for (int i = 0; i < shakeCount; i++)
        {
            for (int j = 0; j < delay; j++)
            {
                yield return new WaitForEndOfFrame();
            }

            mainCamera.position += Vector3.right * (i % 2 == 0 ? intensity : - intensity);
        }

        mainCamera.position = originalPosition;
        active = false;
        if (rocksPS) rocksPS.Stop();
        yield return AudioFadeout();
    }

    IEnumerator AudioFadeout()
    {
        float originalVolume = rocksSound.volume;
        float decreaseValue = originalVolume / 60;
        while(rocksSound.volume > 0)
        {
            yield return new WaitForEndOfFrame();
            rocksSound.volume -= decreaseValue;
        }
        rocksSound.Stop();
        rocksSound.volume = originalVolume;
    }
}
