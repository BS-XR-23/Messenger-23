using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using agora_rtm;

public class ChatUIManager : MonoBehaviour
{
    public static ChatUIManager instance;
    public TextMeshProUGUI profileName;
    public RectTransform conversationContentPanel;
    public RectTransform chatListContentPanel;
    public GameObject senderPrefab;
    public GameObject receiverPrefab;
    public TMP_InputField chatTextInput;
    public GameObject chatBubblePrefab;
    public GameObject addFriendsPanel;
    //For Add Friends
    public TMP_InputField friendsNameInputField;



    //For Calling UI
    public GameObject callInterface;
    public TextMeshProUGUI receiverName;
    public RawImage videoCallSurface;
    public GameObject audioCallSurface;


    //For Incoming Call UI
    public GameObject incomingCallBase;
    public TextMeshProUGUI callerName;
    public Button receiveCall;
    public Button receiveCallVideo;
    public Button refuseCall;


    //Chat
    public UnityEvent SendMessageButtonEvent;
    //Outgoing
    public UnityEvent audioCallButtonEvent;
    public UnityEvent videoCallButtonEvent;
    public UnityEvent leaveCallButtonEvent;

    //Incoming
    public UnityEvent acceptCallButtonEvent;
    public UnityEvent refuseCallButtonEvent;
    public UnityEvent acceptVideoCallButtonEvent;
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;    
    }


    void Start()
    {
        
    }

    public void SetUserName(string name)
    {
        profileName.text = name;
    }

    public void ClearInputField()
    {
        chatTextInput.text = "";
    }

    public void OnSendButtonClick()
    {
        SendMessageButtonEvent.Invoke();
    }

    //Calling Someone

    public void OnAudioCallButtonClicked()
    {
        audioCallButtonEvent.Invoke();
    }

    public void OnVideoCallButtonClicked()
    {
        videoCallButtonEvent.Invoke();
    }

    public void OnCallEndButtonClicked()
    {
        leaveCallButtonEvent.Invoke();
        callInterface.SetActive(false);
    }


    //On Getting Call

    public void OnCallRefused()
    {
        refuseCallButtonEvent?.Invoke();
        ShowIncomingCallUI(false);
    }

    public void OnCallAccept()
    {
        acceptCallButtonEvent.Invoke();
        ShowIncomingCallUI(false);
    }

    public void OnVideoCallAccept()
    {
        acceptVideoCallButtonEvent.Invoke();
        ShowIncomingCallUI(false);
    }

    public void AddMessage(ChatMessage message)
    {
        GameObject prefabToUse = (message.messageType == ChatMessage.MessageType.UserMessage) ? senderPrefab : receiverPrefab;
        prefabToUse.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = message.text;
        GameObject.Instantiate(prefabToUse, conversationContentPanel);
        Canvas.ForceUpdateCanvases();
    }

    public void AddFriends()
    {
        CreateNewConversation(friendsNameInputField.text);
        addFriendsPanel.SetActive(false);
    }

    public void CreateNewConversation(string recepientID, ChatMessage msg = null)
    {
        GameObject g = GameObject.Instantiate(chatBubblePrefab, chatListContentPanel);
        g.GetComponent<ConversationController>().recepientID = recepientID;
        if(msg != null)
        g.GetComponent<ConversationController>().OnMessageReceived(msg);
    }

    public void StartCallUI(string reciepientName, bool isVideoCall)
    {
        receiverName.text = "Calling " + reciepientName;
        videoCallSurface.gameObject.SetActive(isVideoCall);
        audioCallSurface.SetActive(!isVideoCall);
        callInterface.SetActive(true);
    }

    public void ShowIncomingCallUI(bool b)
    {
        incomingCallBase.SetActive(b);
        
    }


    public void SetupIncomingCall(RemoteInvitation remoteInvitation)
    {
        callerName.text = remoteInvitation.GetCallerId();
        acceptCallButtonEvent.RemoveAllListeners();
        acceptCallButtonEvent.AddListener(()=> {
            MessengerManager.instance.callManager.ReceiveCall(false, remoteInvitation.GetCallerId(),remoteInvitation.GetChannelId());
        });
        acceptVideoCallButtonEvent.RemoveAllListeners();
        acceptVideoCallButtonEvent.AddListener(() => {
            MessengerManager.instance.callManager.ReceiveCall(true, remoteInvitation.GetCallerId(), remoteInvitation.GetChannelId());
        });

        
        

        ShowIncomingCallUI(true);
    }

    public void OnCallReceived()
    {

    }

    public void OnCallConnected(string reciepientName)
    {
        receiverName.text = reciepientName;
    }


    public void ClearAllMessage()
    {
        conversationContentPanel.DestoryAllChildImmediate();
    }

}
