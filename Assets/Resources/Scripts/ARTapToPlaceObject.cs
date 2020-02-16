using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;
using TMPro;

public class ARTapToPlaceObject : MonoBehaviour {
    public PageSwiper pageSwipe;
    public MessageDot objectToPlace;
    public GameObject placementIndicator;
    public GameObject placementUI;
    public TextMeshProUGUI currLocation;

    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRayManager;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    public static bool isPlacingMessage = false;
    private bool radiusMsgNotSpawned = true;
    private Message messageToPlace;
    void Start() {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arRayManager = arOrigin.GetComponent<ARRaycastManager>();
        placementIndicator.SetActive(false);
    }
    void Update() {
        if (!DirectionSense.playerOriginIsSet) {
            //do nothing
        }
        else if (radiusMsgNotSpawned) {
            radiusMsgNotSpawned = false;
            placeAllMsgWithinRadius();
        }
        else if(isPlacingMessage) {
            UpdatePlacementPose();
            UpdatePlacementIndicator();
            if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
                var ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.transform.IsChildOf(placementIndicator.transform)) { 
                    PlaceObject();
                    togglePlaceMessage(false);
                    placementIndicator.SetActive(false);
                }
            }
        }
        currLocation.text = DirectionSense.currPlayerPos.ToString();
    }
    private void placeAllMsgWithinRadius() {
        List<Message> msgList = App.GetWithinRadius(DirectionSense.playerOrigin, 100f);
        foreach (Message msg in msgList) {
            MessageDot messageDot = Instantiate(objectToPlace, DirectionSense.realToUnity(msg.location), placementPose.rotation);
            messageDot.setMessage(msg);
        }
    }

    public void placeMessage(Message m) {
        pageSwipe.forceSwipe(SwipeDirection.DOWN);
        togglePlaceMessage(true);
        messageToPlace = m;
    }
    public void togglePlaceMessage(bool isPlacing) {
        isPlacingMessage = isPlacing;
        placementUI.SetActive(isPlacing);
        pageSwipe.panelLock = isPlacing;
        if (!isPlacing) pageSwipe.resetToNormFromAR();
    }

    private void PlaceObject() {
        MessageDot messageDot = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        messageToPlace.location = DirectionSense.unityToReal(placementPose.position);
        messageDot.setMessage(messageToPlace);
        App.Post(messageToPlace);
    }

    private void UpdatePlacementIndicator() {
        if(placementPoseIsValid) {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else {
            placementIndicator.SetActive(false);
        }
    }
    private void UpdatePlacementPose() {
        Vector3 screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        arRayManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid) {
            placementPose = hits[0].pose;
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}
