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


    public readonly string RtmTokenBaseUrl = "https://agora-token-demo.herokuapp.com/token/";
    public readonly string RTCBaseUrl = "https://agora-token-demo.herokuapp.com/rtc-uid-token/";



    [SerializeField]
    private string AppID;

    public bool IsInCall;
    private void Awake()
    {
        instance = this;
        chatSystem = GetComponent<ChatSystem>();
        callManager = GetComponent<CallManager>();
    }


    public string GetAppID()
    {
        return AppID;
    }


    public void GenerateUI(List<ChatMessage> chats)
    {
        //UI Generation
    }

    

    
}
