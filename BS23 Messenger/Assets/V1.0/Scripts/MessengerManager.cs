using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessengerManager : MonoBehaviour
{
    public static MessengerManager instance;
    public ChatSystem chatSystem;
    public CallManager callManager;
    public string loggedInUserID;
    public string RtcToken;
    public GameObject messengerRoot;

    [SerializeField]
    private string AppID;


    private void Awake()
    {
        instance = this;
        chatSystem = GetComponent<ChatSystem>();
        callManager = GetComponent<CallManager>();
    }

    void Start()
    {
        
    }

    public string GetAppID()
    {
        return AppID;
    }


    public void ShowMessenger(bool show)
    {
        messengerRoot.SetActive(show);
    }

    public void LoginToMessenger(string username)
    {
        StartCoroutine(chatSystem.GetTokenFromServer(username));
    }

    

    
}
