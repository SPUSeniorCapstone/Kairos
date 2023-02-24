using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Data.Common;
using static UnityEngine.UI.GridLayoutGroup;
using System.Linq;

public class CreateWorld : MonoBehaviour
{
    public TMP_InputField input;
    public Slider scaleSlider;
    public Slider sizeSlider;
    public RawImage rawImage;
    public World world;


    public Image strongholdIconPlaceholder;
    public Image strongholdIcon;

    public bool placeStronghold;

    public Image corruptionIconPlaceholder;
    public List<Image> corruptionIcons;

    public bool placeCorruptionNode;


    public WorldGenerator generator;

    private void Start()
    {
        input.text = generator.seed.ToString();
        scaleSlider.value = generator.scale;
        sizeSlider.value = generator.worldSize.x;

        generator.GenerateWorld(false);
        rawImage.texture = world.GeneratedWorldTexture();
    }

    private void Update()
    {
        if (placeStronghold)
        {
            Vector3[] corners = new Vector3[4];
            strongholdIconPlaceholder.rectTransform.GetLocalCorners(corners);
            var offset = (corners[2].y - corners[0].y) / 4f;
            strongholdIconPlaceholder.rectTransform.position = Input.mousePosition + new Vector3(0, offset, 0);
            strongholdIconPlaceholder.gameObject.SetActive(true);
        }
        else
        {
            strongholdIconPlaceholder.gameObject.SetActive(false);
        }

        if (placeCorruptionNode)
        {
            Vector3[] corners = new Vector3[4];
            corruptionIconPlaceholder.rectTransform.GetLocalCorners(corners);
            var offset = (corners[2].y - corners[0].y) / 3f;
            corruptionIconPlaceholder.rectTransform.position = Input.mousePosition + new Vector3(0, offset, 0);
            corruptionIconPlaceholder.gameObject.SetActive(true);
        }
        else
        {
            corruptionIconPlaceholder.gameObject.SetActive(false);
        }
    }

    public void BeginPlaceStronghold()
    {
        placeStronghold = true;
        placeCorruptionNode = false;
    }

    public void BeginPlaceCorruptionNode()
    {
        placeCorruptionNode = true;
        placeStronghold = false;
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
        if (!placeStronghold && !placeCorruptionNode)
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
                strongholdIcon.rectTransform.position = strongholdIconPlaceholder.rectTransform.position;
                strongholdIcon.gameObject.SetActive(true);
                generator.strongholdPos = position;
                placeStronghold = false;
            }
        }
        else if (placeCorruptionNode)
        {
            Vector3Int position = (normPos * WorldController.Main.World.LengthInBlocks).ToVector3Int();
            position.z = position.y;
            position.y = 0;


            if (CheckValid(position, new Vector2Int(2, 2)))
            {
                var ico = Instantiate<Image>(corruptionIconPlaceholder, rawImage.transform);
                corruptionIcons.Append(ico);
                ico.gameObject.SetActive(true);
                generator.corruptionNodePositions.Append(position);
                placeCorruptionNode = false;
            }
        }
    }

    public void OnChange()
    {
        generator.GenerateWorld(false);
        rawImage.texture = world.GeneratedWorldTexture();
    }
}
