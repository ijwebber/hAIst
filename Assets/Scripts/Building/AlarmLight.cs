using UnityEngine;

public class AlarmLight : MonoBehaviour
{
    Light alarm;
    public int speed = 5;
    public float maxIntensity = 0.5f;
    public float minIntensity = 0.1f;
    private bool lightOn = false;
    
    private GuardController guardController;

    void Start()
    {
        alarm = GetComponent<Light>();
        alarm.intensity = 0;

        guardController = GameObject.FindObjectOfType<GuardController>();
    }

    void Update()
    {
        if (guardController.inChase()) {
            float diff = ((maxIntensity - minIntensity) / 2);
            alarm.intensity =  diff * Mathf.Sin(speed * Time.time) + diff + minIntensity;
        } else {
            Off();
        }
    }


    public void Off() {
        alarm.intensity = 0;
    }
}
