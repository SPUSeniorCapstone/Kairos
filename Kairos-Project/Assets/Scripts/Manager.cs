using UnityEngine.SceneManagement;
using UnityEngine;

public class Manager : MonoBehaviour
{
    // way to show variable in editor but prevent editing?
    /*[SerializeField]*/ private int index = 0;
    /*[SerializeField]*/ private int count = 0;

    private static bool created = false;

    private void Awake()
    {
        // quick and dirty
        count = SceneManager.sceneCountInBuildSettings;
        index = SceneManager.GetActiveScene().buildIndex;
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }
        GetComponentInChildren<Canvas>().enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GetComponentInChildren<Canvas>().enabled = !GetComponentInChildren<Canvas>().enabled;
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
        GetComponentInChildren<Canvas>().enabled = false;
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
        GetComponentInChildren<Canvas>().enabled = false;
    }
}