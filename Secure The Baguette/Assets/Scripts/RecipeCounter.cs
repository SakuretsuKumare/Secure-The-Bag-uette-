using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecipeCounter : MonoBehaviour
{
    private int recipesCollected;
    public TextMeshProUGUI recipeAmountUI;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        recipesCollected = gameObject.GetComponent<CharacterMovement>().totalRecipesCollected + gameObject.GetComponent<CharacterMovement>().levelRecipesCollected.Count;
        recipeAmountUI.text = recipesCollected.ToString();
    }
}
