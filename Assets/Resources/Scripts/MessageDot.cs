using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using TMPro;

public class MessageDot : MonoBehaviour {
    public GameObject dot;
    public GameObject msgBoard;

    public TextMeshPro username;
    public TextMeshPro message;
    public TextMeshPro location;
    public TextMeshPro timestamp;

    private Transform trans;
    private Message m;
    private bool msgShown = false;
    void Awake() {
        trans = transform;
    }
    public void setMessage(Message msg) {
        m = msg;
        username.text = "@"+m.username;
        message.text = m.message;
        location.text = m.location.ToString();
        timestamp.text = m.timestamp;
    }
    void Update() {
        rotateTowardCamera();
        detectTouch();
    }
    private void rotateTowardCamera() {
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraBearing = new Vector3(cameraForward.x, cameraForward.y, cameraForward.z).normalized;
        trans.rotation = Quaternion.LookRotation(cameraBearing);
    }
    private void detectTouch() {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !ARTapToPlaceObject.isPlacingMessage) {
            var ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.transform.IsChildOf(trans)) toggleView();
        }
    }
    private void toggleView() {
        msgShown = !msgShown;
        dot.SetActive(!msgShown);
        msgBoard.SetActive(msgShown);
    }
}
