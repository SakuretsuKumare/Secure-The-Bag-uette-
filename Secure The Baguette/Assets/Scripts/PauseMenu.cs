using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// This script was used from Hooson https://www.youtube.com/watch?v=tfzwyNS1LUY&t=54s

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject comingSoon;
    private bool pauseMenuActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Presses the Escape key to pause the game.
        if (pauseMenuActive == false)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pauseMenuActive = true;
                Pause();
            }
        }

        else if (pauseMenuActive == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pauseMenuActive = false;
                Resume();
            }
        }
    }

    public void Pause()
    {
        Cursor.visible = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Home()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}