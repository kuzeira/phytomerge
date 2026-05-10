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

    [Header("Order 2")]
    public IngredientData neededIngredient2;
    public int neededCount2 = 0;
    public Image ingredientIcon2;
    public TMP_Text countText2;

    [Header("Timer")]
    public float timeLimit = 30f;
    public TMP_Text timerText;

    [Header("Win Screen")]
    public GameObject winPanel;

    private float currentTime;
    private bool orderCompleted;
    private bool orderFailed;

    private void Start()
    {
        currentTime = timeLimit;
        orderCompleted = false;
        orderFailed = false;

        if (winPanel != null)
            winPanel.SetActive(false);

        RefreshUI();
        UpdateTimerUI();
    }

    private void Update()
    {
        if (orderCompleted || orderFailed)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            orderFailed = true;
            UpdateTimerUI();
        }

        UpdateTimerUI();
    }

    public void OnIngredientMatched(IngredientData matchedIngredient)
    {
        if (matchedIngredient == null || orderCompleted || orderFailed)
            return;

        if (matchedIngredient == neededIngredient1 && neededCount1 > 0)
        {
            neededCount1--;
        }
        else if (matchedIngredient == neededIngredient2 && neededCount2 > 0)
        {
            neededCount2--;
        }

        RefreshUI();

        if (IsOrderComplete())
        {
            orderCompleted = true;
            UpdateTimerUI();

            if (winPanel != null)
                winPanel.SetActive(true);

            Debug.Log("LEVEL COMPLETE!");
        }
    }

    private bool IsOrderComplete()
    {
        bool firstDone = neededIngredient1 == null || neededCount1 <= 0;
        bool secondDone = neededIngredient2 == null || neededCount2 <= 0;

        return firstDone && secondDone;
    }

    private void RefreshUI()
    {
        if (ingredientIcon1 != null)
        {
            bool show = neededIngredient1 != null && neededCount1 > 0;
            ingredientIcon1.sprite = show ? neededIngredient1.icon : null;
            ingredientIcon1.enabled = show;
            ingredientIcon1.preserveAspect = true;
        }

        if (countText1 != null)
            countText1.text = neededIngredient1 != null && neededCount1 > 0 ? "x" + neededCount1 : "";

        if (ingredientIcon2 != null)
        {
            bool show = neededIngredient2 != null && neededCount2 > 0;
            ingredientIcon2.sprite = show ? neededIngredient2.icon : null;
            ingredientIcon2.enabled = show;
            ingredientIcon2.preserveAspect = true;
        }

        if (countText2 != null)
            countText2.text = neededIngredient2 != null && neededCount2 > 0 ? "x" + neededCount2 : "";
    }

    private void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        if (orderCompleted)
        {
            timerText.text = "OK";
            timerText.color = Color.green;
        }
        else if (orderFailed)
        {
            timerText.text = "X";
            timerText.color = Color.red;
        }
        else
        {
            timerText.text = Mathf.CeilToInt(currentTime).ToString();
            timerText.color = Color.white;
        }
    }
}