// ----------------------------------------------------------------------------
// Brief: https://forum.unity.com/threads/canvashelper-resizes-a-recttransform-to-iphone-xs-safe-area.521107/
// ----------------------------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(RectTransform))]
public class CanvasHelper : MonoBehaviour
{
    public UnityEvent onOrientationChange = new UnityEvent();
    public UnityEvent onResolutionChange = new UnityEvent();
    public bool IsLandscape { get; private set; }

    ScreenOrientation lastOrientation = ScreenOrientation.Portrait;
    Vector2 lastResolution = Vector2.zero;
    Vector2 lastSafeArea = Vector2.zero;

    Vector2 wantedReferenceResolution = Vector2.zero;
    Camera wantedCanvasCamera;

    Canvas canvas;
    CanvasScaler scaler;
    RectTransform rectTransform;
    RectTransform safeAreaTransform;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        scaler = GetComponent<CanvasScaler>();
        rectTransform = GetComponent<RectTransform>();

        wantedReferenceResolution = new Vector2(scaler.referenceResolution.x, scaler.referenceResolution.y);
        UpdateReferenceResolution();
        UpdateCanvasCamera();

        safeAreaTransform = transform.Find("SafeArea") as RectTransform;

        lastOrientation = Screen.orientation;
        lastResolution.x = Screen.width;
        lastResolution.y = Screen.height;
        lastSafeArea = Screen.safeArea.size;
    }

    void Start()
    {
        ApplySafeArea();
    }

    /*
    void Update()
    {
        if (Application.isMobilePlatform)
        {
            if (Screen.orientation != lastOrientation)
                OrientationChanged();

            if (Screen.safeArea.size != lastSafeArea)
                SafeAreaChanged();
        }
        else
        {
            //resolution of mobile devices should stay the same always, right?
            // so this check should only happen everywhere else
            if (!Mathf.Approximately(Screen.width, lastResolution.x) || !Mathf.Approximately(Screen.height, lastResolution.y))
                ResolutionChanged();
        }
    }
    */

    void ApplySafeArea()
    {
        if (safeAreaTransform == null)
            return;

        Rect safeArea = Screen.safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= canvas.pixelRect.width;
        anchorMin.y /= canvas.pixelRect.height;
        anchorMax.x /= canvas.pixelRect.width;
        anchorMax.y /= canvas.pixelRect.height;

        safeAreaTransform.anchorMin = anchorMin;
        safeAreaTransform.anchorMax = anchorMax;

        //Debug.Log(
            //"ApplySafeArea:" +
            //"\n Screen.orientation: " + Screen.orientation +
            //#if UNITY_IOS
            //"\n Device.generation: " + UnityEngine.iOS.Device.generation.ToString() +
            //#endif
            //"\n Screen.safeArea.position: " + Screen.safeArea.position.ToString() +
            //"\n Screen.safeArea.size: " + Screen.safeArea.size.ToString() +
            //"\n Screen.width / height: (" + Screen.width.ToString() + ", " + Screen.height.ToString() + ")" +
            //"\n canvas.pixelRect.size: " + canvas.pixelRect.size.ToString() +
            //"\n anchorMin: " + anchorMin.ToString() +
            //"\n anchorMax: " + anchorMax.ToString());
    }

    void UpdateCanvasCamera()
    {
        if (canvas.worldCamera == null && wantedCanvasCamera != null)
            canvas.worldCamera = wantedCanvasCamera;
    }

    void UpdateReferenceResolution()
    {
        scaler.referenceResolution = wantedReferenceResolution;
    }

    void OrientationChanged()
    {
        //Debug.Log("Orientation changed from " + lastOrientation + " to " + Screen.orientation + " at " + Time.time);

        lastOrientation = Screen.orientation;
        lastResolution.x = Screen.width;
        lastResolution.y = Screen.height;

        IsLandscape = lastOrientation == ScreenOrientation.LandscapeLeft || lastOrientation == ScreenOrientation.LandscapeRight || lastOrientation == ScreenOrientation.Landscape;
        onOrientationChange.Invoke();
    }

    void ResolutionChanged()
    {
        if (Mathf.Approximately(lastResolution.x, Screen.width) && Mathf.Approximately(lastResolution.y, Screen.height))
            return;

        //Debug.Log("Resolution changed from " + lastResolution + " to (" + Screen.width + ", " + Screen.height + ") at " + Time.time);

        lastResolution.x = Screen.width;
        lastResolution.y = Screen.height;

        IsLandscape = Screen.width > Screen.height;
        onResolutionChange.Invoke();
    }

    void SafeAreaChanged()
    {
        if (lastSafeArea == Screen.safeArea.size)
            return;

        //Debug.Log("Safe Area changed from " + lastSafeArea + " to " + Screen.safeArea.size + " at " + Time.time);

        lastSafeArea = Screen.safeArea.size;

        ApplySafeArea();
    }

    void SetAllCanvasCamera(Camera cam)
    {
        if (wantedCanvasCamera == cam)
            return;

        wantedCanvasCamera = cam;

        UpdateCanvasCamera();
    }

    public Vector2 CanvasSize()
    {
        return rectTransform.sizeDelta;
    }

    public Vector2 SafeAreaSize()
    {
        if (safeAreaTransform != null)
        {
            return safeAreaTransform.sizeDelta;
        }

        return CanvasSize();
    }

    public Vector2 GetReferenceResolution()
    {
        return wantedReferenceResolution;
    }
}
