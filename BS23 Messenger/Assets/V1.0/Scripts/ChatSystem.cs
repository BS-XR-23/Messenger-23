using System.Collections;
using UnityEngine;
using TMPro;
using agora_rtm;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ChatSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]private TMP_InputField _usernameText;
    [SerializeField] private GameObject _loginPanel;
    private Transform _chatBubblesRoot=> ChatUIManager.instance.chatListContentPanel;


    public RtmClient RtmClient;
    public agora_rtm.SendMessageOptions MessageOptions = new agora_rtm.SendMessageOptions()
    {
        enableOfflineMessaging = true,
        enableHistoricalMessaging = true
    };


    private RtmChannel _channel;
    public RtmCallManager CallManager { get; set; }

    public RtmClientEventHandler ClientEventHandler;
    private string _appId;
    private string _token;



    private void Awake()
    {
        _token = "";
        _appId = MessengerManager.instance.GetAppID();
        ClientEventHandler = new RtmClientEventHandler();



        RtmClient = new RtmClient(_appId, ClientEventHandler);
#if UNITY_EDITOR
        RtmClient.SetLogFile("./rtm_log.txt");
#endif

        //For Chat Listeners
        
        ClientEventHandler.OnLoginSuccess = OnClientLoginSuccessHandler;
        ClientEventHandler.OnLoginFailure = OnClientLoginFailureHandler;
        ClientEventHandler.OnConnectionStateChanged = OnConnectionStateChangedHandler;
        ClientEventHandler.OnMessageReceivedFromPeer = OnMessageReceivedFromPeerHandler;

        RtmCallEventHandler callEventHandler = new RtmCallEventHandler
        {
            OnRemoteInvitationReceived = OnRemoteInvitationReceivedHandler
        };

        CallManager = RtmClient.GetRtmCallManager(callEventHandler);
    }



    void OnApplicationQuit()
    {
        if (_channel != null)
        {
            _channel.Dispose();
            _channel = null;
        }
        if (RtmClient != null)
        {
            RtmClient.Dispose();
            RtmClient = null;
        }
    }

    public void OnSignInButtonClicked()
    {
        StartCoroutine(GetTokenFromServer());
    }


    private void Login()
    {
        string username = _usernameText.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(_appId))
        {
            Debug.LogError("We need a username and appId to login");
            return;
        }

        RtmClient.Login(_token, username);
        MessengerManager.instance.loggedInUserID = username;
        _loginPanel.SetActive(false);
    }


    void OnMessageReceivedFromPeerHandler(int id, string peerId, TextMessage message)
    {
        Debug.Log("client OnMessageReceivedFromPeer id = " + id + ", from user:" + peerId + " text:" + message.GetText());
        ChatMessage recMessage = new ChatMessage
        {
            text = message.GetText(),
            messageType = ChatMessage.MessageType.ReceiverMessage,
            username = peerId
        };
        ConversationController[] activeChats = _chatBubblesRoot.GetComponentsInChildren<ConversationController>();
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
        UnityWebRequest request = UnityWebRequest.Get($"{MessengerManager.instance.RtmTokenBaseUrl}?username={_usernameText.text}&channelName={_usernameText.text}");
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            Token receivedToken = JsonConvert.DeserializeObject<Token>(jsonResponse);
            _token = receivedToken.rtmToken;
            MessengerManager.instance.RtcToken = receivedToken.rtcToken;
            Login();
            StartCoroutine(GetRTCTokenFromServer());

        }
    }

    IEnumerator GetRTCTokenFromServer()
    {
        yield return null;
        UnityWebRequest request = UnityWebRequest.Get($"{MessengerManager.instance.RTCBaseUrl}?uid=1&channelName={MessengerManager.instance.loggedInUserID}");
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result!=UnityWebRequest.Result.Success)
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
        RtmClient.Logout();
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
        string msg = $"connection state changed id:{id} state:{state} reason:{reason}";
        Debug.Log(msg);
        //messageDisplay.AddTextToDisplay(msg, Message.MessageType.Info);
    }


    void OnRemoteInvitationReceivedHandler(RemoteInvitation remoteInvitation)
    {
        if (MessengerManager.instance.IsInCall)
            return;
        string msg =
            $"OnRemoteInvitationReceived channel:{remoteInvitation.GetChannelId()}, callee:{remoteInvitation.GetCallerId()}";
        Debug.Log(msg);
        ChatUIManager.instance.SetupIncomingCall(remoteInvitation);

    }

}
