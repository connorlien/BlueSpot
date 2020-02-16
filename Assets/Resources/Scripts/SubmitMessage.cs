using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubmitMessage : MonoBehaviour {
    public TMP_InputField username;
    public TMP_InputField message;
    public ARTapToPlaceObject arTap;

    public void SendMessage() {
        Message result = new Message(new Mapbox.Utils.Vector2d(0, 0), username.text, message.text, System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        print(result);
        arTap.placeMessage(result);
        resetText();
    }
    public void resetText() {
        message.text = "";
    }
}
