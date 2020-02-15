using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnAppStart()
    {
        Debug.Log("Firebase Test started");
        var testmessage = new Message(new Vector3(0, 0, 0), "Connor", "Hello World!");
        ServerManager.PostMessage(testmessage, "12", () => 
        { 
            Debug.Log("Post successful"); 
        });
        ServerManager.GetMessage("12", info =>
        {
            Debug.Log($"{info.location}{info.username}{info.message}");
        });
    }
}