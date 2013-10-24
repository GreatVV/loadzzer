using System;
using System.Collections.Generic;
using Facebook;
using UnityEngine;

public class FacebookIntegration : MonoBehaviour
{
    public string AccessToken;
    public bool FbInited;

    public List<object> Friends;
    public Dictionary<string, string> Profile;
    public Texture ProfilePicture;
    public string Username;

    #region Event Handlers

    private void OnHide(bool isunityshown)
    {
    }

    private void OnInit()
    {
        Debug.Log("Fb inited");
        FbInited = true;
       // FB.GetAuthResponse(OnLogin);
    }

    private void OnLogin(FBResult result)
    {
        AccessToken = FB.AccessToken;
        Debug.Log("Login: " + result.Text + result.Error);
        Debug.Log("call login: " + FB.UserId);

        if (result.Error != null)
        {
            Debug.Log(result.Text);
            Debug.LogError(result.Error);
            return;
        }                                     
        RequestProfile();
    }

    private void RequestProfile()
    {
        AccessToken = FB.AccessToken;
        FB.API("/me?fields=id,first_name", HttpMethod.GET, OnPlayerInfo,
            new Dictionary<string, string> {{"access_token", AccessToken}});
        //FB.API("/me/picture?width=128&height=128&access_token=" + AccessToken, Facebook.HttpMethod.GET, OnPicture);
    }

    private void OnPlayerInfo(FBResult result) // handle user profile info
    {
        if (result.Error != null)
        {
            Debug.Log(result.Text);
            Debug.LogError(result.Error);
            return;
        }

        Profile = Util.DeserializeJSONProfile(result.Text);
        Username = Profile["first_name"];
        //  Friends = Util.DeserializeJSONFriends(result.Text);
    }

    private void OnPicture(FBResult result) // store user profile pic
    {
        if (result.Error != null)
        {
            Debug.LogError(result.Error);
            return;
        }

        ProfilePicture = result.Texture;
    }

    #endregion

    private void Start()
    {
        if (FB.IsLoggedIn && !string.IsNullOrEmpty(FB.AccessToken))
        {
            OnLogin(new FBResult("0"));
        }
        else
        {
            if(!FbInited)
            {
                FB.Init(OnInit, OnHide);
                Debug.Log("fb not inited");
            }
            else
            {
                FB.Login("email,publish_actions", OnLogin);
            }
        }
       
    }

    public void OnFBLoginClick()
    {
        Debug.Log("Fb connect clicked");
   
        if (!FB.IsLoggedIn)
        {
            Debug.Log("Try login");
            FB.Login("email,publish_actions", OnLogin);
        }
        else
        {
            Debug.Log("Logged! " + FB.UserId);
            RequestProfile();
        }
    }

    private void CallFBPublishInstall()
    {
        FB.PublishInstall(PublishComplete);
    }

