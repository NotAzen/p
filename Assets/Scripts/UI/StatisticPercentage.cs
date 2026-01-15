using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System;
using UnityEngine.UI;

public class StatisticPercentage : MonoBehaviour
{
    // --------------------------------------------------------------------------------- //
    // PUBLIC VARIABLES

    [Header("Statistics")]
    [SerializeField] GameObject HPStatsUI;
    [SerializeField] GameObject StaminaStatsUI;

    // funny class thingy that manages all the UI stuff for a player statistic
    private class StatisticBarHUDHandler
    {
        // variables needed for this class to even function properly LOL
        private TextMeshProUGUI UIText;
        private GameObject OverallMask;
        private Image Bar;
        private Image DifferenceBar;

        private float displayStatistic;
        private float smoothingSpeed = 5f;

        private float startDifferenceTime;

        // constructor that takes a UI GameObject and a game statistic value (class initialization)
        public StatisticBarHUDHandler(GameObject uiObject)
        {
            // initialize variables
            UIText = uiObject.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            OverallMask = uiObject.transform.Find("Overall Mask").gameObject;
            Bar = OverallMask.transform.Find("Bar").GetComponent<Image>();
            DifferenceBar = OverallMask.transform.Find("ChangeBar").GetComponent<Image>();

            displayStatistic = 0f;
        }

        // ----------------------------------------------------------------------------------- //
        // PUBLIC METHODS

        public void UpdateDisplay(Statistic statistic)
        {
            // grab current and max statistic values
            float statisticCurrent = statistic.CurrentValue;
            float statisticMax = statistic.maxValue;

            // calculate smoothed display value and percentage
            displayStatistic = Mathf.Lerp(displayStatistic, statisticCurrent, smoothingSpeed * Time.deltaTime);
            float percentage = displayStatistic / statisticMax;

            // number displayed
            string displayNum = Mathf.RoundToInt(displayStatistic).ToString();
            UIText.text = displayNum;

            // bar update
            Bar.fillAmount = percentage;

            // check if difference timer needs to be updated
            if (MathF.Abs(displayStatistic - statisticCurrent) > 2f) { UpdateDifferenceTimer(); }

            // update difference bar when timer is active
            if (Time.time > startDifferenceTime)
            {
                DifferenceBar.fillAmount = Mathf.Lerp(DifferenceBar.fillAmount, percentage, 10f * Time.deltaTime);
            }
        }

        public void UpdateDifferenceTimer()
        {
            // set time to start regenerating stamina
            startDifferenceTime = Time.time + 0.6f;
        }
    }

    // ui handlers for health and stamina
    private StatisticBarHUDHandler healthHandler;
    private StatisticBarHUDHandler staminaHandler;

    // --------------------------------------------------------------------------------- //
    // PRIVATE VARIABLES

    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthHandler = new StatisticBarHUDHandler(HPStatsUI);
        staminaHandler = new StatisticBarHUDHandler(StaminaStatsUI);

        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        PlayerStatistics playerController = player.GetComponent<PlayerStatistics>();
        healthHandler.UpdateDisplay(playerController.health);
        staminaHandler.UpdateDisplay(playerController.stamina);
    }

}
