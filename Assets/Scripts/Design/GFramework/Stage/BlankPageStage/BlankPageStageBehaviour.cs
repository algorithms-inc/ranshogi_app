using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using GFramework.Core;


namespace GFramework
{
    public class BlankPageStageBehaviour : ImageActionBehaviour, IBlankPageStage
    {
        // ===============================================
        // MARK: - ● Property ─────

        private IBlankPageManager _blankPageManager;


        public override float Alpha => throw new NotImplementedException();


        private bool _isShown = false;
        public bool IsShown => _isShown;



        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(IBlankPageManager blankPageManager)
        {
            _blankPageManager = blankPageManager;

            Locator.Instance.audioManager.PlayDefaultStageBGM();
        }



        // ===============================================
        // MARK: - ● Alpha ─────

        public override void SetAlpha(float alpha)
        {
            throw new NotImplementedException();
        }


        // ===============================================
        // MARK: - ● Show / Hide ─────

        public void ShowWithAnimation()
        {
            Assert.IsTrue(_isShown == false);


            _isShown = true;
        }

        public override void Show()
        {
            throw new NotImplementedException();
        }

        public override void FadeIn(Action actionOnDidFinish = null)
        {
            throw new NotImplementedException();
        }


        public override void Hide()
        {
            Assert.IsTrue(_isShown);


            _isShown = false;
        }

        public override void FadeOut(Action actionOnDidFinish = null)
        {
            Assert.IsTrue(_isShown);

            // Do notning

            _isShown = false;
        }


        public void HideWithAnimationAndChangeScene(string nextSceneName, EStageType nextStageType)
        {
            Assert.IsTrue(_isShown);

            FadeOut();

            if (AppManager.Instance.UICreator.WindowNormal.IsShown())
            {
                AppManager.Instance.UICreator.WindowNormal.Hide();
            }

            // 次のページと曲が異なるならば、曲をフェードする
            Locator.Instance.audioManager.FadeOutBGMIfNeeded(nextStageType: nextStageType, duration: Config.ANIMATION_DURATION_3);

            var action = CAction.CreateActionFire(
                actionBehaviour: this,
                duration: Config.ANIMATION_DURATION_3,
                action: () => { 
                    AppManager.Instance.DestroyStageAndLoadScene(nextSceneName: nextSceneName);
                 });

            RegisterAction(action: action);
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}