using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public GameObject t;
    // Start is called before the first frame update
    void Start()
    {
       t = GameObject.Find("Terrain");
        Debug.Log("AWAKEN");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeValue(string This)
    {
        GameObject temp = GameObject.Find(This);
        
        temp.GetComponentInChildren<TextMeshProUGUI>().text = temp.GetComponent<Slider>().value.ToString();
        if (temp.name == "Eccen")
        {
            t.GetComponent<Generator>().eccentricity = temp.GetComponent<Slider>().value;

        } else if(temp.name == "Scale")
        {
            t.GetComponent<Generator>().scale = temp.GetComponent<Slider>().value;
        }
       
        
    }

}
