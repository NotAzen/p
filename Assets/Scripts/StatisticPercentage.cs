using UnityEngine;

public class StatisticPercentage : MonoBehaviour
{
    // --------------------------------------------------------------------------------- //
    // PUBLIC VARIABLES

    [SerializeField] GameObject Player;

    [Header("Statistics")]
    [SerializeField] GameObject HPStatsUI;
    [SerializeField] GameObject StaminaStatsUI;

    // class for handling UI updates related to the statistic it tracks
    class UIHandler
    {
        // public variables for the UI GameObject and the game statistic value
        public GameObject UIObject;
        public TextMesh UIText;
        public float GameStatistic;

        // constructor that takes a UI GameObject and a game statistic value (class initialization)
        public UIHandler(GameObject uiObject, float gameStatistic)
        {
            UIObject = uiObject;
            GameStatistic = gameStatistic;
        }

        // ----------------------------------------------------------------------------------- //
        // PUBLIC METHODS

        public void UpdateUI()
        {
            Debug.Log("Updating UI...");
        }

        public void UpdateText()
        {
            Debug.Log("Updating Text...");
        }
    }

    // --------------------------------------------------------------------------------- //
    // PRIVATE VARIABLES

    private Transform FullHP;
    private Transform FullStamina;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //FullHP = HPBar.transform;
        //FullStamina = StaminaBar.transform;

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowPercentage()
    {
        Debug.Log("Showing statistic percentage...");
    }
}
