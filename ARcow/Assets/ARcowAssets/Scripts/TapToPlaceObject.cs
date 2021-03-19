using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
//using UnityEngine.Experimental.XR
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using System;

public class TapToPlaceObject : MonoBehaviour
{
    public GameObject objectToPlace;
    public GameObject placementIndicator;
    //private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid = false;
    static private List<GameObject> objects; // stores instantiated objects

    //Input variables
    public Vector2 startPos;
    public Vector2 direction;
    public const float MIN_SWIPE_DISTANCE = 3.0f; //might have to adjust it to a higher value [lastVal:0.30]

    //Canvas Variables
    public Dropdown dropdown;
    static private List<string> objNames;
    static int objCounter;
    GameObject curCow;

    void Start()
    {
        //arOrigin = FindObjectOfType<ARSessionOrigin>();
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();
        objects = new List<GameObject>();
        objNames = new List<string>();
        dropdown.onValueChanged.AddListener(delegate {DropdownItemSelected(dropdown);});
    }

    void DropdownItemSelected(Dropdown dropdown){
        if(dropdown.value > 0){
            curCow = objects[dropdown.value-1];
        }
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        ReceiveTouchInput();

    }

    private void ReceiveTouchInput(){
        if(Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            
            switch (touch.phase){
                case TouchPhase.Began:
                    startPos = touch.position;
                    break;
                case TouchPhase.Moved:
                    direction = touch.position - startPos;
                    if(Mathf.Abs(direction.x) >= MIN_SWIPE_DISTANCE){
                        /*foreach(GameObject obj in objects){
                            obj.transform.Rotate(0f, touch.deltaPosition.x,0f);
                        }*/
                        curCow.transform.Rotate(0f,touch.deltaPosition.x, 0f);                       
                    }
                    break;
                case TouchPhase.Ended:
                    if(placementPoseIsValid && Mathf.Abs(direction.x) < MIN_SWIPE_DISTANCE && direction.y < MIN_SWIPE_DISTANCE && !EventSystem.current.IsPointerOverGameObject(touch.fingerId)){
                        PlaceObject();
                    }
                    break;
            } 
        }
    }
    private void PlaceObject()
    {
        GameObject obj = Instantiate(objectToPlace, placementPose.position,placementPose.rotation);
        objects.Add(obj);
        objCounter++;
        /*objNames.Add("Cow" + objCounter);
        dropdown.AddOptions(objNames);
        Debug.Log(objNames);*/
        dropdown.options.Add(new Dropdown.OptionData() {text = "Cow" + objCounter});
    }


    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        { //show green arrow
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
		{ //show red arrow
            placementIndicator.SetActive(false);
		}
	}

    private void UpdatePlacementPose()
	{
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneEstimated);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
		{
            placementPose = hits[0].pose;

            /*var cameraForward = Camera.current.transform.forward; 
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized; //don't care about the vertical rotation of the camera. 
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);*/
		}
	}
}