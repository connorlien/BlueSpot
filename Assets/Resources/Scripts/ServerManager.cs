using System;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;
using Proyecto26;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;

public static class ServerManager {
    private static readonly string url = "https://treehacks-c2aec.firebaseio.com/";
    private static fsSerializer serializer = new fsSerializer();

    public delegate void GetCallback(Message m);
    public delegate void DeleteCallback();
    public delegate void PostCallback();
    public delegate void GetAllCallback(Dictionary<string, Message> messages);

    public static void GetMessage(string path, GetCallback callback) {
        RestClient.Get<Message>($"{url}messages/{path}.json").Then(message => {
            Debug.Log("GetMessage() called");
            callback(message);
        });
    }

    public static void PostMessage(Message m, string path, PostCallback callback) {
        RestClient.Put<Message>($"{url}messages/{path}.json", m).Then(response => {
            Debug.Log("PostMessage() called");
            callback();
        });
    }

    public static void DeleteMessage(string path, DeleteCallback callback) {
        RestClient.Delete($"{url}messages/{path}.json").Then(message => {
            Debug.Log("DeleteMessage() called");
            callback();
        });
    }

    public static void GetAllMessages(GetAllCallback callback) {
        RestClient.Get($"{url}messages.json").Then(response => {
            Debug.Log("GetAllMessages() called");
            var responseJson = response.Text;
            var data = fsJsonParser.Parse(responseJson);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(Dictionary<string, Message>), ref deserialized).AssertSuccess();
            var messages = deserialized as Dictionary<string, Message>;
            callback(messages);
        });
    }
} 

[Serializable]
public class Message 
{
    public Vector2d location;
    public string username;
    public string message;
    public string timestamp;

    public Message(Vector2d loc, string userid, string mess, string ti) {
        location = loc;
        username = userid;
        message = mess;
        timestamp = ti;
    }

    public override string ToString() {
        return username + " at " + location + ": " + message;
    }
}