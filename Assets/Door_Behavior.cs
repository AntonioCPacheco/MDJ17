using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door_Behavior : MonoBehaviour {

    public Transform[] switches;
    public Material OpenM;
    public Material ClosedM;
    bool opened = false;
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        bool open = true;
        foreach (Transform t in switches)
        {
            if (!t.GetComponent<Switch_Behaviour>().isActivated())
            {
                open = false;
                break;
            }
        }
        this.GetComponent<Renderer>().material = open ? OpenM : ClosedM;
        opened = open;
    }

    public bool isOpen()
    {
        return opened;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name.Equals("player") && opened)
        {
            string name = "MainMenu";
            if (SceneManager.GetActiveScene().name.Equals("First Level"))
                name = "Second Level";
            else if (SceneManager.GetActiveScene().name.Equals("Second Level"))
                name = "Third Level";

            GameObject.Find("player").GetComponent<LevelManager>().saveTime();
            GameObject.Find("UIManager").GetComponent<UIManager>().LoadLevel(name);
        }
    }
}
