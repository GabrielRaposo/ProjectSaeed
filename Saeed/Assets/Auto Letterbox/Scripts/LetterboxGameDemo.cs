using UnityEngine;
using System.Collections;
using LetterboxCamera;

namespace LetterboxCamera {

    public class LetterboxGameDemo : MonoBehaviour {

        public ForceCameraRatio cameraManager;
        public float letterboxInRate = 2f;
        public float letterboxOutRate = 10f;
        float letterboxRate;
        Vector2 targetRatio;
        bool inRatio = false;

        public void Start() {
            targetRatio = new Vector2(5, 4);
        }

        public void Update() {
            cameraManager.ratio = Vector2.Lerp(cameraManager.ratio, targetRatio, letterboxRate * Time.deltaTime);
        }
    }
}