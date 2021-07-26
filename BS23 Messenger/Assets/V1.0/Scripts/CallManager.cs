using agora_rtm;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CallManager : MonoBehaviour
{
    private static CallEngine _app;

    [SerializeField]private RawImage VideoImage;








    public void LeaveCallChannel()
    {
        _app.Leave();
        MessengerManager.instance.IsInCall = false;
    }

   

    public void StartCall(bool isVideo, string receiverId,string channelName,string rtcToken,uint uId)
    {
        
        //UI
        if (_app == null)
        {
            _app = new CallEngine();
            _app.LoadEngine();
        }

        if(!isVideo)
            _app.Join(channelName, rtcToken,uId);
        else
            _app.Join(channelName, rtcToken,uId,VideoImage);


        ChatUIManager.instance.StartCallUI(receiverId, isVideo);
        MessengerManager.instance.IsInCall = true;
        InvitePeer(receiverId, channelName);


    }

    IEnumerator ReceiveCallAfterToken(bool isVideo, string receiverId, string channelName)
    {
        yield return null;
        UnityWebRequest request = UnityWebRequest.Get($"{MessengerManager.instance.RTCBaseUrl}?uid={2}&channelName={channelName}&username={MessengerManager.instance.loggedInUserID}");
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            RTCToken receivedToken = JsonConvert.DeserializeObject<RTCToken>(jsonResponse);
            //todo uid is fixed
            StartCall(isVideo, receiverId, channelName, receivedToken.token,2);


        }
    }

    public void ReceiveCall(bool isVideo, string receiverId, string channelName)
    {
        StartCoroutine(ReceiveCallAfterToken(isVideo, receiverId, channelName));
    }

    private void InvitePeer(string receipientId, string channelName)
    {
        string peerUid = receipientId;
        if (string.IsNullOrEmpty(peerUid))
        {
            return;
        }
        // Creates LocalInvitation
        LocalInvitation invitation = MessengerManager.instance.chatSystem.CallManager.CreateLocalCallInvitation(peerUid);
        invitation.SetChannelId(channelName);
        // Sends call invitation
        int rc = MessengerManager.instance.chatSystem.CallManager.SendLocalInvitation(invitation);
        Debug.Log("Send invitation to " + peerUid + " rc = " + rc);
    }

    private void OnApplicationQuit()
    {
        _app.UnloadEngine();
    }
}
