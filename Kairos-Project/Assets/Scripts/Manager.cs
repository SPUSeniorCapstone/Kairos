using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public List<string> SceneNames;
    [SerializeField] int index = 0;
    public int count = 0;
    private string nextScene;
    private string previousScene;
    private string nextButton = "Load Next Scene";
    private string previousButton = "load Previous Scene";
    private bool left = false;

    private static bool created = false;

    private Rect buttonNextRect;
    private Rect buttonPreviousRect;
    private int width, height;
    //private SceneManager sceneManager;

    private void Awake()
    {
        // quick and dirty
        count = SceneManager.sceneCountInBuildSettings;
        index = SceneManager.GetActiveScene().buildIndex;
       
        //SceneNames.Add("Scene 0");
        //SceneNames.Add("Scene 1");
        //SceneNames.Add("Scene 2");
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void NextScene()
    {
        if (index == count - 1)
        {
            index = 0;
            SceneManager.LoadScene(index);     
        }
        else
        {
            index++;
            SceneManager.LoadScene(index);
        }
    }

    public void PreviousScene()
    {
        if (index == 0)
        {
            index = count - 1;
            SceneManager.LoadScene(index);
        }
        else
        {
            index--;
            SceneManager.LoadScene(index);
        }
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    //private void OnGUI()
    //{
    //    // Return the current Active Scene in order to get the current Scene name.
    //    //Scene scene = SceneManager.GetActiveScene();
    //   // scene.buildIndex;
    //    int max = SceneNames.Count - 1;
    //    Debug.Log("Count equals " + max);
    //    Debug.Log("index = " + index);
    //    if (index == max)
    //    {
    //        nextScene = SceneNames[0];
    //        previousScene = SceneNames[index - 1];
    //    }
    //    else if (index == 0)
    //    {
    //        nextScene = SceneNames[index++];
    //        previousScene = SceneNames[max];
    //    }
    //    else
    //    {
    //        nextScene = SceneNames[index++];
    //        previousScene = SceneNames[index--];
    //    }
    //    // Check if the name of the current Active Scene is your first Scene.
    //    //if (scene.name == myFirstScene)
    //    //{
    //    //    nextButton = "Load Next Scene";
    //    //    nextScene = mySecondScene;
    //    //}
    //    //else
    //    //{
    //    //    nextButton = "Load Previous Scene";
    //    //    nextScene = myFirstScene;
    //    //}

    //    // Display the button used to swap scenes.
    //    GUIStyle buttonStylePrevious = new GUIStyle(GUI.skin.GetStyle("button"));
    //    buttonStylePrevious.alignment = TextAnchor.MiddleCenter;
    //    buttonStylePrevious.fontSize = 12 * (width / 200);

    //    GUIStyle buttonStyleNext = new GUIStyle(GUI.skin.GetStyle("button"));
    //    buttonStyleNext.alignment = TextAnchor.MiddleCenter;
    //    buttonStyleNext.fontSize = 12 * (width / 200);


    //    if (GUI.Button(buttonNextRect, nextButton, buttonStyleNext))
    //    {
    //        Debug.Log("Load " + nextScene);
    //        SceneManager.LoadScene(nextScene);
    //        if (index == max)
    //        {
    //            index = 0;
    //        }
    //        else
    //        {
    //            index++;
    //        }
    //    }
    //    else if (GUI.Button(buttonPreviousRect, previousButton, buttonStylePrevious))
    //    {
    //        Debug.Log("Load " + previousScene);
    //        SceneManager.LoadScene(previousScene);
    //        if (index == 0)
    //        {
    //            index = max;
    //        }
    //        else
    //        {
    //            index--;
    //        }
    //    }
    //}
}
