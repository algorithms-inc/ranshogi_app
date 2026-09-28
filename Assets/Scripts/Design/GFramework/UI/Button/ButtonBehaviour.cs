using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


// システム割り込み中も動作するボタンかどうかを設定できるようにする。AppMangerにてチェックするようにする？

// TODO: 押下中にずっと呼び出すラムダを用意する→これはボタン表示側にしたほうが良さそう
// TODO: ON時かCLICK時か、
// TODO: ボタンの画像を選択する(9slice か、単純拡大か？)。テキスト文字についてはどうする？
// TODO: 押されると一定割合で下がる。このとき、テキストも同量下がるようにする。絵柄は固定で変更する。（ひとまず、スプライトカラーの変更でいいのでは？）
// TODO: どこかで、ボタンタップをシングル判定にする必要がある。




namespace GFramework
{
    /// ボタンのビヘイビア
    public class ButtonBehaviour : AButtonBaseBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        // Image Type
        private EButtonImageType _buttonImageType;



        // ===============================================
        // MARK: - ● Setup Component ─────

        public virtual void SetupComponent(in FButtonInitializer initializer)
        {
            _buttonImageType = initializer.ButtonImageType;
            _tapJudgeType = initializer.TapJudgeType;
            _area = initializer.Area;
            _isPlaySE = initializer.IsPlaySE;
            _isEnabled = initializer.IsEnabled;

            SetupButtonBody(sortingOrder: initializer.SortingOrder);
            SetupTapInput(targetCanvas: initializer.TargetCanvas);
            SetAction(action: initializer.Action);


            Hide();
        }


        /// ボタン画像のセットアップ
        private void SetupButtonBody(int sortingOrder)
        {
            string texturePathUnderResources;

            if (_isEnabled)
            {
                texturePathUnderResources = Locator.Instance.textureManager.GetFilePath(
                    buttonImageType: _buttonImageType,
                    buttonStatusType: EButtonStatusType.DEFAULT);
            }
            else
            {
                texturePathUnderResources = Locator.Instance.textureManager.GetFilePath(
                    buttonImageType: _buttonImageType,
                    buttonStatusType: EButtonStatusType.DISABLED);
            }


            if (_buttonImageType == EButtonImageType.BUTTON_UNRESIZABLE_PROMOTION_SELECTOR ||
                _buttonImageType == EButtonImageType.BUTTON_UNRESIZABLE_RESIGN_SELECTOR_CANSEL ||
                _buttonImageType == EButtonImageType.BUTTON_UNRESIZABLE_RESIGN_SELECTOR_OK ||
               _buttonImageType == EButtonImageType.BUTTON_UNRESIZABLE_WINDOW_CLOSE )
            {
                SetupButtonBody(texturePathUnderResources: texturePathUnderResources, sortingOrder: sortingOrder, isResizable: false);
            }
            else
            {
                SetupButtonBody(texturePathUnderResources: texturePathUnderResources, sortingOrder: sortingOrder, isResizable: true);
            }
        }



        // ===============================================
        // MARK: - ● Button ─────

        protected override void ChangeButtonTexture(EButtonStatusType buttonStatusType)
        {
            // Editor にてテスト終了時に、NullReferenceException が出ることがあるため、null である場合中断する
            // Locator.Instance自体がなくなってしまっており、再生成されるためエラーなる。そのため、nullチェックしている
            if (Locator.Instance == null) { return; }


            string texturePathUnderResources = Locator.Instance.textureManager.GetFilePath(
                    buttonImageType: _buttonImageType,
                    buttonStatusType: buttonStatusType);

            if (_buttonImageType == EButtonImageType.BUTTON_UNRESIZABLE_PROMOTION_SELECTOR ||
                _buttonImageType == EButtonImageType.BUTTON_UNRESIZABLE_RESIGN_SELECTOR_CANSEL ||
                _buttonImageType == EButtonImageType.BUTTON_UNRESIZABLE_RESIGN_SELECTOR_OK)
            {
                ChangeButtonTexture(texturePathUnderResources: texturePathUnderResources, isResizable: false);
            }
            else
            {
                ChangeButtonTexture(texturePathUnderResources: texturePathUnderResources, isResizable: true);
            }
        }
    }
}