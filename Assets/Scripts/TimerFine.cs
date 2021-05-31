using UnityEngine;

public class TimerFine : MonoBehaviour
{
    private void TimerExpired()
    {
        GetComponentInParent<Threat>().TimerFine();
    }

}
