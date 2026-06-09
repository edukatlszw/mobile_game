using UnityEngine;

public class UIScaleComponent : MonoBehaviour
{
    public void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
