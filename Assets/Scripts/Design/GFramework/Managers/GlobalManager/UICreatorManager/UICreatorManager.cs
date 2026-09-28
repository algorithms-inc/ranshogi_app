using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;
using GFramework.Core;
//using UnityEditor.VersionControl;


// 表示、非表示アニメーションをする
// Window を非表示して、非表示アニメーション中に次のものを作成しようとしたらどうする？非表示開始段階で入力を止める必要もある


namespace GFramework
{
    public enum EWindowType
    {
        TOP,
        NORMAL,
        CONFIRMATION,
        ERROR
    }
}


namespace GFramework
{
    public sealed class UICreatorManager : IUICreatorManager
    {
        private WindowBehaviour _windowBehaviourTop = null;
        private WindowBehaviour _windowBehaviourNormal = null;
        private WindowBehaviour _windowBehaviourConfirmation = null;
        private WindowBehaviour _windowBehaviourError = null;

        private UIWindowCreator _windowTop = null;
        public UIWindowCreator WindowTop => _windowTop;

        private UIWindowCreator _windowNormal = null;
        public UIWindowCreator WindowNormal => _windowNormal;

        private UIWindowCreator _windowConfirmation = null;
        public UIWindowCreator WindowConfirmation => _windowConfirmation;

        private UIWindowCreator _windowError = null;
        public UIWindowCreator WindowError => _windowError;


        
        public int FontSize(float fontScale)
        {
            var baseFontSizeOnWindow = _layouter.GetAreaWindowMain(windowType: EWindowType.NORMAL).size.y / 20f / LineSpacing;

            return (int)(baseFontSizeOnWindow * fontScale);
        }

        public float LineSpacing => 1.0f;


        private WindowLayouter _layouter;
        public WindowLayouter Layouter => _layouter;



        public void Setup()
        {
            SetupLayout();

            SetupWindow();
        }


        private void SetupLayout()
        {
            _layouter = new WindowLayouter();
        }


        private void SetupWindow()
        {
            WindowBehaviour Support(EWindowType windowType)
            {
                string gameObjectName = windowType switch
                {
                    EWindowType.TOP => "WindowTop",
                    EWindowType.NORMAL => "WindowNormal",
                    EWindowType.CONFIRMATION => "WindowConfrimation",
                    EWindowType.ERROR => "WindowError",
                    _ => throw new NotImplementedException(),
                };

                var gameObject = new GameObject(gameObjectName);
                var resultWindowBehaviour = gameObject.AddComponent<WindowBehaviour>();

                Utl.SetDontDestroyOnLoad(targetObject: gameObject);

                resultWindowBehaviour.SetupComponent(windowType: windowType);

                return resultWindowBehaviour;
            }

            _windowBehaviourTop = Support(windowType: EWindowType.TOP);
            _windowTop = new UIWindowCreator(windowBehaviour: _windowBehaviourTop);

            _windowBehaviourNormal = Support(windowType: EWindowType.NORMAL);
            _windowNormal = new UIWindowCreator(windowBehaviour: _windowBehaviourNormal);

            _windowBehaviourConfirmation = Support(windowType: EWindowType.CONFIRMATION);
            _windowConfirmation = new UIWindowCreator(windowBehaviour: _windowBehaviourConfirmation);

            _windowBehaviourError = Support(windowType: EWindowType.ERROR);
            _windowError = new UIWindowCreator(windowBehaviour: _windowBehaviourError);
        }



        // ===============================================
        // MARK: - ● Show Simple Window Confirmation  ─────

        public void ShowSimpleWindowConfirmation(string message, EWindowTextColorType textColorType, string textButtonNo, string textButtonYes, Action actionOnTapButtonYes)
        {
            AppManager.Instance.UICreator.WindowConfirmation.TextField.Add(
            name: "textMessage",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.0f),
                endPoint: new Vector2(0.9f, 0.8f)),
            text: message,
            fontScale: 0.95f,
            fontType: EFontType.SERIF,
            textColorType: textColorType,
            alignment: EAlignment.TOP_LEFT,
            actionOnTap: null);

        AppManager.Instance.UICreator.WindowConfirmation.Button.Add(
            name: "buttonNo",
            area: new FWindowElementArea(
                origin: new Vector2(0f, 0.8f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                width: 0.425f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.DEFAULT,
            text: textButtonNo,
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: () => {
                AppManager.Instance.UICreator.WindowConfirmation.Hide(onDidHideAction: null);
            });

        AppManager.Instance.UICreator.WindowConfirmation.Button.Add(
            name: "buttonYes",
            area: new FWindowElementArea(
                origin: new Vector2(0.475f, 0.8f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                width: 0.425f,
                height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
            buttonDesignType: EUIButtonDesignType.WARNING,
            text: textButtonYes,
            isPlaySE: true,
            isEnabled: true,
            actionOnTap: actionOnTapButtonYes);

        AppManager.Instance.UICreator.WindowConfirmation.Show();
        }



        // ===============================================
        // MARK: - ● Get Base Button Height ─────

        public float GetBaseButtonHeight()
        {
            return 0.175f;
        }



        // ===============================================
        // MARK: - ● Show Layout Area ─────

        public void ShowLayoutArea()
        {
            _layouter.ShowLayoutArea();
        }



        // ===============================================
        // MARK: - ● Destroy ─────

        public void Destroy()
        {
            Utl.SafeDestroy(behaviour: _windowBehaviourTop);
            Utl.SafeDestroy(behaviour: _windowBehaviourNormal);
            Utl.SafeDestroy(behaviour: _windowBehaviourConfirmation);
            Utl.SafeDestroy(behaviour: _windowBehaviourError);
        }
    }
}