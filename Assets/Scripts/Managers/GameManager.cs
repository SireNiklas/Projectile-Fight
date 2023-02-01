using Sir.Core.Singletons;
using System.Collections;
using UnityEngine;

public class GameManager : NetworkSingleton<GameManager>
{

    public bool connected;
    public bool inGame;
    public bool isHost;
    public ulong myClientId;

    //[Header("Message System")]
    //[SerializeField] private int maxMessages = 20;
    //private List<Message> messageList = new List<Message>();

    //public class Message
    //{
    //    public string text;
    //}

    //public void SendMessageToChat(string _text, ulong _fromwho, bool _server)
    //{
    //    if (messageList.Count >= maxMessages)
    //    {
    //        Destroy(messageList[0])
    //    }
    //}

    public void HostCreated()
    {

    }

    public void ConnectedAsClient()
    {

    }

    public void Disconnected()
    {
        
    }

    public void Quit()
    {
        Application.Quit();
    }
}
