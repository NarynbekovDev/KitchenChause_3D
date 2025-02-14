using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSucces;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }


    [SerializeField] private RecipeListSO recipeListSO;


    private List<RecipeSO> waitingRecipeSOList;

    private float _spawnRecipeTimer;
    private float _spawnRecipeTimerMax = 4f;
    private int _waitingRecipesMax = 4;
    private int _successfulRecipesAmount;


    private void Awake()
    {
        Instance = this;

        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update()
    {
        _spawnRecipeTimer -= Time.deltaTime;
        if (_spawnRecipeTimer <= 0f)
        {
            _spawnRecipeTimer = _spawnRecipeTimerMax;

            if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < _waitingRecipesMax)
            {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];

                waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                // Has the same number of ingredients
                bool plateContentsMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    // Cycling though all ingredients in the Recipe
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        // Cycling though all ingredients in the Plate
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            // Ingredient matches!
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        // This Recipe ingredient was not found on the Plate
                        plateContentsMatchesRecipe = false;
                    }
                }

                if (plateContentsMatchesRecipe)
                {
                    // Player delivered the correct recipe!

                    _successfulRecipesAmount++;

                    waitingRecipeSOList.RemoveAt(i);

                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipeSucces?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }

        // No matches found!
        // Player did not deliver a correct recipe
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount()
    {
        return _successfulRecipesAmount;
    }
}
