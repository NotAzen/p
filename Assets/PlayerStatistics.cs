using System;
using UnityEngine;

// --------------------------------------------------------------------------------- //
// CUSTOM CLASSES

[Serializable]
public class Statistic
{
    private float _currentValue;
    public float maxValue;
    [SerializeField] private float regenerationRate;
    [SerializeField] private Cooldown regenerationCooldown = new(0);

    // constructor (initialization)
    public Statistic(float max)
    {
        // idk set default values
        _currentValue = max;
        maxValue = max;
    }

    // property to get current value
    public float CurrentValue
    {
        get { return _currentValue; }
    }

    // method for checking if statistic has enough value
    public bool Has(float amount)
    {
        return _currentValue >= amount;
    }

    // method to consume the statistic (like stamina)
    public void Consume(float amount)
    {
        _currentValue -= amount;
        regenerationCooldown.Trigger();
    }

    // method to regenerate the statistic over time
    public void Regenerate()
    {
        if (regenerationCooldown.IsReady())
        {
            _currentValue += regenerationRate * Time.deltaTime;
            _currentValue = Mathf.Min(_currentValue, maxValue); // clamp to max value
        }
    }
}

[Serializable]
public class Cooldown
{
    private float lastUsedTime;
    [SerializeField] private float cooldownDuration;

    // constructor (initialization)
    public Cooldown(float duration)
    {
        // default values ig
        cooldownDuration = duration;
        lastUsedTime = -duration; // so it's ready at start
    }

    // method to check if cooldown is ready
    public bool IsReady()
    {
        return Time.time >= lastUsedTime + cooldownDuration;
    }

    // method to trigger the cooldown
    public void Trigger()
    {
        lastUsedTime = Time.time;
    }
}

public class PlayerStatistics : MonoBehaviour
{
    [Header("Player Statistics")]
    public Statistic health = new(100f);
    public Statistic stamina = new(30f);

    public Cooldown iframes = new(0.5f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        health.Regenerate();
        stamina.Regenerate();
    }
}
