using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class volumenSFX : MonoBehaviour
{
    public Slider slider;
    public float sliderValue;

    // Start is called before the first frame update
    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("volumenSFX", 0.5f);
        AudioManager.Instance.volumen = slider.value;
    }

    public void ChangeSlider(float valor)
    {
        sliderValue = valor;
        PlayerPrefs.SetFloat("volumenSFX", sliderValue);
        AudioManager.Instance.volumen = slider.value;
    }
}