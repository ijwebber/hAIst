using UnityEngine;

public class AlarmLight : MonoBehaviour
{
    Light alarm;
    public int speed = 5;
    public float maxIntensity = 0.5f;
    public float minIntensity = 0.1f;
    private bool lightOn = false;

    void Start()
    {
        alarm = GetComponent<Light>();
        alarm.intensity = 0;
    }

    void Update()
    {
        if (lightOn) {
            float diff = ((maxIntensity - minIntensity) / 2);
            alarm.intensity =  diff * Mathf.Sin(speed * Time.time) + diff + minIntensity;
        }
    }

    public void On() {
        lightOn = true;
    }

    public void Off() {
        lightOn = false;
        alarm.intensity = 0;
    }
}
