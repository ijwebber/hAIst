using UnityEngine;

public class AlarmLight : MonoBehaviour
{
    Light alarm;
    public int speed = 5;
    public float maxIntensity = 0.5f;
    public float minIntensity = 0f;

    void Start()
    {
        alarm = GetComponent<Light>();
    }

    void Update()
    {
        float diff = ((maxIntensity - minIntensity) / 2);

        alarm.intensity =  diff * Mathf.Sin(speed * Time.time) + diff + minIntensity;
    }
}
