using System;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;
using Proyecto26;

public static class ServerManager
{
    private static readonly string url = "https://treehacks-c2aec.firebaseio.com/";
    private static fsSerializer serializer = new fsSerializer();

    public delegate void GetCallback(Message m);
    public delegate void PostCallback();
    public delegate void GetAllCallback(Dictionary<string, Message> messages);

    public static void GetMessage(string path, GetCallback callback) {
        RestClient.Get<Message>($"{url}messages/{path}.json").Then(message => {
            callback(message);
        });
    }

    public static void PostMessage(Message m, string path, PostCallback callback) {
        RestClient.Put<Message>($"{url}messages/{path}.json", m).Then(response => {
            callback();
        });
    }

    public static void GetAllMessages(GetAllCallback callback) {
        RestClient.Get($"{url}messages.json").Then(response => {
            var responseJson = response.Text;
            var data = fsJsonParser.Parse(responseJson);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(Dictionary<string, Message>), ref deserialized);
            var messages = deserialized as Dictionary<string, Message>;
            callback(messages);
        });
    }
}

[Serializable]
public class Message 
{
    public Vector3 location;
    public string username;
    public string message;

    public Message(Vector3 loc, string userid, string mess) {
        location = loc;
        username = userid;
        message = mess;
    }
}