using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System;

public class StatisticPercentage : MonoBehaviour
{
    // --------------------------------------------------------------------------------- //
    // PUBLIC VARIABLES

    [Header("Statistics")]
    [SerializeField] GameObject HPStatsUI;
    [SerializeField] GameObject StaminaStatsUI;

    // funny class thingy that manages all the UI stuff for a player statistic
    public class StatisticBarHUDHandler
    {
        // variables needed for this class to even function properly LOL
        private TextMeshProUGUI UIText;
        private GameObject OverallMask;
        private RectTransform BarPosition;
        private RectTransform DifferenceBarPosition;

        private RectTransform FullBarPosition;
        private RectTransform EmptyBarPosition;

        private float displayStatistic;
        private float smoothingSpeed = 5f;

        private float startDifferenceTime;

        // constructor that takes a UI GameObject and a game statistic value (class initialization)
        public StatisticBarHUDHandler(GameObject uiObject)
        {
            // initialize variables
            UIText = uiObject.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            OverallMask = uiObject.transform.Find("Overall Mask").gameObject;
            BarPosition = OverallMask.transform.Find("Bar").GetComponent<RectTransform>();
            DifferenceBarPosition = OverallMask.transform.Find("ChangeBar").GetComponent<RectTransform>();

            FullBarPosition = OverallMask.transform.Find("100Percent").GetComponent<RectTransform>();
            EmptyBarPosition = OverallMask.transform.Find("0Percent").GetComponent<RectTransform>();

            displayStatistic = 0f;
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

            // check if difference timer needs to be updated
            if (MathF.Abs(displayStatistic - statistic) > 2f) { UpdateDifferenceTimer(); }

            // update difference bar position when timer is active
            if (Time.time > startDifferenceTime)
            {
                DifferenceBarPosition.localPosition = Vector3.Lerp(DifferenceBarPosition.localPosition, newPosition, 10f * Time.deltaTime);
            }
        }

        public void UpdateDifferenceTimer()
        {
            // set time to start regenerating stamina
            startDifferenceTime = Time.time + 0.6f;
        }
    }

    // ui handlers for health and stamina
    public StatisticBarHUDHandler healthHandler;
    public StatisticBarHUDHandler staminaHandler;

    // --------------------------------------------------------------------------------- //
    // PRIVATE VARIABLES


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthHandler = new StatisticBarHUDHandler(HPStatsUI);
        staminaHandler = new StatisticBarHUDHandler(StaminaStatsUI);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
