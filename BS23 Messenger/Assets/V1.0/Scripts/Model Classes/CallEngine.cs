using agora_gaming_rtc;
using UnityEngine;
using UnityEngine.UI;

public class CallEngine
{
    static IRtcEngine mRtcEngine = null;

    bool videoCall;
    RawImage videoView;

    // Load the RTC Engine
    public void loadEngine()
    {
        // start sdk
        Debug.Log("initializeEngine");

        if (mRtcEngine != null)
        {
            Debug.Log("Engine exists. Please unload it first!");
            return;
        }

        // init engine
        mRtcEngine = IRtcEngine.GetEngine(MessengerManager.instance.GetAppID());

        // enable log
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
    }

    // Unload The RTC Engine
    public void unloadEngine()
    {
        Debug.Log("calling unloadEngine");

        // delete
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();  // Place this call in ApplicationQuit
            mRtcEngine = null;
        }
    }




    // Join a channel based on certain information.
    // channel - name of the channel
    // token - Generated Token from the server
    // isVideoCall - Define if its a video call or just audio call
    // uID - A Specific uID so you can track joined users
    // videoSurface - provide a raw image component to show other person's video if it's a video call
    public void join(string channel, string token, bool isVideoCall, uint uID,RawImage videoSurface = null)
    {
        Debug.Log("calling join (channel = " + channel + ")");

        if (mRtcEngine == null)
            return;
        
        // set callbacks (optional)
        mRtcEngine.OnJoinChannelSuccess = onJoinChannelSuccess;
        mRtcEngine.OnUserJoined = onUserJoined;
        mRtcEngine.OnUserOffline = onUserOffline;

        mRtcEngine.OnWarning = (int warn, string msg) =>
        {
            Debug.LogWarningFormat("Warning code:{0} msg:{1}", warn, IRtcEngine.GetErrorDescription(warn));
        };
        mRtcEngine.OnError = HandleError;
        if (isVideoCall)
        {
            videoView = videoSurface;
            videoCall = isVideoCall;
            // enable video
            mRtcEngine.EnableVideo();
            // allow camera output callback
            mRtcEngine.EnableVideoObserver();
        }


        // join channel
        mRtcEngine.JoinChannelByKey(token, channel, null, uID);
    }

    public void leave()
    {
        Debug.Log("calling leave");

        if (mRtcEngine == null)
            return;

        // leave channel
        mRtcEngine.LeaveChannel();
        // deregister video frame observers in native-c code
        mRtcEngine.DisableVideoObserver();
        mRtcEngine.DisableVideo();

    }


    //Mute  Call
    public void MuteCall(bool isMute)
    {
        if (mRtcEngine == null)
        {
            return;
        }
 
        mRtcEngine.MuteLocalAudioStream(isMute);
    }

    //Mute Video Call
    public void DisableVideo(bool isVideo)
    {
        if (mRtcEngine == null)
        {
            return;
        }

        if(!isVideo)
        {
            mRtcEngine.DisableVideo();
            isVideo = true;
        }
        else
        {
            mRtcEngine.EnableVideo();
        }
        
       
    }

    public void EnableVideo()
    {
        if (mRtcEngine == null)
        {
            return;
        }

        mRtcEngine.EnableVideo();
    }


    //Enable Speaker
    public void EnableSpeaker(bool isSpeaker)
    {

        if (mRtcEngine == null)
        {
            return;
        }

        mRtcEngine.SetEnableSpeakerphone(isSpeaker);
    }

    public void SwitchCamera()
    {
        mRtcEngine.SwitchCamera();
    }

    private int LastError { get; set; }
    private void HandleError(int error, string msg)
    {
        if (error == LastError)
        {
            return;
        }

        msg = string.Format("Error code:{0} msg:{1}", error, IRtcEngine.GetErrorDescription(error));

        switch (error)
        {
            case 101:
                msg += "\nPlease make sure your AppId is valid and it does not require a certificate for this demo.";
                break;
        }

        Debug.LogError(msg);


        LastError = error;
    }

    private void onJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("JoinChannelSuccessHandler: uid = " + uid);

    }

    // When a remote user joined, this delegate will be called. Typically
    // create a GameObject to render video on it
    private void onUserJoined(uint uid, int elapsed)
    {
        Debug.Log("onUserJoined: uid = " + uid + " elapsed = " + elapsed);
        // this is called in main thread

        
        if (!videoCall) return;
        Debug.Log("-----------------------------------------Starting Video---------------------------------------");
        VideoSurface vSurface = videoView.GetComponent<VideoSurface>();
        vSurface.SetForUser(uid);
        vSurface.SetEnable(true);
        vSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
        vSurface.SetGameFps(30);
        vSurface._enableFlipVertical = true;

    }


    private void onUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        // remove video stream
        Debug.Log("onUserOffline: uid = " + uid + " reason = " + reason);
        // this is called in main thread
        ChatUIManager.instance.OnCallEndButtonClicked();

    }
}
