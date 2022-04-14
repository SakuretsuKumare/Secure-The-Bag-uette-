using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] Text textBox;
    public string tutorialText;
    private bool hasShown;

    // Start is called before the first frame update
    void Start()
    {
        //tutorialPanel = GameObject.Find("Canvas/Tutorial Panel");
        //textBox = GameObject.Find("Canvas/Tutorial Panel/Text").GetComponent<Text>();
        StartCoroutine("FirstTutorial");
        hasShown = false;
    }

    // When the player walks into a trigger box, tutorial screen pops up.
    private void OnTriggerEnter(Collider player)
    {
        if (player.gameObject.tag == "Player" && hasShown == false)
        {
            StartCoroutine("TutorialScreen");
        }
    }

    // Gets the words to say and displays the tutorial screen.
    IEnumerator TutorialScreen()
    {
        textBox.text = tutorialText;
        tutorialPanel.SetActive(true);
        yield return new WaitForSeconds(4f);
        tutorialPanel.SetActive(false);
        hasShown = true;
    }
    
    // Shows the first tutorial on Start()
    IEnumerator FirstTutorial()
    {
        tutorialPanel.SetActive(true);
        yield return new WaitForSeconds(4f);
        tutorialPanel.SetActive(false);
    }
}