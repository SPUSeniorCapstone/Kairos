using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Data.Common;

public class CreateWorld : MonoBehaviour
{
    public TMP_InputField input;
    public Slider scaleSlider;
    public Slider sizeSlider;
    public RawImage rawImage;
    public World world;


    public bool placeStronghold;
    public Vector3Int strongholdPos;

    WorldGenerator generator;

    private void Start()
    {
        generator = GetComponent<WorldGenerator>();

        input.text = generator.seed.ToString();
        scaleSlider.value = generator.scale;
        sizeSlider.value = generator.worldSize.x;

        generator.GenerateWorld(false);
        rawImage.texture = world.GeneratedWorldTexture();
    }

    bool CheckValid(Vector3Int position, Vector2Int size)
    {
        var w = WorldController.Main;
        int h = w.World.GetHeight(position);

        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                int h2 = w.World.GetHeight(position.x + x, position.z + z);
                if (h2 != h)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void OnClick(BaseEventData data)
    {
        if (!placeStronghold)
        {
            return;
        }

        var clickData = data as PointerEventData;
        Vector3[] corners = new Vector3[4];
        rawImage.rectTransform.GetWorldCorners(corners);
        Vector3 bottomLeft = corners[0];
        Vector3 topRight = corners[2];
        
        var posInImage = Input.mousePosition - bottomLeft;
        topRight -= bottomLeft;

        var normPos = new Vector3(posInImage.x / topRight.x, posInImage.y / topRight.y, 0);


        if (placeStronghold)
        {
            Vector3Int position = (normPos * WorldController.Main.World.LengthInBlocks).ToVector3Int();
            position.z = position.y;
            position.y = 0;

            
            if(CheckValid(position, new Vector2Int(2, 2)))
            {
                strongholdPos = position;
                placeStronghold = false;
            }
        }
    }

    public void OnChange()
    {
        generator.GenerateWorld(false);
        rawImage.texture = world.GeneratedWorldTexture();
    }
}
