using UnityEngine;
using TMPro;
using System.IO;
using Unity.VisualScripting;

public class StatisticPercentage : MonoBehaviour
{
    // --------------------------------------------------------------------------------- //
    // PUBLIC VARIABLES

    [Header("Statistics")]
    [SerializeField] GameObject HPStatsUI;
    [SerializeField] GameObject StaminaStatsUI;

    // Make the handler class public to match the accessibility of the field
    public class PlayerStatisticHUDHandler
    {
        // public variables for the UI GameObject and the game statistic value
        public GameObject UIObject;
        public TextMeshProUGUI UIText;

        private float displayStatistic;
        private float smoothingSpeed = 5f;

        // constructor that takes a UI GameObject and a game statistic value (class initialization)
        public PlayerStatisticHUDHandler(GameObject uiObject)
        {
            // initialize variables
            UIObject = uiObject;
            UIText = uiObject.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            
            displayStatistic = 0f;
        }

        // ----------------------------------------------------------------------------------- //
        // PUBLIC METHODS

        public void UpdateDisplay(float statistic)
        {
            displayStatistic = Mathf.Lerp(displayStatistic, statistic, smoothingSpeed * Time.deltaTime);
            string displayNum = Mathf.RoundToInt(displayStatistic).ToString();
            UIText.text = displayNum;
        }

        public void UpdateText()
        {
            Debug.Log("Updating Text...");
        }
    }

    // ui handlers for health and stamina
    public PlayerStatisticHUDHandler healthHandler;
    public PlayerStatisticHUDHandler staminaHandler;

    // --------------------------------------------------------------------------------- //
    // PRIVATE VARIABLES

    private Transform FullHP;
    private Transform FullStamina;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //FullHP = HPBar.transform;
        //FullStamina = StaminaBar.transform;

        healthHandler = new PlayerStatisticHUDHandler(HPStatsUI);
        staminaHandler = new PlayerStatisticHUDHandler(StaminaStatsUI);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
