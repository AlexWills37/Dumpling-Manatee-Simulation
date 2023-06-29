using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPS : MonoBehaviour
{
    private TextMeshProUGUI text;
    private float[] deltaTimes;
    private int pointer = 0;

    // Start is called before the first frame update
    void Start()
    {
        deltaTimes = new float[10];
        text = this.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        deltaTimes[pointer] = Time.deltaTime;
        pointer = (pointer += 1) % 10;
        if (pointer == 0) {
            float fps = 0;
            foreach (float f in deltaTimes) {
                fps += f;
            }
            fps /= 10f;
            fps = 1f / fps;

            text.text = "FPS: " + fps;
        }
    }
}
