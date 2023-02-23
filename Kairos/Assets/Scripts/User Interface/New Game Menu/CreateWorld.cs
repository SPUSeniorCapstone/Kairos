using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateWorld : MonoBehaviour
{
    public TMP_InputField input;
    public Slider slider;
    public float RawImage;

    WorldGenerator generator;


    private void Start()
    {
        generator = GetComponent<WorldGenerator>();

        input.text = generator.seed.ToString();
        slider.value = generator.scale;
    }
}
