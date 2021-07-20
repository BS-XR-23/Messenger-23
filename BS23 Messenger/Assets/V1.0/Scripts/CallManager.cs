using agora_rtm;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CallManager : MonoBehaviour
{
    static CallEngine app = null;
    public bool isInCall = false;
    public RawImage videoImage;


    private string currentChannelName;


    private void Start()
    {
        currentChannelName = "";
    }

    public void LeaveCallChannel()
    {
        app.leave();
        isInCall = false;
        currentChannelName = "";
    }

   

    public void StartCall(bool isVideo, string receiverID,string channelName,string rtcToken,uint uID)
    {
        
        //UI
        if (app == null)
        {
            app = new CallEngine();
            app.loadEngine();
        }
        if(!isVideo)
            app.join(channelName, rtcToken, isVideo,uID);
        else
            app.join(channelName, rtcToken, isVideo,uID,videoImage);
        ChatUIManager.instance.StartCallUI(receiverID, isVideo);
        isInCall = true;
        InvitePeer(receiverID, channelName);


    }

    IEnumerator ReceiveCallAfterToken(bool isVideo, string receiverID, string channelName)
    {
        yield return null;
        UnityWebRequest request = UnityWebRequest.Get("https://agora-token-demo.herokuapp.com/rtc-uid-token/?uid=2" + "&channelName=" + channelName + "&username=" +   MessengerManager.instance.loggedInUserID);
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
            StartCall(isVideo, receiverID, channelName, receivedToken.token,2);


        }
    }

    public void ReceiveCall(bool isVideo, string receiverID, string channelName)
    {
        StartCoroutine(ReceiveCallAfterToken(isVideo, receiverID, channelName));
    }

    public void InvitePeer(string receipientID, string channelName)
    {
        string peerUid = receipientID;
        if (string.IsNullOrEmpty(peerUid))
        {
            return;
        }
        // Creates LocalInvitation
        LocalInvitation invitation = MessengerManager.instance.chatSystem.callManager.CreateLocalCallInvitation(peerUid);
        invitation.SetChannelId(channelName);
        // Sends call invitation
        int rc = MessengerManager.instance.chatSystem.callManager.SendLocalInvitation(invitation);
        Debug.Log("Send invitation to " + peerUid + " rc = " + rc);
    }

    private void OnApplicationQuit()
    {
        app.unloadEngine();
    }
}
