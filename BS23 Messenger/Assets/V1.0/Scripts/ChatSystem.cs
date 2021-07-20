using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using agora_rtm;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ChatSystem : MonoBehaviour
{
    public TMP_InputField usernameText;
    public GameObject LoginPanel;



    public RtmClient rtmClient = null;
   
    public agora_rtm.SendMessageOptions _MessageOptions = new agora_rtm.SendMessageOptions()
    {
        enableOfflineMessaging = true,
        enableHistoricalMessaging = true
    };


    private RtmChannel channel;
    [HideInInspector]
    public RtmCallManager callManager;

    public RtmClientEventHandler clientEventHandler;
    private RtmChannelEventHandler channelEventHandler;
    private RtmCallEventHandler callEventHandler;
    private string appId;
    private string token;
    private Transform chatBubblesRoot;


    private void Awake()
    {
        token = "";
        appId = MessengerManager.instance.GetAppID();
        clientEventHandler = new RtmClientEventHandler();
        channelEventHandler = new RtmChannelEventHandler();
        callEventHandler = new RtmCallEventHandler();

        rtmClient = new RtmClient(appId, clientEventHandler);
#if UNITY_EDITOR
        rtmClient.SetLogFile("./rtm_log.txt");
#endif

        //For Chat Listeners
        
        clientEventHandler.OnLoginSuccess = OnClientLoginSuccessHandler;
        clientEventHandler.OnLoginFailure = OnClientLoginFailureHandler;
        clientEventHandler.OnConnectionStateChanged = OnConnectionStateChangedHandler;
        clientEventHandler.OnMessageReceivedFromPeer = OnMessageReceivedFromPeerHandler;


        callEventHandler.OnRemoteInvitationReceived = OnRemoteInvitationReceivedHandler;

        callManager = rtmClient.GetRtmCallManager(callEventHandler);
    }

    private void Start()
    {
        chatBubblesRoot = ChatUIManager.instance.chatListContentPanel;
    }


    void OnApplicationQuit()
    {
        if (channel != null)
        {
            channel.Dispose();
            channel = null;
        }
        if (rtmClient != null)
        {
            rtmClient.Dispose();
            rtmClient = null;
        }
    }

    public void OnSignInButtonClicked()
    {
        StartCoroutine(GetTokenFromServer());
    }


    private void Login()
    {
        string username = usernameText.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(appId))
        {
            Debug.LogError("We need a username and appId to login");
            return;
        }

        rtmClient.Login(token, username);
        MessengerManager.instance.loggedInUserID = username;
        LoginPanel.SetActive(false);
    }


    void OnMessageReceivedFromPeerHandler(int id, string peerId, TextMessage message)
    {
        Debug.Log("client OnMessageReceivedFromPeer id = " + id + ", from user:" + peerId + " text:" + message.GetText());
        ChatMessage recMessage = new ChatMessage();
        recMessage.text = message.GetText();
        recMessage.messageType = ChatMessage.MessageType.ReceiverMessage;
        recMessage.username = peerId;
        ConversationController[] activeChats = chatBubblesRoot.GetComponentsInChildren<ConversationController>();
        Debug.Log("Has Child = " +  activeChats.Length);
        //Check if this conversation with this peer exist, then add the message to that convo
        if (activeChats.Length>0)
        {
            Debug.Log("Has Content");
            foreach(ConversationController c in activeChats)
            {
                if(c.recepientID == peerId)
                {
                    c.OnMessageReceived(recMessage);
                    return;
                }
                else
                {
                    Debug.Log("Cant Match " + peerId + " with " + c.recepientID);
                }
            }

        }
        //Else Create new conversation
        ChatUIManager.instance.CreateNewConversation(peerId,recMessage);
        
        
        //messageDisplay.AddTextToDisplay(peerId + ": " + message.GetText(), Message.MessageType.PeerMessage);
    }


    IEnumerator GetTokenFromServer()
    {
        yield return null;
        UnityWebRequest request = UnityWebRequest.Get("https://agora-token-demo.herokuapp.com/token/?username=" + usernameText.text + "&channelName=" + usernameText.text);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            Token receivedToken = JsonConvert.DeserializeObject<Token>(jsonResponse);
            token = receivedToken.rtmToken;
            MessengerManager.instance.RtcToken = receivedToken.rtcToken;
            Login();
            StartCoroutine(GetRTCTokenFromServer());

        }
    }

    IEnumerator GetRTCTokenFromServer()
    {
        yield return null;
        UnityWebRequest request = UnityWebRequest.Get("https://agora-token-demo.herokuapp.com/rtc-uid-token/?uid=1" + "&channelName=" + MessengerManager.instance.loggedInUserID);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            RTCToken receivedToken = JsonConvert.DeserializeObject<RTCToken>(jsonResponse);
            MessengerManager.instance.RtcToken = receivedToken.token;


        }
    }

    public void Logout()
    {
        //messageDisplay.AddTextToDisplay(UserName + " logged out of the rtm", Message.MessageType.Info);
        //Add UI Codes
        rtmClient.Logout();
    }


    

    void OnClientLoginSuccessHandler(int id)
    {
        string msg = "client login successful! id = " + id;
        Debug.Log(msg);
        //messageDisplay.AddTextToDisplay(msg, Message.MessageType.Info);
    }


    void OnClientLoginFailureHandler(int id, LOGIN_ERR_CODE errorCode)
    {
        string msg = "client login unsuccessful! id = " + id + " errorCode = " + errorCode;
        Debug.Log(msg);
        //messageDisplay.AddTextToDisplay(msg, Message.MessageType.Error);
    }


    


    void OnConnectionStateChangedHandler(int id, CONNECTION_STATE state, CONNECTION_CHANGE_REASON reason)
    {
        string msg = string.Format("connection state changed id:{0} state:{1} reason:{2}", id, state, reason);
        Debug.Log(msg);
        //messageDisplay.AddTextToDisplay(msg, Message.MessageType.Info);
    }


    void OnRemoteInvitationReceivedHandler(RemoteInvitation remoteInvitation)
    {
        if (MessengerManager.instance.callManager.isInCall)
            return;
        string msg = string.Format("OnRemoteInvitationReceived channel:{0}, callee:{1}", remoteInvitation.GetChannelId(), remoteInvitation.GetCallerId());
        Debug.Log(msg);
        ChatUIManager.instance.SetupIncomingCall(remoteInvitation);

    }

}
