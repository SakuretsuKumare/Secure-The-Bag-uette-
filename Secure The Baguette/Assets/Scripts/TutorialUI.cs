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
        tutorialPanel.SetActive(false);
        hasShown = false;
    }

    private void OnTriggerEnter(Collider player)
    {
        if (player.gameObject.tag == "Player" && hasShown == false)
        {
            StartCoroutine("TutorialScreen");
        }
    }

    IEnumerator TutorialScreen()
    {
        textBox.text = tutorialText;
        tutorialPanel.SetActive(true);
        yield return new WaitForSeconds(4f);
        tutorialPanel.SetActive(false);
        hasShown = true;
    }
}