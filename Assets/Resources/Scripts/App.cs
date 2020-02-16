using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;

public class App : MonoBehaviour {
    /** Stores all messages */
    private static Dictionary<int, Message> messages;

    /** Instance of the singleton this class is in */
    public static App instance;

    private float timer = 0f;

    /** Runs when application starts */
    void Awake() {
        instance = this;
        UpdateUserLocation();
    }

    void Update() {
        timer += Time.deltaTime;
        if (timer > 5f) {
            LoadAll();
            UpdateUserLocation();
            timer = 0;
        }
    }
    /** PROGRAM START */


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnAppStart() {
        messages = new Dictionary<int, Message>();
        LoadAll();
        UpdateUserLocation();
    }


    /** SERVER METHODS */


    /** Posts a Message M to path PATH */
    public static void Post(Message m) {
        /*if (ProfanityDetector.ContainsProfanity(m.message)) {
            Debug.Log("Post failed: profanity detected");
            return;
        }*/
        int path = GetUnusedPath();
        ServerManager.PostMessage(m, "coryca" + path, () => 
        { 
            Debug.Log("Post to path " + path + " successful");
            messages.Add(path, m);
        });
    }

    /** Gets the message at path PATH */
    public static void Get(int path) {
        ServerManager.GetMessage("coryca" + path, info =>
        {
            Message m = new Message(info.location, info.username, info.message, info.timestamp);
            messages.Add(path, m);
            Debug.Log("Get from path " + path + " successful:\n" + m);
        });
    }

    /** Deletes the message at path PATH */
    public static void Delete(int path) {
        ServerManager.DeleteMessage("coryca" + path, () => 
        { 
            Debug.Log("Deleting path " + path + " successful"); 
        });
    }

    /** Load all messages in the server */
    public static void LoadAll() {
        ServerManager.GetAllMessages(data =>
        {
            Debug.Log("Load All successful: " + data.Count + " messages obtained.");
            foreach (var info in data) {
                Message m = info.Value;
                int path = int.Parse(info.Key.Substring(6, info.Key.Length - 6));
                messages.Add(path, m);
                // Debug.Log(m);
            }
        });
    }

    /** Returns the smallest path number that has not been used */
    private static int GetUnusedPath() {
        int path = 0;
        while (messages.ContainsKey(path)) {
            path += 1;
        }
        return path;
    }


    /** LOCATION METHODS */


    /** Returns a List of Messages within RADIUS distance of POSITION */
    public static List<Message> GetWithinRadius(Vector2d position, float radius) {
        List<Message> result = new List<Message>();
        foreach (KeyValuePair<int, Message> kvp in messages) {
            Message current = kvp.Value;
            if ((current.location - position).magnitude < radius) {
                result.Add(current);
            }
        }
        return result;
    }

    /** Returns a List of Messages that USER has posted */
    public static List<Message> GetUserPosts(string user) {
        List<Message> result = new List<Message>();
        foreach (KeyValuePair<int, Message> kvp in messages) {
            Message current = kvp.Value;
            if (current.username == user) {
                result.Add(current);
            }
        }
        return result;
    }

    /** Gets the location of the user */
    public static void UpdateUserLocation() {
        if (instance) {
            instance.StartCoroutine(LocationHelper());
        }
    }

    /** A helper routine to determine user location. 
        Sets location to (latitude, longitude, altitude) */
    private static IEnumerator LocationHelper() {
        if (!Input.location.isEnabledByUser) {
            Debug.Log("Location is not enabled");
            if (!DirectionSense.playerOriginIsSet) {
                DirectionSense.playerOrigin = new Vector2d(-1, -1);
                DirectionSense.playerOriginIsSet = true;
            }
            DirectionSense.currPlayerPos = new Vector2d(-1, -1);
            yield break;
        }
        Input.location.Start(0.5f, 0.5f);
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if (maxWait < 1) {
            Debug.Log("Timed out");
            if (!DirectionSense.playerOriginIsSet) {
                DirectionSense.playerOrigin = new Vector2d(-2, -2);
                DirectionSense.playerOriginIsSet = true;
            }
            DirectionSense.currPlayerPos = new Vector2d(-2, -2);
            yield break;
        }
        if (Input.location.status == LocationServiceStatus.Failed) {
            Debug.Log("Unable to determine device location");
            if (!DirectionSense.playerOriginIsSet) {
                DirectionSense.playerOrigin = new Vector2d(-3, -3);
                DirectionSense.playerOriginIsSet = true;
            }
            DirectionSense.currPlayerPos = new Vector2d(-3, -3);
            yield break;
        }
        else {
            LocationInfo info = Input.location.lastData;
            Vector3 temp = new Vector3(info.latitude, info.altitude, info.longitude);
            if (!DirectionSense.playerOriginIsSet) {
                DirectionSense.playerOrigin = temp.ToVector2d();
                DirectionSense.playerOriginIsSet = true;
            }
            DirectionSense.currPlayerPos = temp.ToVector2d();
        }
        Input.location.Stop();
    }
}