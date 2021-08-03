using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using agora_rtm;
using Newtonsoft.Json;

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

    //Login
    public GameObject loginPanel;
    public bool useInternalLogin = false;
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;    
    }


    void Start()
    {
        loginPanel.SetActive(useInternalLogin);
    }

    public void OnLoginButtonClicked()
    {
        string username =  loginPanel.GetComponentInChildren<TMP_InputField>().text;
        MessengerManager.instance.LoginToMessenger(username);
        loginPanel.SetActive(false);
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

    public void LoadSavedConversation()
    {
        string path = Application.persistentDataPath + "/messengerData/" + MessengerManager.instance.loggedInUserID+ "/";
        if (!Directory.Exists(path)) return;
        DirectoryInfo dInfo = new DirectoryInfo(path);

        FileInfo[] allConvoFiles = dInfo.GetFiles("*.json");
        foreach (var file in allConvoFiles)
        {
            GameObject g = GameObject.Instantiate(chatBubblePrefab, chatListContentPanel);
            string recepientID = Path.GetFileNameWithoutExtension(file.FullName);
            g.GetComponent<ConversationController>().recepientID = recepientID;
            string jsonData = File.ReadAllText(file.FullName);
            List<ChatMessage> oldMessages = JsonConvert.DeserializeObject<List<ChatMessage>>(jsonData);
            g.GetComponent<ConversationController>().chats = oldMessages;

        }
    }

    public void ShowIncomingCallUI(bool show)
    {
        AudioSource ringtone = incomingCallBase.GetComponent<AudioSource>();
        incomingCallBase.SetActive(show);
        ringtone.Stop();

        if (show) ringtone.Play();
        
        
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
