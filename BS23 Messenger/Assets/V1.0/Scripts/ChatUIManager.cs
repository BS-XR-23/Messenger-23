using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using agora_rtm;
using Newtonsoft.Json;
using System;

public class ChatUIManager : MonoBehaviour
{

    public GameObject comScroll;
    public GameObject popUpwindow;
    
    public static ChatUIManager instance;
    public TextMeshProUGUI profileName;
    public TextMeshProUGUI MyID;
    public RectTransform conversationContentPanel;
    public RectTransform chatListContentPanel;
    public GameObject senderPrefab;
    public GameObject receiverPrefab;
    public TMP_InputField chatTextInput;
    public GameObject chatBubblePrefab;
    public GameObject chatBubblePrefab_Mobile;
    public GameObject addFriendsPanel;
    //For Add Friends & Chat
    public TMP_InputField friendsNameInputField;
    public TMP_InputField sendMessangeInputField;
    public TMP_InputField Myprofile;



    //For Calling UI
    public GameObject callInterface;
    public TextMeshProUGUI receiverName;
    public GameObject videoCallSurface;
    public GameObject audioCallSurface;
    public GameObject selfCameraView;


    //Call notifications UI
    public GameObject RecieveVideoCallNotifications;
    public GameObject RecieveAudioCallNotifications;

    //For Incoming Call UI
    public GameObject incomingCallBase;
    public TextMeshProUGUI callerName;
    


    //Chat Button Event
    public UnityEvent SendMessageButtonEvent;

    //Outgoing Call Button Events
    public UnityEvent audioCallButtonEvent;
    public UnityEvent videoCallButtonEvent;
    public UnityEvent leaveCallButtonEvent;

    //Incoming Call Button Events
    public UnityEvent acceptCallButtonEvent;
    public UnityEvent acceptVideoCallButtonEvent;
    public UnityEvent refuseCallButtonEvent;

    //List
    public List<string> ReceiepentList;

    //Stack
    public Stack<GameObject> Panels;


    //Login
    public GameObject loginPanel;

    //This will come handy if you want the user to log in from a different interface
    public bool useInternalLogin = false;

    //Mobile Specific UI
    public GameObject homePage;
    public GameObject messagePage;
    public GameObject ExitPanel;


    public GameObject DateText;
    

    private void Awake()
    {

        instance = this;    
    }

