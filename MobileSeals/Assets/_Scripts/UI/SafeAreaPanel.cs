using System;
using UnityEngine;

public class SafeAreaPanel : MonoBehaviour
{
    [SerializeField] bool updateAnchors = false;
    [SerializeField] RectTransform _rectTransform;
    private void Start()
    {
        UpdateAnchors();
    }

    private void UpdateAnchors()
    {
        Rect safeArea = Screen.safeArea;

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 anchorMin = safeArea.position / screenSize;
        Vector2 anchorMax = (safeArea.position + safeArea.size) / screenSize;

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (updateAnchors)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null) return; // object may have been destroyed
                UpdateAnchors();
                updateAnchors = false;

            };
        }

#endif
    }
}