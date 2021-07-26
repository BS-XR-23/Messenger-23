using agora_gaming_rtc;
using UnityEngine;
using UnityEngine.UI;

public class CallEngine
{
    //todo check if necessary
    static IRtcEngine _mRtcEngine = null;

    private bool videoCall;
    private RawImage videoView;


    private int LastError { get; set; }

    public void LoadEngine()
    {
        // start sdk
        Debug.Log("initializeEngine");

        if (_mRtcEngine != null)
        {
            Debug.Log("Engine exists. Please unload it first!");
            return;
        }

        // init engine
        _mRtcEngine = IRtcEngine.GetEngine(MessengerManager.instance.GetAppID());

        // enable log
        _mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
    }

    public void UnloadEngine()
    {
        Debug.Log("calling unloadEngine");

        // delete
        if (_mRtcEngine != null)
        {
            IRtcEngine.Destroy();
            _mRtcEngine = null;
        }
    }





    public void Join(string channel, string token, uint uId,RawImage videoSurface = null)
    {
        Debug.Log("calling join (channel = " + channel + ")");

        if (_mRtcEngine == null)
            return;
        
        // set callbacks (optional)
        _mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
        _mRtcEngine.OnUserJoined = OnUserJoined;
        _mRtcEngine.OnUserOffline = OnUserOffline;

        _mRtcEngine.OnWarning = OnWarning;
        _mRtcEngine.OnError = HandleError;


        videoCall = videoSurface != null;
        if (videoSurface!=null)
        {
            videoView = videoSurface;
            // enable video
            _mRtcEngine.EnableVideo();
            // allow camera output callback
            _mRtcEngine.EnableVideoObserver();
        }


        // join channel
        _mRtcEngine.JoinChannelByKey(token, channel, null, uId);
    }

    

    public void Leave()
    {
        Debug.Log("calling leave");

        if (_mRtcEngine == null)
            return;

        // leave channel
        _mRtcEngine.LeaveChannel();
        // deregister video frame observers in native-c code
        _mRtcEngine.DisableVideoObserver();

    }



    
    private void HandleError(int error, string msg)
    {
        if (error == LastError)
        {
            return;
        }

        msg = $"Error code:{error} msg:{IRtcEngine.GetErrorDescription(error)}";

        switch (error)
        {
            case 101:
                msg += "\nPlease make sure your AppId is valid and it does not require a certificate for this demo.";
                break;
            default:
                msg += "\nUnknown Error!";
                break;
        }

        Debug.LogError(msg);


        LastError = error;
    }


    private void OnWarning(int warn, string msg)
    {
        Debug.LogWarningFormat("Warning code:{0} msg:{1}", warn, IRtcEngine.GetErrorDescription(warn));
    }
    private void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
        //TODO Fix this 
        GameObject textVersionGameObject = GameObject.Find("VersionText");

    }

    // When a remote user joined, this delegate will be called. Typically
    // create a GameObject to render video on it
    private void OnUserJoined(uint uid, int elapsed)
    {
        Debug.Log("onUserJoined: uid = " + uid + " elapsed = " + elapsed);
        // this is called in main thread

        // find a game object to render video stream from 'uid'
        //TODO Fix this 
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }
        if (!videoCall) return;
        Debug.Log("-----------------------------------------Starting Video---------------------------------------");
        VideoSurface vSurface = videoView.GetComponent<VideoSurface>();
        vSurface.SetForUser(uid);
        vSurface.SetEnable(true);
        vSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
        vSurface.SetGameFps(30);
        vSurface._enableFlipVertical = true;

    }


    private void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
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
