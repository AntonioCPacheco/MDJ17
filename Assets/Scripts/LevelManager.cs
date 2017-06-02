using System.Collections;
using System.Collections.Generic; //to use lists
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class LevelManager : MonoBehaviour {

    public static LevelManager instance = null;              //Static instance of LevelManager which allows it to be accessed by any other script.
    private int level = 0, intervalTime = 0;                            //Initial level
    private float time = 0;
    private bool levelStatus = false;
    string FILE_NAME = "time.txt";
    string FILE_NAME2 = "positions.txt";

    //Awake is always called before any Start functions
    void Awake()
    {
        /*
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        //DontDestroyOnLoad(gameObject); 

        //Call the InitGame function to initialize the first level 
        //InitGame();
        */
    }
    // Use this for initialization
    void Start ()
    {
        StreamWriter sw = File.AppendText(FILE_NAME2);
        sw.WriteLine("NEW PLAYER, " + SceneManager.GetActiveScene().name);
        sw.Close();
        //InitGame();
    }

    //Initializes the game
    void InitGame()
    {
        //Loads level
        SceneManager.LoadSceneAsync(SceneManager.GetSceneAt(level).name); //Gets the first level
    }

    // Update is called once per frame
    void Update () {
        time+=Time.deltaTime;
        if (intervalTime > 50)
        {
            float x = this.transform.position.x;
            float y = this.transform.position.y;
            StreamWriter sw = File.AppendText(FILE_NAME2);
            sw.WriteLine("My x position is " + x + " My y position is " + y);
            sw.Close();
            //Debug.Log("Written to file");
            intervalTime = 0;
        }
        else
            intervalTime++;
    }

    public void saveTime()
    {
        StreamWriter sw = File.AppendText(FILE_NAME);
        sw.WriteLine("You took: " + time + " seconds!");
        sw.Close();
    }
}
