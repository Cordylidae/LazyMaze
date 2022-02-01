using UnityEngine;

public class Orientation : MonoBehaviour
{
    [SerializeField] private GameObject UIPortrait;
    [SerializeField] private GameObject UILandscape;

    private Camera camera;

    private bool isSwapLandscape = false;
    private bool isSwapPortrait = true;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        if (isSwapLandscape == false && Screen.orientation == ScreenOrientation.Landscape) ToLandscape();
        if (isSwapPortrait == false && Screen.orientation == ScreenOrientation.Portrait) ToPortrait();
    }


    // TODO: something normal for orientation
    private void ToLandscape()
    {
        camera.orthographicSize = 2.8f;

        UILandscape.SetActive(true);
        UIPortrait.SetActive(false);

        isSwapLandscape = true;
        isSwapPortrait = false;
    }

    private void ToPortrait()
    {
        camera.orthographicSize = 5.0f;

        UIPortrait.SetActive(true);
        UILandscape.SetActive(false);

     
        isSwapPortrait = true;
        isSwapLandscape = false;
    }


}