    public void Update()
    {

       // Debug.Log("Real time and date " + DateTime.Now);

        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
             {
                if(messagePage)
                {
                    homePage.SetActive(true);
                    messagePage.SetActive(false);
                }
                if(loginPanel)
                {
                    ExitPanel.SetActive(true);
                }

               
            }
        }
    }


  
    void Start()
    {

        loginPanel.SetActive(useInternalLogin);
    }

    public void OnLoginButtonClicked()
    {
        string username =  loginPanel.GetComponentInChildren<TMP_InputField>().text;
        if (username != "")
        {
            MessengerManager.instance.LoginToMessenger(username);
            loginPanel.SetActive(false);
            Debug.Log(MyID.text);
            MyID.text = Myprofile.text;
            
        }
        
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

        sendMessangeInputField.text.Trim();
        SendMessageButtonEvent.Invoke();
        sendMessangeInputField.OnSelect(null);
      
        //GameObject.Instantiate(DateText, conversationContentPanel);
        //DateText.GetComponent<TextMeshProUGUI>().text = DateTime.Now.ToString();
        Debug.Log("time " + DateTime.Now);
       
        
    }
        

    //Calling Someone

    public void OnAudioCallButtonClicked()
    {
        audioCallButtonEvent.Invoke();
        RecieveVideoCallNotifications.SetActive(false);
        RecieveAudioCallNotifications.SetActive(true);
    }

    public void OnVideoCallButtonClicked()
    {
        videoCallButtonEvent.Invoke();
        RecieveAudioCallNotifications.SetActive(false);
        RecieveVideoCallNotifications.SetActive(true);
    }

    public void OnCallEndButtonClicked()
    {
        leaveCallButtonEvent.Invoke();
        callInterface.SetActive(false);
    }

    public void OnExitSelected()
    {
        Application.Quit();
    }
    public void OnNoSelected()
    {
        ExitPanel.SetActive(false);
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


    //This adds a message to the ui based on who is the sender and who is the receiver.
    //public void AddMessage(Conversations message)
    //{
    //    GameObject prefabToUse = (message.chats.messageType == ChatMessage.MessageType.UserMessage) ? senderPrefab : receiverPrefab;

    //    //prefabToUse.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = DateTime.Now.ToString();
    //    prefabToUse.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = message.chats.text;
    //    message.UpdatedDateTime();
        
    //    GameObject.Instantiate(prefabToUse, conversationContentPanel);
    //    StartCoroutine(ForceScrollDown(comScroll.GetComponent<ScrollRect>(),0,0,0.5f));
        
    //    Canvas.ForceUpdateCanvases();
    //}


    public void AddMessage(ChatMessage message)
    {
        GameObject prefabToUse = (message.messageType == ChatMessage.MessageType.UserMessage) ? senderPrefab : receiverPrefab;

        
        //prefab
      //  prefabToUse.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = DateTime.Now.ToString();
        prefabToUse.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = message.text;


        //loading data to model class
        //Conversations convo = new Conversations();
        //message.text = convo.Messages;
        //convo.DateText = DateTime.Now.ToString();
        //Debug.Log("Messages " + message.text);


        GameObject.Instantiate(prefabToUse, conversationContentPanel);
        StartCoroutine(ForceScrollDown(comScroll.GetComponent<ScrollRect>(), 0, 0, 0.5f));

        Canvas.ForceUpdateCanvases();
    }


    IEnumerator ForceScrollDown(ScrollRect scrollRect,float startPosition, float endPosition,float duration)
    {
        // Wait for end of frame AND force update all canvases before setting to bottom.
        yield return new WaitForSeconds(0.5f);
        float t0 = 0.0f;
        while(t0 < 1.0f)
        {
            t0 += Time.deltaTime / duration;
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(startPosition, endPosition, t0);
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(startPosition, endPosition, t0);
            yield return null;
        }
    }

    // Just Creates A Brand new conversation based on given username.
    public void AddFriends()
    {

        

        if (friendsNameInputField.text == "")
        {
            Debug.Log(ReceiepentList.ToString());
            return;
        }

        if (ReceiepentList.Contains(friendsNameInputField.text))
        {
            
            popUpwindow.SetActive(true);
            return;
        }

        else
        {
            CreateNewConversation(friendsNameInputField.text);
            addFriendsPanel.SetActive(false);
        }
        
    }

    public void OpenFriendsPanelWindow()
    {

        sendMessangeInputField.enabled = false;

        addFriendsPanel.SetActive(true);
    }
   

    public void CloseAddFriendWindow()
    {
        addFriendsPanel.SetActive(false);
        sendMessangeInputField.enabled = true;
    }

    public void ClosePopUpWindow()
    {
        popUpwindow.SetActive(false);
        addFriendsPanel.SetActive(false);
    }
    public void openPopUpWindow()
    {
        popUpwindow.SetActive(true);
    }

    //Creates A New Conversation. A chat message object can be added to create a new conversation from message received
    public void CreateNewConversation(string recepientID, ChatMessage msg = null)
    {
#if UNITY_ANDROID || UNITY_IOS
        GameObject g = GameObject.Instantiate(chatBubblePrefab_Mobile, chatListContentPanel);
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR_WIN
        GameObject g = GameObject.Instantiate(chatBubblePrefab, chatListContentPanel);
#endif
        g.GetComponent<ConversationController>().recepientID = recepientID;
        ReceiepentList.Add(g.GetComponent<ConversationController>().recepientID);



        if (msg != null)
            g.GetComponent<ConversationController>().OnMessageReceived(msg);
    }


    // Shows The Calling User Interface
    public void StartCallUI(string reciepientName, bool isVideoCall)
    {
        receiverName.text = "Calling " + reciepientName;
        videoCallSurface.gameObject.SetActive(isVideoCall);
        audioCallSurface.SetActive(!isVideoCall);
        callInterface.SetActive(true);
    }

    
    // Loads messages from local storage
    public void LoadSavedConversation()
    {
        string path = Application.persistentDataPath + "/messengerData/" + MessengerManager.instance.loggedInUserID+ "/";
        if (!Directory.Exists(path)) return;
        DirectoryInfo dInfo = new DirectoryInfo(path);

        FileInfo[] allConvoFiles = dInfo.GetFiles("*.json");
        foreach (var file in allConvoFiles)
        {
#if UNITY_ANDROID || UNITY_IOS 
            GameObject g = GameObject.Instantiate(chatBubblePrefab_Mobile, chatListContentPanel);
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR_WIN
            GameObject g = GameObject.Instantiate(chatBubblePrefab, chatListContentPanel);
#endif

            string recepientID = Path.GetFileNameWithoutExtension(file.FullName);
            g.GetComponent<ConversationController>().recepientID = recepientID;
            ReceiepentList.Add(g.GetComponent<ConversationController>().recepientID);
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
        

        
    

    // Assign actions to incoming call UI buttons and show the UI.
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

    //Mobile UI Code
    public void ShowHomeScreen()
    {
        homePage.SetActive(true);
        messagePage.SetActive(false);
        
    }

    public void showMessengerScreen()
    {
        homePage.SetActive(false);
        messagePage.SetActive(true);
    }

}
