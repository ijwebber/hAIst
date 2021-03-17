using System.Collections;

using UnityEngine;

public class BarController : MonoBehaviour
{
    public static BarController Instance { get; private set; }
    [SerializeField] private GameObject barContainer;
    [SerializeField] private Animator barAnimator;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void ShowBars()
    {
        barContainer.SetActive(true);
    }
    public void HideBars()
    {
        if (barContainer.activeSelf)
        {
            StartCoroutine(HideBarsAndDisable());
        }
    }


    private IEnumerator HideBarsAndDisable()
    {
        
        barAnimator.SetTrigger("HideBars");
        yield return new WaitForSeconds(0.5f);
        barContainer.SetActive(false);
    }
}
