using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoSlides : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] [TextArea(15,20)]
    private List<string> slides = new List<string>();

    private int index = 0;
    private int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        count = slides.Count;
        textMesh.text = slides[index];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            if (index == count - 1)
            {
                //index = 0;
                //textMesh.text = slides[index];
                GetComponent<Image>().enabled = false;
                GetComponentInChildren<TextMeshProUGUI>().enabled = false;
            }
            else
            {
                index++;
                textMesh.text = slides[index];
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            if (index == 0)
            {
                //index = count - 1;
                //textMesh.text = slides[index];
            }
            else
            {
                GetComponent<Image>().enabled = true;
                GetComponentInChildren<TextMeshProUGUI>().enabled = true;
                index--;
                textMesh.text = slides[index];
            }
        }
    }
}
