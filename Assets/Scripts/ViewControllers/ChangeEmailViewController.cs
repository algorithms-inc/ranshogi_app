using UnityEngine;
//using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using GFramework.Core;


public class ChangeEmailViewController : MonoBehaviour, IBlankPageManager
{

    private IBlankPageStage _blankPageStage = null;

    async public void Start()
    {
        await Auth.instance.auth.CurrentUser.ReloadAsync();

        Debug.Log(Auth.instance.auth.CurrentUser.Email);

        AppManager.Instance.Setup();
        ShowUpdate();

        _blankPageStage = AppManager.Instance.CreateBlankPageStage(blankPageManager: this);
        _blankPageStage.ShowWithAnimation();
        SetupStatusBar();
    }


    public void OnDidShowAnimation()
    {
        // Do Nothing
    }


    public void Update()
    {
    }

    private void SetupStatusBar()
    {
        AppManager.Instance.ShowStatusBar();
    }



    // ===============================================
    // MARK: - ● Show Window Normal ─────

    private void ShowUpdate()
    {
        SubscriptionView view = FindObjectOfType<SubscriptionView>();
        view.ShowUpdateEmailView(() => {
            _blankPageStage.HideWithAnimationAndChangeScene(nextSceneName: "Settings", nextStageType: EStageType.BLANK_PAGE);
        });
    }

}
