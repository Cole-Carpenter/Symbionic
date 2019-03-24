using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public GameObject credits;

    private bool sCredits = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(sCredits && Input.GetButtonDown("Pause"))
        {
            sCredits = false;
            credits.SetActive(false);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Connor's_Honest-to-Goodness_Whopper_of_a_Scene");
    }

    public void Credits()
    {
        credits.SetActive(true);
        sCredits = true; ;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
