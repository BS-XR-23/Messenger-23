using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class MessengerManager : MonoBehaviour
{
    public static MessengerManager instance;

#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList();
#endif

    public ChatSystem chatSystem;
    public CallManager callManager;
    public string loggedInUserID;
    public string RtcToken;
    public GameObject messengerRoot;

    [SerializeField]
    private string AppID;


    private void Awake()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        permissionList.Add(Permission.Microphone);
        permissionList.Add(Permission.Camera);
#endif
        instance = this;
        chatSystem = GetComponent<ChatSystem>();
        callManager = GetComponent<CallManager>();
    }

    private void CheckPermissions()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        foreach (string permission in permissionList)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {
                Permission.RequestUserPermission(permission);
            }
        }
#endif
    }

    void Update()
    {
        CheckPermissions();
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
