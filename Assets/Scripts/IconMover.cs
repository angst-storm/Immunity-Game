using UnityEngine;

public class IconMover : MonoBehaviour
{
    void Update()
    {
        if (transform.position.x >= 350)
        {
            transform.Translate(-5, 0, 0);
        }
    }
}