    private void PublishComplete(FBResult result)
    {
        Debug.Log("publish response: " + result.Text);
    }
}
            /*

public class FacebookAuthenticator
{
    public string ApiQuery = "";
    private Action<string, string> GotFacebookIdAndAccessToken;
    public Action<FBResult> SessionOpened;
    //public Action<PlayerIOError> ErrorOccured;

    private bool _development;
    private string lastResponse = "";
    private Texture2D lastResponseTexture;

    private string permissions = "publish_actions,publish_stream";

    public void FacebookLogin(bool development)
    {
        Debug.Log("FacebookLogin");
        _development = development;
#if UNITY_ANDROID || UNITY_IPHONE
        if (FB.IsLoggedIn && !string.IsNullOrEmpty(FB.AccessToken))
        {
            OnSessionOpenedWhenLogin(null);
        }
        else
        {
            // if(!PlayerIONetwork.Instance.FBInited)
            {
                FB.Init(OnInitComplete, OnHideUnity);
                Debug.Log("fb not inited");
            }
            //  else
            {
                FB.Login(permissions, OnSessionOpenedWhenLogin);
            }
        }
#endif
    }

    private void OnInitComplete()
    {
        Debug.Log("Facebook init completed");
        //PlayerIONetwork.Instance.FBInited = true;
        FB.Login(permissions, OnSessionOpenedWhenLogin);
    }

    private void CallGetAuthResponse()
    {
        FB.GetAuthResponse(Callback);
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void FacebookOpenSession()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        if (FB.IsLoggedIn && !string.IsNullOrEmpty(FB.AccessToken))
        {
            Debug.LogWarning("islogged");
            InvokeSessionOpened(null);
        }
        else
        {
            Debug.LogWarning("before login");

            //  if(!PlayerIONetwork.Instance.FBInited)
            {
                FB.Init(delegate
                {
                    Debug.LogWarning("delegate");
                    // PlayerIONetwork.Instance.FBInited = true;
                    if (FB.IsLoggedIn && !string.IsNullOrEmpty(FB.AccessToken))
                    {
                        Debug.LogWarning("logged in userid: " + FB.UserId);
                        InvokeSessionOpened(null);
                    }
                    else
                    {
                        FB.Login(permissions, InvokeSessionOpened);
                    }
                }, OnHideUnity);
            }
            // else
            {
                FB.Login(permissions, InvokeSessionOpened);
            }
        }
#endif
    }

    public void GetFacebookIdAndAccessToken(Action<string, string> callback)
    {
#if UNITY_ANDROID || UNITY_IPHONE
        if (!FB.IsLoggedIn || String.IsNullOrEmpty(FB.AccessToken))
        {
            Debug.LogWarning("get id session not valid");
            GotFacebookIdAndAccessToken = callback;
            SessionOpened += OnSessionOpenedWhenGetFacebookId;
            FacebookOpenSession();
        }
        else
        {
            Debug.LogWarning("get id session valid");
            string facebookToken = FB.AccessToken;
            Debug.Log("Facebook token: " + facebookToken);
            callback(FB.UserId, facebookToken);
        }
#endif
    }

    public void Deinitialize()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        SessionOpened -= OnSessionOpenedWhenLogin;
        SessionOpened -= OnSessionOpenedWhenGetFacebookId;
#endif
    }

    /*
 private void InvokeOnErrorOccured(PlayerIOError error)
 {
  var handler = ErrorOccured;
  if(handler != null)
  {
   handler(error);
  }
 }
      

    private void InvokeSessionOpened(FBResult result)
    {
        Debug.Log("invokesessionopened: " + result);
        if (result != null)
        {
            Debug.Log("invokesessionopened error: " + result.Error);
            if (!String.IsNullOrEmpty(result.Error))
            {
                //return;
            }
        }
        else
        {
            Debug.Log("invokesessionopened: result is null");
        }

        Action<FBResult> handler = SessionOpened;
        if (handler != null)
        {
            handler(result);
        }
    }

    
 private void OnError(PlayerIOError error)
 {
  InvokeOnErrorOccured(error);
 }                    

    private void OnSessionOpenedWhenLogin(FBResult result)
    {
        if (result != null)
        {
            Debug.Log("OnSessionOpenedWhenLogin " + result.Error);
        }
        else
        {
            Debug.Log("onsessionopenedwhenlogin: " + result);
        }
        SessionOpened -= OnSessionOpenedWhenLogin;

#if UNITY_ANDROID || UNITY_IPHONE
        string accesToken = FB.AccessToken;
        Debug.Log("Token: " + accesToken);
        Debug.Log("userid: " + FB.UserId);
        //PlayerIONetwork.Instance.ConnectViaFacebook(_development, accesToken, OnError);
#endif
    }

    private void OnSessionOpenedWhenGetFacebookId(FBResult result)
    {
        Debug.Log("onsessionOpenedWhenGetFacebookId: " + result);
        if (result != null)
        {
            Debug.Log("onsessionopenedwhengetfacebookid: " + result.Error);
        }

        SessionOpened -= OnSessionOpenedWhenGetFacebookId;

#if UNITY_ANDROID || UNITY_IPHONE
        Debug.LogWarning("token fid: " + FB.AccessToken);
        if (String.IsNullOrEmpty(FB.AccessToken))
        {
            return;
        }
        InvokeGotFacebookIdAndAccessToken(FB.UserId, FB.AccessToken);

#endif
    }

    private void InvokeGotFaceobokIdAndAccessToken(string userId, string token)
    {
        Action<string, string> handler = GotFacebookIdAndAccessToken;
        if (handler != null)
        {
            handler(userId, token);
        }
    }

    private void Callback(FBResult result)
    {
        lastResponseTexture = null;
        if (result.Error != null)
        {
            lastResponse = "Error Response:\n" + result.Error;
        }
        else if (!ApiQuery.Contains("/picture"))
        {
            lastResponse = "Success Response:\n" + result.Text;
        }
        else
        {
            lastResponseTexture = result.Texture;
            lastResponse = "Success Response:\n";
            Debug.Log("access token: " + FB.AccessToken);
        }
    }
}              */