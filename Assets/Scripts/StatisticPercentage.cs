using UnityEngine;

public class StatisticPercentage : MonoBehaviour
{
    // --------------------------------------------------------------------------------- //
    // PUBLIC VARIABLES

    [Header("Statistics")]
    [SerializeField] GameObject HPBar;
    [SerializeField] GameObject StaminaBar;

    // --------------------------------------------------------------------------------- //
    // PRIVATE VARIABLES

    private Transform FullHP;
    private Transform FullStamina;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FullHP = HPBar.transform;
        FullStamina = StaminaBar.transform;
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
