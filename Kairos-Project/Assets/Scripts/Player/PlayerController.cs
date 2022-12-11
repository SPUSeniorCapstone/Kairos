using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerController : MonoBehaviour
{
    public bool onEnemy;
    // brute force
    public bool heroFollow = false;
    public Dictionary<KeyCode, List<Entity>> hotKeys = new Dictionary<KeyCode, List<Entity>>();

    [SerializeField] private Vector3 startPosition;
    private Vector3 startPosition3D;
    
    private Vector3 endPosition;
    TerrainCollider terrainCollider;

    [SerializeField] public PositionMarker positionMarker;

    [SerializeField] private GameObject selectionAreaTransform;



    // okay to be public?
    public List<Entity> selectedEntityList;
    // Replace Entity with selectable entity
    [SerializeField] public List<GameObject> playerEntities;

    private KeyCode[] keyCodes = {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
     };

    private void Awake()
    {
        selectedEntityList = new List<Entity>();
        selectionAreaTransform.gameObject.SetActive(false);
    }


    private void Start()
    {
        //GameObject[] temp = GameObject.FindGameObjectsWithTag("Entity");
        //playerEntities = temp.ToList();
        terrainCollider = Terrain.activeTerrain.GetComponent<TerrainCollider>();

    }

    private void Update()
    {
        if (!GameController.main.paused)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (onEnemy)
                {
                    Debug.Log("ATTACK!");
                }
                MoveSelected();
            }
            
           Selection();
        }

        // logic for hot keying units (needs add buildings and prioritizing units over buildings in selection)
        // as of right now, allows for a unit to be hot keyed to multiple keys, allowing for "sub hot keys" 
        // i.e. '2' is your swordsman hot key, but perhaps '3' is for your two handed swords and '4' is for your shield and swords men, whil '2' selects both of them
        if (Input.GetKey(KeyCode.LeftControl) )/*&&
            Input.GetKeyDown(KeyCode.Alpha2))*/
        {
            /*
            foreach (Entity entity in selectedEntityList)           //<- is it faster to do a massive OR statment than iterate through the key array?
                    {
                        entity.hoykey = KeyCode.Alpha2;
                    }*/
            foreach (KeyCode keyCode in keyCodes)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    hotKeys.Remove(keyCode);
                    List<Entity> list = new List<Entity>(selectedEntityList);
                    foreach (Entity entity in selectedEntityList)
                    {
                        entity.hotkey = keyCode;                  
                    }
                    hotKeys.Add(keyCode, list);
                }
            }
        }
        foreach (KeyCode keyCode in keyCodes)
        {
            if (Input.GetKeyDown(keyCode))
            {
                if (hotKeys[keyCode] == null) //<- try catch instead?
                {
                    Debug.Log("HELPME");

                }
                // don't add hot key to total list (does this cancel shift?) <-yes
                foreach(Entity entity in selectedEntityList)
                {
                    entity.SetSelectedVisible(false);
                    var battalion = entity as Battalion;
                    if (battalion != null)
                    {
                        battalion.Deselect();
                    }
                }
                selectedEntityList.Clear();
                foreach(Entity entity in hotKeys[keyCode])
                {
                   if (entity != null)
                    {
                        entity.SetSelectedVisible(true);
                        selectedEntityList.Add(entity);
                        var battalion = entity as Battalion;
                        if (battalion != null)
                        {
                            battalion.Select();
                        }
                    }
                }
            }
        }
    }

    public void MoveSelected()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000, LayerMask.GetMask("Terrain")))
        {
            //Debug.Log(MapController.main.grid.WorldToCell(hit.point));
            if (!heroFollow)
            {
                positionMarker.point = hit.point;
                positionMarker.point.y = .1f;
                positionMarker.PlaceMarker();
            }
            
            foreach (var entity in selectedEntityList)
            {
                var unit = entity as Unit;
                var battalion = entity as Battalion;            
                if (battalion != null)
                {
                    var pos = hit.point;
                    pos.y = 0;
                    if (heroFollow){
                        pos = GameObject.Find("Hero").transform.position;
                    }
                    if (selectedEntityList.Count != 1)
                    {
                        pos += Random.insideUnitSphere * 6;
                    }               
                    battalion.Move(pos);
                }
                if (unit != null)
                {
                    var pos = hit.point;
                    pos.y = 0;
                    if (heroFollow)
                    {
                        pos = GameObject.Find("Hero").transform.position;
                    }
                    if (selectedEntityList.Count != 1)
                    {
                        pos += Random.insideUnitSphere * 6;
                    }
                    unit.MoveAsync(pos);
                }
            }
        }

    }

    void Selection()
    {

        if (Input.GetMouseButtonDown(0))
        {
            //left mouse
            //startPosition3D = GetMouseWorldPosition3D();
            startPosition = Input.mousePosition;
            selectionAreaTransform.gameObject.SetActive(true);
            //Debug.Log(startPosition);
        }

        if (Input.GetMouseButton(0))
        {
            // while mouse held down
            Vector3 currentMousePosition = Input.mousePosition;
            float z = currentMousePosition.z;
            Vector3 lowerLeft = new Vector3(
                    Mathf.Min(startPosition.x, currentMousePosition.x),
                    Mathf.Min(startPosition.y, currentMousePosition.y),
                    z
            );
            Vector3 upperRight = new Vector3(
                 Mathf.Max(startPosition.x, currentMousePosition.x),
                  Mathf.Max(startPosition.y, currentMousePosition.y),
                  z
                );
            // shouldn't matter
            Vector3 lowerRight = new Vector3(
                upperRight.x,
                lowerLeft.y,
                z
               );
            Vector3 upperLeft = new Vector3(
                lowerLeft.x,
                upperRight.y,
                z
               );

            //================================================================================================================

            var line = selectionAreaTransform.GetComponent<LineRenderer>();
            line.positionCount = 5;
            List<Vector3> vector3s = new List<Vector3>();
            vector3s.Add(MouseToWorld(upperLeft));
            vector3s.Add(MouseToWorld(upperRight));
            vector3s.Add(MouseToWorld(lowerRight));
            vector3s.Add(MouseToWorld(lowerLeft));
            vector3s.Add(MouseToWorld(upperLeft));
            line.SetPositions(vector3s.ToArray());

        }

        if (Input.GetMouseButtonUp(0))
        {
            bool singleClick = false;
            selectionAreaTransform.gameObject.SetActive(false);

            // this takes care of deselection, with left shift allowance
            //=============================================
            if (!Input.GetKey(KeyCode.LeftShift)) {
                foreach (var entity in selectedEntityList)
                {
                    entity.SetSelectedVisible(false);
                    var battalion = entity as Battalion;
                    if (battalion != null)                      
                    {
                        battalion.Deselect();                   
                    }                 
                }
                selectedEntityList.Clear();
            }
            //===========================================================
            Vector3 currentMousePosition = Input.mousePosition;

            if (currentMousePosition == startPosition)
            {
                singleClick = true;
            }



            Vector3 lowerLeft = new Vector3(
                 Mathf.Min(startPosition.x, currentMousePosition.x),
                 Mathf.Min(startPosition.y, currentMousePosition.y)
          );
            Vector3 upperRight = new Vector3(
                 Mathf.Max(startPosition.x, currentMousePosition.x),
                 Mathf.Max(startPosition.y, currentMousePosition.y)


                );
            Vector3 lowerRight = new Vector3(
               upperRight.x,
                 lowerLeft.y

              );
            Vector3 upperLeft = new Vector3(
                lowerLeft.x,
                upperRight.y

               );

            if (!singleClick)
            {
                List<Battalion> battalions = new List<Battalion>();
                foreach (var item in playerEntities)
                {
                    var test = item.GetComponent<Unit>();
                    var battalion = item.GetComponentInParent<Battalion>();
                    if (test != null)
                    {
                        var tree = Camera.main.WorldToScreenPoint(item.transform.position);
                        if (tree.x > lowerLeft.x && tree.x < upperRight.x && tree.y > lowerLeft.y && tree.y < upperRight.y)
                        {
                            // remove this to allow selecting individuals from a battalian
                            if (battalion != null)
                            {
                                if (!battalion.selected)
                                {
                                    battalion.selected = true;
                                    battalions.Add(battalion);
                                }
                            }
                            else
                            {
                                test.SetSelectedVisible(true);
                                selectedEntityList.Add(test);
                            }
                        }        
                    }  
                }
                foreach (Battalion battalion in battalions)
                {
                    selectedEntityList.Add(battalion);
                    battalion.Select();
                }
            }
            else
            {
                Entity Entity = GetMouseWorldPosition3D();
                
                if (Entity != null)
                {
                    Unit unit = Entity.GetComponent<Unit>();
                    if (unit != null){
                        var battalion = unit.GetComponentInParent<Battalion>();
                        if (battalion != null)
                        {
                            Debug.Log("Battalion");
                            battalion.Select();
                            selectedEntityList.Add(battalion);
                        }
                        else
                        {
                            Entity.SetSelectedVisible(true);
                            selectedEntityList.Add(Entity);
                        }
                    }                       
                }
            }
        }
    }

    // does this need to run in update?
    private Entity GetMouseWorldPosition3D()
    {
        Ray ray;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData, 1000) && (hitData.transform.GetComponentInParent<Entity>() || hitData.transform.GetComponent<Entity>())) // <- will this be problematic?
        {
            Debug.Log(hitData.transform.name);
            return hitData.transform.GetComponentInParent<Entity>();
        }
        else if (hitData.transform != null) { Debug.Log(hitData.transform.name); } return null; 
        
        

    }
    private Vector3 MouseToWorld(Vector3 vector)
    {
        vector.z = 2.5f;
        return Camera.main.ScreenToWorldPoint(vector);
    }

    public Mesh CreateMesh(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        Mesh mesh = new Mesh();
        Vector3[] vector3s = new Vector3[4];
        vector3s[0] = a;
        vector3s[1] = b;
        vector3s[2] = c;
        vector3s[3] = d;
        Vector2[] uvs = new Vector2[vector3s.Length];
        int[] triangles =
        {
            0,1,3,
            1,3,2
        };
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vector3s[i].x, vector3s[i].z);
        }
        mesh.Clear();
        mesh.uv = uvs;
        mesh.vertices = vector3s;
        mesh.triangles = triangles;
        mesh.name = "Selection Mesh";
        return mesh;
    }
}
