using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCOrderUI : MonoBehaviour
{
    [Header("Order 1")]
    public IngredientData neededIngredient1;
    public int neededCount1 = 1;
    public Image ingredientIcon1;
    public TMP_Text countText1;

    [Header("Order 2 (optional)")]
    public IngredientData neededIngredient2;
    public int neededCount2 = 0;
    public Image ingredientIcon2;
    public TMP_Text countText2;

    [Header("Timer")]
    public float timeLimit = 20f;
    public TMP_Text timerText;
    public Image timerFill;

    [Header("State")]
    public bool orderCompleted = false;
    public bool orderFailed = false;

    private float currentTime;

    private void Start()
    {
        currentTime = timeLimit;
        RefreshUI();
        UpdateTimerUI();
    }

    private void Update()
    {
        if (orderCompleted || orderFailed)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            orderFailed = true;
            Debug.Log("NPC заказ провален по таймеру");
        }

        UpdateTimerUI();
    }

    public void OnIngredientMatched(IngredientData matchedIngredient)
    {
        if (matchedIngredient == null || orderCompleted || orderFailed)
            return;

        if (neededIngredient1 == matchedIngredient && neededCount1 > 0)
        {
            neededCount1--;
        }
        else if (neededIngredient2 == matchedIngredient && neededCount2 > 0)
        {
            neededCount2--;
        }

        RefreshUI();

        if (IsOrderComplete())
        {
            orderCompleted = true;
            Debug.Log("NPC заказ выполнен!");
        }
    }

    public bool IsOrderComplete()
    {
        bool firstDone = neededIngredient1 == null || neededCount1 <= 0;
        bool secondDone = neededIngredient2 == null || neededCount2 <= 0;
        return firstDone && secondDone;
    }

    public void RefreshUI()
    {
        if (ingredientIcon1 != null)
        {
            bool showFirst = neededIngredient1 != null && neededCount1 > 0;
            ingredientIcon1.sprite = showFirst ? neededIngredient1.icon : null;
            ingredientIcon1.enabled = showFirst;
        }

        if (countText1 != null)
        {
            if (neededIngredient1 == null)
                countText1.text = "";
            else if (neededCount1 > 0)
                countText1.text = "x" + neededCount1;
            else
                countText1.text = "Done";
        }

        if (ingredientIcon2 != null)
        {
            bool showSecond = neededIngredient2 != null && neededCount2 > 0;
            ingredientIcon2.sprite = showSecond ? neededIngredient2.icon : null;
            ingredientIcon2.enabled = showSecond;
        }

        if (countText2 != null)
        {
            if (neededIngredient2 == null || neededCount2 <= 0)
                countText2.text = "";
            else
                countText2.text = "x" + neededCount2;
        }

        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            if (orderCompleted)
                {
                    timerText.text = "✓";
                    timerText.color = Color.green;
                }
            else if (orderFailed)
                timerText.text = "Time up";
            else
                timerText.text = Mathf.CeilToInt(currentTime).ToString();
        }

        if (timerFill != null && timeLimit > 0f)
        {
            timerFill.fillAmount = currentTime / timeLimit;
        }
    }

    public void ResetOrder(
        IngredientData newIngredient1, int newCount1,
        IngredientData newIngredient2 = null, int newCount2 = 0,
        float newTimeLimit = 20f)
    {
        neededIngredient1 = newIngredient1;
        neededCount1 = newCount1;

        neededIngredient2 = newIngredient2;
        neededCount2 = newCount2;

        timeLimit = newTimeLimit;
        currentTime = timeLimit;

        orderCompleted = false;
        orderFailed = false;

        RefreshUI();
    }
}