using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PlatformMode
{
    DESKTOP,
    MOBILE
}


public struct PointOfInterest
{
    Vector3 location;

    string info;

    public PointOfInterest(Vector3 loc, string information)
    {
        location = loc;
        info = information;
    }

    public void MoveCamera(Camera t)
    {
        t.transform.position=location;
    }

    public void MoveObject(GameObject t)
    {
        t.transform.position = location;
    }
}
public class CameraController : MonoBehaviour
{
    [SerializeField]
    public PlatformMode platformTarget;

    private Vector2 lastMousePos;

    public float maxZoom;

    public float minZoom;

    private PointOfInterest startingLoc;
    // Start is called before the first frame update
    void Start()
    {
#if PLATFORM_STANDALONE_WIN
        platformTarget = PlatformMode.DESKTOP;
#elif !PLATFORM_STANDALONE_WIN
    platformTarget=PlatformMode.MOBILE;
#endif

        startingLoc = new PointOfInterest(MapToScroll.position, "Starting Camera Position");
    }

    public Transform MapToScroll;

    // this state is assocated only with ProcessTouch
    Vector3 PreviousWorldPoint;
    void ProcessTouch(Camera viewCamera, Vector3 screenPosition, bool isDown, bool wentDown)
    {
        // replace this part with however you want to get from screen mouse
        // coordinate to where it is in the world. This just casts it to a
        // Plane at y == 0
        var ray = viewCamera.ScreenPointToRay(screenPosition);
        var plane = new Plane(Vector3.forward*-1, Vector3.zero);
        float distance = 0;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);

            if (wentDown)
            {
                // on the frame the mouse went down, by definition we didn't move at all
                PreviousWorldPoint = worldPoint;
            }

            if (isDown)
            {
                // we are down, how much did we move in world space?
                Vector3 worldDelta = worldPoint - PreviousWorldPoint;

                // NOW! If you want the map to move FASTER than finger speed, you can
                // do that here, like so:
                //                worldDelta *= 2.0f;

                // move the map that much
                MapToScroll.position += worldDelta;
            }

            PreviousWorldPoint = worldPoint;
        }
    }
    public void Zoom(float amount)
    {
        transform.Translate(Vector3.back * amount);
    }
    private void ManageMouse()
    {
        if(platformTarget==PlatformMode.DESKTOP)
        {
            lastMousePos = Input.mousePosition;
        }
    }
    public bool isDown;


    private void ManageZoom()
    {
        transform.position = new Vector3(transform.position.x,
        transform.position.y,
        Mathf.Clamp(transform.position.z, maxZoom, minZoom));
    }

    public void ResetCameraPos()
    {
        startingLoc.MoveObject(MapToScroll.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPosition = Input.mousePosition;

        isDown = Input.GetMouseButton(0);
        bool wentDown = Input.GetMouseButtonDown(0);

        // at this point screenPosition, isDown and wentDown have been
        // abstracted from the core Input module; if you want to implement
        // something like multitouch, you can just replace the above code
        // and call the ProcessTouch() function below.

        // The camera is where you are regarding the world from.
        ProcessTouch(viewCamera: Camera.main, screenPosition: screenPosition, isDown: isDown, wentDown: wentDown);

        ManageZoom();
    }
}
