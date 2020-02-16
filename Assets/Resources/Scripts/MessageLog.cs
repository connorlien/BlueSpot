using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageLog : MonoBehaviour
{
    public Image spawnlog;
    public VerticalLayoutGroup spawnarea;
    private List<Message> messages;
    private const string username = "Connor";
    private int framewait = 5;

    // Update is called once per frame
    void Update()
    {
        if (framewait > 0) {
            framewait -= 1;
        }
        else if (framewait == 0) {
            messages = App.GetUserPosts(username);
            SpawnMessages();
            framewait = -1;
        }
    }

    private void SpawnMessages() {
        foreach (Message m in messages) {
            Image i = Instantiate(spawnlog, spawnarea.transform);
            i.GetComponentInChildren<TextMeshProUGUI>().text = m.timestamp + "\t" + m.message;
        }
    }
}
