using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLevelMonitor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        float[] spectrum = new float[128];

        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        for (int i = 1; i < spectrum.Length - 1; i++)
        {
            //TODO - Code here to animate based on frequency volumes

            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
            //Debug.Log(Mathf.Log(spectrum[i]));
        }
    }
}
