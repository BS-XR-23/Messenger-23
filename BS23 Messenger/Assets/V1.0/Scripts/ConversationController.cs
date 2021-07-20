using agora_rtm;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using agora_gaming_rtc;

public class ConversationController : MonoBehaviour
{
    public TextMeshProUGUI profileName;
    public TextMeshProUGUI lastMessage;
    public GameObject onlineIcon;

    public string recepientID;

    [HideInInspector]
    public List<ChatMessage> chats;



    
    private RtmClient rtmClient = null;
    private agora_rtm.SendMessageOptions _MessageOptions;
    private RtmClientEventHandler clientEventHandler;
    private Toggle thisToggle;
    



    // load agora engine



    private void Awake()
    {
        chats = new List<ChatMessage>();
        GetComponent<BaseToggleModifier>().OnClick.AddListener(() =>
        {
            GenerateConversation();

        });
    }

    


   
    // Start is called before the first frame update
    void Start()
    {
        thisToggle = GetComponent<Toggle>();
        
        
        clientEventHandler = MessengerManager.instance.chatSystem.clientEventHandler;
        clientEventHandler.OnQueryPeersOnlineStatusResult = OnQueryPeersOnlineStatusResultHandler;




        rtmClient = MessengerManager.instance.chatSystem.rtmClient;
        _MessageOptions = MessengerManager.instance.chatSystem._MessageOptions;
        StartCoroutine(CheckOnlineStatus());
        Initialize();
    }


    public void Initialize()
    {
        profileName.text = recepientID;
        if (chats.Any())
            lastMessage.text = chats.Last()?.text;
    }


    public void GenerateConversation()
    {
        ChatUIManager.instance.SetUserName(recepientID);
        LoadMessages();
        ChatUIManager.instance.SendMessageButtonEvent.RemoveAllListeners();
        ChatUIManager.instance.SendMessageButtonEvent.AddListener(() =>
        {
            SendPeerMessage();

        });
        ChatUIManager.instance.audioCallButtonEvent.RemoveAllListeners();
        ChatUIManager.instance.audioCallButtonEvent.AddListener(()=> {
           MessengerManager.instance.callManager.StartCall(false,recepientID,MessengerManager.instance.loggedInUserID, MessengerManager.instance.RtcToken,1);
        });

        ChatUIManager.instance.videoCallButtonEvent.RemoveAllListeners();
        ChatUIManager.instance.videoCallButtonEvent.AddListener(() => {
            MessengerManager.instance.callManager.StartCall(true,recepientID, MessengerManager.instance.loggedInUserID, MessengerManager.instance.RtcToken,1);
        });

        ChatUIManager.instance.leaveCallButtonEvent.RemoveAllListeners();
        ChatUIManager.instance.leaveCallButtonEvent.AddListener(() => {
            MessengerManager.instance.callManager.LeaveCallChannel();
        });

    }

    

    public void LoadMessages()
    {
        ChatUIManager.instance.ClearAllMessage();
        if (chats.Count<=0) return;
        foreach (ChatMessage msg in chats)
        {
            ChatUIManager.instance.AddMessage(msg);
        }
    }



    public void SendPeerMessage()
    {
        string msg = ChatUIManager.instance.chatTextInput.text;
        string peer = recepientID;
        if (string.IsNullOrEmpty(msg)) return;
        ChatMessage message = new ChatMessage();
        message.text = msg;
        message.messageType = ChatMessage.MessageType.UserMessage;
        message.username = MessengerManager.instance.loggedInUserID;

        Debug.Log("Sending Message");

        //string displayMsg = string.Format("{0}->{1}: {2}", UserName, peer, msg);
        //messageDisplay.AddTextToDisplay(displayMsg, Message.MessageType.PlayerMessage);

        rtmClient.SendMessageToPeer(
            peerId: peer,
            message: rtmClient.CreateMessage(msg),
            options: _MessageOptions
       );
        ChatUIManager.instance.ClearInputField();
        chats.Add(message);

        Initialize();
        DoAfterConvoUpdate();


    }

    public void DoAfterConvoUpdate()
    {
        if (!thisToggle.isOn)
            return;
        LoadMessages();
    }

    IEnumerator CheckOnlineStatus()
    {
        QueryPeersOnlineStatus();
        yield return new WaitForSeconds(15);
        StartCoroutine(CheckOnlineStatus());
    }


    public void QueryPeersOnlineStatus()
    {
        long req = 222222;
        rtmClient.QueryPeersOnlineStatus(new string[] { recepientID }, req);
    }


    void OnQueryPeersOnlineStatusResultHandler(int id, long requestId, PeerOnlineStatus[] peersStatus, int peerCount, QUERY_PEERS_ONLINE_STATUS_ERR errorCode)
    {
        if (peersStatus.Length > 0)
        {
            onlineIcon.SetActive(peersStatus[0].onlineState == PEER_ONLINE_STATE.PEER_ONLINE_STATE_ONLINE);
        }
    }
    

    public void OnMessageReceived(ChatMessage recMessage)
    {
        chats.Add(recMessage);
        Initialize();
        DoAfterConvoUpdate();
    }


}

public class ChatMessage
{
    public string text;
    public MessageType messageType;
    public string username;

    public enum MessageType
    {
        Info,
        Error,
        UserMessage,
        ReceiverMessage
    }
}
