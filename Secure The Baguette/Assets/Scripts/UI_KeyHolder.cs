using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This code was used from Code Monkey https://youtu.be/MIt0PJHMN5Y?t=823

public class UI_KeyHolder : MonoBehaviour
{
    [SerializeField] KeyHolderScript keyHolder;
    private Transform container;
    private Transform keyTemplate;

    private void Awake()
    {
        container = transform.Find("Container");
        keyTemplate = container.Find("KeyTemplate");
        keyTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        keyHolder.OnKeysChanged += KeyHolder_OnKeysChanged;
    }

    private void KeyHolder_OnKeysChanged(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        // Cleans up old keys.
        foreach (Transform child in container)
        {
            if (child == keyTemplate) continue;
            Destroy(child.gameObject);
        }

        // Instantiates current key list.
        List<KeyScript.KeyAccess> keyList = keyHolder.GetKeyList();
        for (int i = 0; i < keyList.Count; i++)
        {
            KeyScript.KeyAccess keyAccess = keyList[i];
            Transform keyTransform = Instantiate(keyTemplate, container);
            keyTransform.gameObject.SetActive(true);
            keyTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(150 * i, 0);
            Image keyImage = keyTransform.Find("Image").GetComponent<Image>();
            switch (keyAccess)
            {
                default:
                case KeyScript.KeyAccess.Level1: /*keyImage.color = Color.red;*/ break; //Maybe try keyImage.sprite = something else;
                case KeyScript.KeyAccess.Level2: /*keyImage.color = Color.blue;*/ break;
                case KeyScript.KeyAccess.Level3: /*keyImage.color = Color.cyan;*/ break;
            }
        }
    }
}