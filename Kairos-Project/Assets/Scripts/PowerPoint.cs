using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PowerPoint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    private List<string> slides = new List<string>();
    private int index = 0;
    private int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        string slide1 = "Feature: Single Selection\n\nLeft click on an entity to select.\n\nAn entity can be a unit or a building";
        string slide2 = "Feature: Multi Selection\n\nLeft click and drag to form a box.\n\nAny entities within the box will be selected, but if both units and buildings are within, only the units will be selected.";
        string slide3 = "Feature: Shift Selection\n\nPressing left shift and selecting a unit will add the unit to the selected entity list without removing entities already in the list.\n\nThis works with single selection as well.";
        string slide4 = "Feature: Hot Keys\n\nWhile any amount of entities are selected, press left control and a number to assign a hot key.\n\nWhen this number is pressed, it will select the group of entities assigned to the hot key."; //<- include buildings?
        string slide5 = "Feature: Hero Unit View\n\nPress 'f' when a hero unit is selected.\n\nWe are still debating whether to include this feature.";
        slides.Add(slide1);
        slides.Add(slide2);
        slides.Add(slide3);
        slides.Add(slide4);
        slides.Add(slide5);
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
                index = 0;
                textMesh.text = slides[index];
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
                index = count - 1;
                textMesh.text = slides[index];
            }
            else
            {
                index--;
                textMesh.text = slides[index];
            }
        }
    }
}
