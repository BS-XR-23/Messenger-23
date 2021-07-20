using agora_gaming_rtc;
using UnityEngine;
using UnityEngine.UI;

public class CallEngine
{
    static IRtcEngine mRtcEngine = null;

    bool videoCall;
    RawImage videoView;
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
        GameObject textVersionGameObject = GameObject.Find("VersionText");

    }

    // When a remote user joined, this delegate will be called. Typically
    // create a GameObject to render video on it
    private void onUserJoined(uint uid, int elapsed)
    {
        Debug.Log("onUserJoined: uid = " + uid + " elapsed = " + elapsed);
        // this is called in main thread

        // find a game object to render video stream from 'uid'
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }
        if (!videoCall) return;
        if (uid != 2) return;
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
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            Object.Destroy(go);
        }
    }
}
