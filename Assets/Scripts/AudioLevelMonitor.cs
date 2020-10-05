using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLevelMonitor : MonoBehaviour
{
    private readonly float[] _spectrumData = new float[128];

    // Update is called once per frame
    void Update()
    {
        AudioListener.GetSpectrumData(_spectrumData, 0, FFTWindow.Rectangular);

        for (int i = 1; i < _spectrumData.Length - 1; i++)
        {
            //TODO - Code here to animate based on frequency volumes

            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(_spectrumData[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(_spectrumData[i]), 3), Color.blue);
            //Debug.Log(Mathf.Log(spectrum[i]));
        }
    }
}
