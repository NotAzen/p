using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class StatisticPercentage : MonoBehaviour
{
    // --------------------------------------------------------------------------------- //
    // PUBLIC VARIABLES

    [Header("Statistics")]
    [SerializeField] GameObject HPStatsUI;
    [SerializeField] GameObject StaminaStatsUI;

    // funny class thingy that manages all the UI stuff for a player statistic
    public class PlayerStatisticHUDHandler
    {
        // variables needed for this class to even function properly LOL
        private TextMeshProUGUI UIText;
        private GameObject OverallMask;
        private RectTransform BarPosition;

        private RectTransform FullBarPosition;
        private RectTransform EmptyBarPosition;

        private float displayStatistic;
        private float smoothingSpeed = 5f;

        // constructor that takes a UI GameObject and a game statistic value (class initialization)
        public PlayerStatisticHUDHandler(GameObject uiObject)
        {
            // initialize variables
            UIText = uiObject.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            OverallMask = uiObject.transform.Find("OverallMask").gameObject;
            BarPosition = OverallMask.transform.Find("Bar").GetComponent<RectTransform>();

            FullBarPosition = OverallMask.transform.Find("100Percent").GetComponent<RectTransform>();
            EmptyBarPosition = OverallMask.transform.Find("0Percent").GetComponent<RectTransform>();

            displayStatistic = 0f;

            print("yeah i exist now");
        }

        // ----------------------------------------------------------------------------------- //
        // PUBLIC METHODS

        public void UpdateDisplay(float statistic, float statisticMax)
        {
            // calculate smoothed display value and percentage
            displayStatistic = Mathf.Lerp(displayStatistic, statistic, smoothingSpeed * Time.deltaTime);
            float percentage = displayStatistic / statisticMax;

            // number displayed
            string displayNum = Mathf.RoundToInt(displayStatistic).ToString();
            UIText.text = displayNum;

            // bar position update
            Vector3 newPosition = Vector3.Lerp(EmptyBarPosition.localPosition, FullBarPosition.localPosition, percentage);
            BarPosition.localPosition = newPosition;
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthHandler = new PlayerStatisticHUDHandler(HPStatsUI);
        staminaHandler = new PlayerStatisticHUDHandler(StaminaStatsUI);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
