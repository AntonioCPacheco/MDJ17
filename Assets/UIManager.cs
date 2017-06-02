using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class UIManager : MonoBehaviour
{

    GameObject[] pauseObjects;
    GameObject[] deathObjects;
    GameObject controlsText;
    GameObject player;
    public string aiFilepath;
    public Text aiText;
    public bool textHidden;
    GameObject mainMenuBackground;
    StreamReader reader;
    string[] lines;
    int countLines;

    // Use this for initialization
    void Start()
    {
        //DontDestroyOnLoad(this.transform.parent.gameObject);

        textHidden = true;
        Time.timeScale = 1;
        reader = new StreamReader(aiFilepath);
        countLines = 1;
        lines = reader.ReadToEnd().Split("\n"[0]);
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        deathObjects = GameObject.FindGameObjectsWithTag("ShowOnDeath");
        controlsText = GameObject.Find("ControlsText");
        controlsText.SetActive(false);
        if (!SceneManager.GetActiveScene().name.Equals("MainMenu"))
        {
            hidePaused();
            hideDeath();
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            mainMenuBackground = GameObject.Find("MainMenuBackground");
            mainMenuBackground.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
            Destroy(player);
        if (!SceneManager.GetActiveScene().name.Equals("MainMenu"))
        {
            if (Input.GetKeyDown(KeyCode.Mouse1) && textHidden)
            {
                Debug.Log("show");
                ReadString();
                textHidden = false;
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Mouse1) && !textHidden)
                {
                    Debug.Log("hide");
                    aiText.text = "";
                    textHidden = true;
                }
            }
        }

        if (player == null)
        {
            Time.timeScale = 0;
            showDeath();
            if (Input.GetKeyDown(KeyCode.Space))
                Reload();
            else if (Input.GetKeyDown(KeyCode.Escape))
                LoadLevel("MainMenu");
        }
        else
        {
            //uses the ESC button to pause and unpause the game
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (SceneManager.GetActiveScene().name.Equals("MainMenu"))
                {
                    Reload();
                }
                else if (Time.timeScale == 1)
                {
                    Time.timeScale = 0;
                    showPaused();
                }
                else if (Time.timeScale == 0)
                {
                    if (GameObject.Find("ControlsText") != null)
                    {
                        controlsText.SetActive(false);
                        showPaused();
                    }
                    else
                    {
                        Debug.Log("high");
                        Time.timeScale = 1;
                        hidePaused();
                    }
                }
            }
        }
    }


    //Reloads the Level
    public void Reload()
    {
        reader.Close();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //controls the pausing of the scene
    public void pauseControl()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            showPaused();
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            hidePaused();
        }
    }

    //shows objects with ShowOnPause tag
    public void showPaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(true);
        }
    }

    //hides objects with ShowOnPause tag
    public void hidePaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
        }
    }

    public void showDeath()
    {
        foreach (GameObject g in deathObjects)
        {
            g.SetActive(true);
        }
    }

    //hides objects with ShowOnPause tag
    public void hideDeath()
    {
        foreach (GameObject g in deathObjects)
        {
            g.SetActive(false);
        }
    }

    //loads inputted level
    public void LoadLevel(string level)
    {
        reader.Close();
        SceneManager.LoadScene(level);
    }

    public void helpControls()
    {
        //Debug.Log("CONTROLS");
        //if (!SceneManager.GetActiveScene().name.Equals("MainMenu"))
        //{
        foreach (GameObject g in pauseObjects)
        {
            if (g.name.Contains("Button"))
                g.SetActive(false);
        }
        controlsText.SetActive(true);
        if (SceneManager.GetActiveScene().name.Equals("MainMenu"))
            mainMenuBackground.SetActive(true);
        //}
    }

    public void terminate()
    {
        Application.Quit();
    }

    public void ReadString()
    {
        //Read the text from directly from the test.txt file
        //StreamReader reader = 

        //Debug.Log("READ");
        //Debug.Log(aiText.text);
        //aiText.text += reader.ReadToEnd();
        //aiText.text += reader.ReadLine();
        if (countLines < lines.Length)
        {
            aiText.text += lines[countLines];
            countLines++;
            Debug.Log(aiText.text);
        }
        //reader.Close();
    }
}
