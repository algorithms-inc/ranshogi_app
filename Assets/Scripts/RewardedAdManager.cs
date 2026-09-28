using System;
using UnityEngine;
using GoogleMobileAds.Api;
using System.Collections;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif
using GFramework.Core;

public class RewardedAdManager : MonoBehaviour
{

    public static RewardedAdManager instance = null;

    public delegate void OnUserEarnedReward();
    public delegate void OnAdLoaded();
    public delegate void OnEarnRewardFailed();
    public delegate void OnLoadAdFailed();

    public OnUserEarnedReward onUserEarnedReward;
    public OnAdLoaded onAdLoaded;
    public OnEarnRewardFailed onEarnRewardFailed;
    public OnLoadAdFailed onLoadAdFailed;

    private bool isEarned = false;
    private bool isClosed = false;

    private RewardedAd _rewardedAd;

#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-0000000000000000/0000000000";
#elif UNITY_IOS
    private string _adUnitId = "ca-app-pub-0000000000000000/0000000000";
#else
    private string _adUnitId = "undefined";
#endif

    public void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }


    public void Show()
    {
        const string rewardMsg = "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            AppManager.Instance.AudioController.SetVolumeMaster(0);
            var _this = this;
            isClosed = false;
            _rewardedAd.Show((Reward reward) =>
            {
                isEarned = true;
                if(isClosed) {
                    isEarned = false;
                    //_this.onUserEarnedReward();
                    _this.GrantUnlimited();
                }
            });
        }
    }

    private void GrantUnlimited() {
        var _this = this;
        var request = new GrantUnlimitedRequest();
        request._requestJson = JsonUtility.ToJson(new EmptyStruct());
        Api.GrantUnlimited(request, (response) => {
            Auth.instance.GetUser().unlimitedUntil = response.unlimitedUntil;
            _this.onUserEarnedReward();
        }, (error) => {
            Debug.LogError("GrantUnlimited failed: " + error);
            AlertViewController.ShowOKConfirmation("報酬の獲得に失敗しました。しばらく時間を置いてからお試しください。");
        });
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        var _this = this;
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("#Rewarded ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            float volumeMaster = (PlayerPrefs.HasKey("VolumeMaster") ?  PlayerPrefs.GetInt("VolumeMaster") : 10)  / 10.0f;
            AppManager.Instance.AudioController.SetVolumeMaster(volumeMaster);
            Debug.Log("Rewarded ad full screen content closed.");
            isClosed = true;
            if(isEarned) {
                this.GrantUnlimited();
                isEarned = false;
            }
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                        "with error : " + error);
        };
    }


//  public void Start()
//  {
//      if (instance == null)
//      {
//f UNITY_ANDROID
//          this.rewardedAd = new RewardedAd("ca-app-pub-0000000000000000/0000000000");
//ndif

//f UNITY_IOS
//          this.rewardedAd = new RewardedAd("ca-app-pub-0000000000000000/0000000000");
//ndif 
//          // Called when an ad request has successfully loaded.
//          this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
//          // Called when an ad request failed to load.
//          this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
//          // Called when an ad is shown.
//          this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
//          // Called when an ad request failed to show.
//          this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
//          // Called when the user should be rewarded for interacting with the ad.
//          this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
//          // Called when the ad is closed.
//          this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

//          // Create an empty ad request.

//          instance = this;
//      }
//      else if (instance != this)
//      {
//          Destroy(gameObject);
//      }
//      DontDestroyOnLoad(gameObject);
//  }

//  public void HandleRewardedAdLoaded(object sender, EventArgs args)
//  {
//      this.onAdLoaded();
//  }

//  public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
//  {
//      MonoBehaviour.print("HandleRewardedAdFailedToLoad event received: " + args.LoadAdError);
//      this.onLoadAdFailed();
//  }

//  public void HandleRewardedAdOpening(object sender, EventArgs args)
//  {
//      float volumeMaster = 0;
//      AppManager.Instance.AudioController.SetVolumeMaster(volumeMaster);
//      MonoBehaviour.print("HandleRewardedAdOpening event received");
//  }

//  public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
//  {
//      MonoBehaviour.print(
//          "HandleRewardedAdFailedToShow event received with message: "
//                           + args.Message);
//  }

//  public void HandleRewardedAdClosed(object sender, EventArgs args)
//  {
//      Debug.Log("HandleRewardedAdClosed event received");
//      float volumeMaster = (PlayerPrefs.HasKey("VolumeMaster") ?  PlayerPrefs.GetInt("VolumeMaster") : 10)  / 10.0f;
//      AppManager.Instance.AudioController.SetVolumeMaster(volumeMaster);
//      if (isEarned && !isClosed)
//      {
//          this.onUserEarnedReward();
//      }
//      isClosed = true;
//  }

//  public void HandleUserEarnedReward(object sender, Reward args)
//  {
//      Debug.Log("!!!HandleUserEarnedReward");
//      isEarned = true;
//      PlayerPrefs.SetInt("NAdLife", 1);
//      if(isClosed) { 
//          this.onUserEarnedReward();
//      }
//  }

//  public void Show() {
//      isClosed = false;
//      this.rewardedAd.Show();
//  }
    public void LoadAd() {
        StartCoroutine("LoadAdCoroutine");
    }

    IEnumerator LoadAdCoroutine() {
//       Debug.Log("In Coroutine");
#if UNITY_IOS
      Debug.Log("############## ATTrackingStatusBinding Requesting");
      if (!Application.isEditor && ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED) {
          ATTrackingStatusBinding.RequestAuthorizationTracking();

          while (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
                  ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
          {
              yield return null;
          }
       }
#endif

        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        var _this = this;
        RewardedAd.Load(_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                    "with error : " + error);
                    _this.onLoadAdFailed();
                    return;
                }

                //Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());

                _rewardedAd = ad;
                var options = new ServerSideVerificationOptions();
                options.CustomData = Auth.instance.GetUser().firebaseUid;

                _rewardedAd.SetServerSideVerificationOptions(options);
                _this.RegisterEventHandlers(ad);
                _this.onAdLoaded();
            });

      yield break;
  }

}



