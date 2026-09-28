using System;
using System.Collections.Generic;
using UnityEngine;
using GFramework.Core;


namespace GFramework
{
    /// <summary>
    /// 「AIと対戦」ウィンドウ(強さ・手番を選んで対局開始)。検討画面で使う。
    /// </summary>
    public static class AiMatchWindow
    {
        private static readonly (string Text, RanshogiEngine.Level Level)[] LevelList = {
            ("やさしい", RanshogiEngine.Level.Easy),
            ("ふつう",   RanshogiEngine.Level.Normal),
            ("つよい",   RanshogiEngine.Level.Hard),
        };

        // 強さの初期選択(つよい)
        private const int DefaultLevelIndex = 2;


        /// <param name="note">タイトルの下に出す説明(どの局面から対局するか)</param>
        /// <param name="onStart">対局開始が押されたとき。ウィンドウが閉じてから呼ばれる。</param>
        public static void Show(string note, Action<Color, RanshogiEngine.Level> onStart)
        {
            var window = AppManager.Instance.UICreator.WindowNormal;

            window.TextField.Add(
                name: "textFieldTitleOnAiMatchWindow",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 0.0f),
                    endPoint: new Vector2(1f, 0.15f)),
                text: "AIと対戦",
                fontScale: 1.0f,
                fontType: EFontType.SERIF,
                textColorType: EWindowTextColorType.NORMAL,
                alignment: EAlignment.TOP_CENTER,
                actionOnTap: null);

            window.TextField.Add(
                name: "textFieldNoteOnAiMatchWindow",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 0.15f),
                    endPoint: new Vector2(1f, 0.27f)),
                text: note,
                fontScale: 0.7f,
                fontType: EFontType.SERIF,
                textColorType: EWindowTextColorType.NORMAL,
                alignment: EAlignment.TOP_CENTER,
                actionOnTap: null);

            // 見出し + ラジオボタン(縦並び)を追加し、ラジオボタンの下端の y を返す
            float AddRadioButtonGroup(string name, string heading, float top, string[] textList, int indexOfOn)
            {
                window.TextField.Add(
                    name: name + "Heading",
                    area: new FWindowElementArea(
                        origin: new Vector2(0f, top),
                        endPoint: new Vector2(1f, top + 0.1f)),
                    text: heading,
                    fontScale: 0.8f,
                    fontType: EFontType.SERIF,
                    textColorType: EWindowTextColorType.NORMAL,
                    alignment: EAlignment.TOP_LEFT,
                    actionOnTap: null);

                var areaList = new List<FWindowElementArea>();
                var textSettingList = new List<FCompanionTextFieldSetting>();
                var y = top + 0.1f;
                foreach (var text in textList)
                {
                    areaList.Add(new(origin: new Vector2(0.05f, y), endPoint: new Vector2(0.17f, y + 0.12f)));
                    textSettingList.Add(new(
                        windowElementArea: new FWindowElementArea(origin: new Vector2(0.21f, y), endPoint: new Vector2(1.0f, y + 0.12f)),
                        text: text,
                        fontType: EFontType.SERIF,
                        fontScale: 1.0f,
                        alignment: EAlignment.MIDDLE_LEFT));
                    y += 0.13f;
                }

                window.RadioButtonGroup.Add(
                    name: name,
                    areaList: areaList,
                    companionTextFieldSettingList: textSettingList,
                    indexOfOn: indexOfOn,
                    actionOnTap: null,
                    isEnabled: true);

                return y;
            }

            var bottomOfLevel = AddRadioButtonGroup(
                name: "radioButtonLevelOnAiMatchWindow",
                heading: "強さ",
                top: 0.29f,
                textList: Array.ConvertAll(LevelList, i => i.Text),
                indexOfOn: DefaultLevelIndex);

            AddRadioButtonGroup(
                name: "radioButtonColorOnAiMatchWindow",
                heading: "手番",
                top: bottomOfLevel + 0.03f,
                textList: new[] { "先手", "後手" },
                indexOfOn: 0);  // 先手

            window.Button.Add(
                name: "buttonStartOnAiMatchWindow",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 1.225f),
                    endPoint: new Vector2(1f, 1.4f)),
                buttonDesignType: EUIButtonDesignType.POSITIVE,
                text: "対局開始",
                isPlaySE: true,
                isEnabled: true,
                actionOnTap: () => {
                    var level = LevelList[window.RadioButtonGroup.GetIndexOfOn(name: "radioButtonLevelOnAiMatchWindow")].Level;
                    var playerColor = window.RadioButtonGroup.GetIndexOfOn(name: "radioButtonColorOnAiMatchWindow") == 0 ? Color.Black : Color.White;

                    window.Hide(onDidHideAction: () => { onStart(playerColor, level); });
                });

            window.Button.Add(
                name: "buttonCloseOnAiMatchWindow",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 1.425f),
                    endPoint: new Vector2(1f, 1.6f)),
                buttonDesignType: EUIButtonDesignType.DEFAULT,
                text: "閉じる",
                isPlaySE: true,
                isEnabled: true,
                actionOnTap: () => {
                    window.Hide(onDidHideAction: null);
                });

            window.Show(onDidShowAction: null);
        }
    }
}
