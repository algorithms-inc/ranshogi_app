using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GFramework.Core;



namespace GFramework
{
    public class MenuStageBehaviour : ImageActionBehaviour, IMenuStage
    {
        // ===============================================
        // MARK: - ● Property ─────

        private IMenuManager _menuManager;

        private MenuStageLayouter _layouter;


        public override float Alpha => _titleLogoBehaviour.Alpha;

        private bool _isShown = false;
        public bool IsShown => _isShown;


        private Canvas _canvas;

        private TitleLogoBehaviour _titleLogoBehaviour;

        private List<ButtonBehaviour> _buttonBehaviourList = new List<ButtonBehaviour>();
        private ButtonBehaviour _buttonGame66;
        private ButtonBehaviour _buttonKachikire;
        private ButtonBehaviour _buttonGameWithFriend;
        private ButtonBehaviour _buttonHistory;
        private ButtonBehaviour _buttonSetting;
        private ButtonBehaviour _buttonRecord;

        private ButtonBehaviour _buttonDebugMenu;


        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(IMenuManager menuManager)
        {
            _menuManager = menuManager;

            SetupLayout();

            SetupCanvas();

            SetupDebugButton();

            SetupTitleLogo();

            SetupButton();

            HideButton();

            Locator.Instance.audioManager.PlayDefaultStageBGM();
        }


        public void SetupLayout()
        {
            _layouter = new MenuStageLayouter();
        }


        private void SetupCanvas()
        {
            var gameObjectWithCanvas = Utl.CreateGameObjectWithCanvas(
                name: "Canvas",
                size: Locator.Instance.layoutManager.AreaUICanvas.size,
                localScale: Locator.Instance.layoutManager.ScaleUICanvas,
                parentTransform: gameObject.transform,
                sortingOrder: Config.MENU_STAGE_INPUT_CANVAS__LAYERED_ORDERID.SortingOrder);

            _canvas = gameObjectWithCanvas.GetComponent<Canvas>();
        }

        private void SetupDebugButton()
        {
            if (AppManager.Instance.IsDebugMode)
            {
                _buttonDebugMenu = Support(
                    name: "ButtonDebugMenu",
                    buttonText: "Debug",
                    buttonImageType: EButtonImageType.BUTTON_RESIZABLE_WARNING,
                    buttonArea: _layouter.AreaButtonDebugMenu,
                    action: () => { OnTapButtonDebugMenu(); });
            }
        }

        private void SetupTitleLogo()
        {
            var gameObjectTitleLogo = new GameObject("TitleLogo");
            _titleLogoBehaviour = gameObjectTitleLogo.AddComponent<TitleLogoBehaviour>();
            _titleLogoBehaviour.SetupComponent(layouter: _layouter);

            gameObjectTitleLogo.transform.parent = transform;
        }


        private ButtonBehaviour Support(string name, string buttonText, EButtonImageType buttonImageType, FArea buttonArea, Action action)
        {
            GameObject gameObjectButton = new GameObject(name: name);
            var buttonBehaviour = gameObjectButton.AddComponent<ButtonBehaviour>();

            gameObjectButton.transform.parent = gameObject.transform;

            var buttonInitializer = new FButtonInitializer(
                targetCanvas: _canvas,
                buttonImageType: buttonImageType,
                area: buttonArea,
                tapJudgeType: ETapJudgeType.CLICK,
                isPlaySE: true,
                isEnabled: true,
                sortingOrder: Config.MENU_STAGE_BUTTON__LAYERED_ORDERID.SortingOrder,
                action: action);

            buttonBehaviour.SetupComponent(initializer: buttonInitializer);


            var buttonTextInitializer = new FButtonTextInitializer(
                text: buttonText,
                fontType: EFontType.SERIF,
                fontSize: (int)(buttonArea.size.y * 0.5f),
                alignment: EAlignment.MIDDLE_CENTER);
            buttonBehaviour.SetupText(buttonTextInitializer: buttonTextInitializer);


            return buttonBehaviour;
        }

        private void SetupButton()
        {
            _buttonGame66 = Support(
                name: "ButtonGame66",
                buttonText: "    乱将棋で対局",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_POSITIVE,
                buttonArea: _layouter.AreaButtonGame66,
                action: () => { OnTapButtonGame66(); });
            _buttonBehaviourList.Add(_buttonGame66);

            _buttonGame66.SetupCompanionImage(
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForButtonIcon.PLAY_RANSHOGI),
                area: _layouter.AreaButtonGame66CompanionImage);

            // 「将棋で対局」の位置に置く
            _buttonKachikire = Support(
                name: "ButtonKachikire",
                buttonText: "勝ち切れ乱将棋",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_POSITIVE,
                buttonArea: _layouter.AreaButtonGame99,
                action: () => { OnTapButtonKachikire(); });
            _buttonBehaviourList.Add(_buttonKachikire);

            _buttonKachikire.SetupCompanionImage(
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForButtonIcon.PLAY_SHOGI),
                area: _layouter.AreaButtonGame99CompanionImage);


            _buttonGameWithFriend = Support(
                name: "ButtonGameWithFriend",
                buttonText: "友達と対局",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_POSITIVE,
                buttonArea: _layouter.AreaButtonGameWithFriend,
                action: () => { OnTapButtonGameWithFriend(); });
            _buttonBehaviourList.Add(_buttonGameWithFriend);

            _buttonGameWithFriend.SetupCompanionImage(
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForButtonIcon.PLAY_WITH_FRIEND),
                area: _layouter.AreaButtonGameWithFriendCompanionImage);


            _buttonHistory = Support(
                name: "ButtonHistory",
                buttonText: "対局履歴",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButtonMypageWithoutRanking,
                action: () => { OnTapButtonHistory(); });
            _buttonBehaviourList.Add(_buttonHistory);
        
            _buttonSetting = Support(
                name: "ButtonSetting",
                buttonText: "設定",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButtonSetting,
                action: () => { OnTapButtonSetting(); });
            _buttonBehaviourList.Add(_buttonSetting);

            // ランキングは廃止し、同じ場所にこれまでのマイページ(成績)を開くボタンを置く
            _buttonRecord = Support(
                name: "ButtonRecord",
                buttonText: "成績",
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_DEFAULT,
                buttonArea: _layouter.AreaButtonRanking,
                action: () => { OnTapButtonRecord(); });
            _buttonBehaviourList.Add(_buttonRecord);
        }



        // ===============================================
        // MARK: - ● Menu Manager への接続 ─────

        private void OnTapButtonGame66()
        {
            if (AppManager.Instance.IsDebugMode)
            {
                Locator.Instance.debugManager.BoardType = EBoardType.BOARD_66;
                Locator.Instance.debugManager.IsWithFriendMode = false;
            }

            _menuManager.OnTapButtonGame66();
        }

        private void OnTapButtonKachikire()
        {
            _menuManager.OnTapButtonKachikire();
        }

        private void OnTapButtonGameWithFriend()
        {
            if (AppManager.Instance.IsDebugMode)
            {
                Locator.Instance.debugManager.IsWithFriendMode = true;
            }

            _menuManager.OnTapButtonGameWithFriend();
        }


        private void OnTapButtonHistory()
        {
            _menuManager.OnTapButtonHistory();
        }


        private void OnTapButtonRecord()
        {
            _menuManager.OnTapButtonRecord();
        }


        private void OnTapButtonSetting()
        {
            _menuManager.OnTapButtonSetting();
        }


        private void OnTapButtonHelp()
        {
            _menuManager.OnTapButtonHelp();
        }


        private void OnTapButtonDebugMenu()
        {
            HideWithAnimationAndChangeScene(nextSceneName: "DebugMenuStageScene", nextStageType: EStageType.GENERAL_BUTTON_LIST);
        }



        // ===============================================
        // MARK: - ● Window Kachikire ─────

        private const string KACHIKIRE_CLEAR_MARK_FILE_PATH = "UI/ImageBox/kachikire_clear_check.png";

        private int _kachikireYear;
        private int _kachikireMonth;
        private DateTime _kachikireToday;
        private DateTime _kachikireStartDate;
        private List<FKachikireDay> _kachikireDayList = new List<FKachikireDay>();
        // 選択中の日。表示中の月の日のみ
        private DateTime? _kachikireSelectedDate = null;


        public void ShowWindowKachikireCalendar(int year, int month, DateTime today, DateTime startDate, IReadOnlyList<FKachikireDay> days)
        {
            _kachikireYear = year;
            _kachikireMonth = month;
            _kachikireToday = today.Date;
            _kachikireStartDate = startDate.Date;
            _kachikireDayList = new List<FKachikireDay>(days);

            // 選択は今日 (今日が表示中の月に無ければ未選択)
            var isTodayInMonth = (_kachikireToday.Year == year) && (_kachikireToday.Month == month);
            _kachikireSelectedDate = isTodayInMonth ? _kachikireToday : (DateTime?)null;

            BuildWindowKachikireCalendar();

            AppManager.Instance.UICreator.WindowNormal.Show(onDidShowAction: null);
        }


        // 局面がある日のみ返す。開始日より前の日は局面があっても扱わない
        private FKachikireDay? FindKachikireDay(DateTime date)
        {
            if (date.Date < _kachikireStartDate) { return null; }

            foreach (var day in _kachikireDayList)
            {
                if (day.Date == date.Date) { return day; }
            }
            return null;
        }


        private void BuildWindowKachikireCalendar()
        {
            var window = AppManager.Instance.UICreator.WindowNormal;
            window.RemoveAllUIParts();

            void AddText(string name, float x0, float y0, float x1, float y1, string text, float fontScale,
                EAlignment alignment, EWindowTextColorType textColorType = EWindowTextColorType.NORMAL, Action actionOnTap = null)
            {
                window.TextField.Add(
                    name: name,
                    area: new FWindowElementArea(origin: new Vector2(x0, y0), endPoint: new Vector2(x1, y1)),
                    text: text,
                    fontType: EFontType.SERIF,
                    fontScale: fontScale,
                    textColorType: textColorType,
                    alignment: alignment,
                    actionOnTap: actionOnTap);
            }

            AddText("textFieldTitleOnKachikire", 0f, 0f, 1f, 0.12f, "勝ち切れ乱将棋", 1.0f, EAlignment.TOP_CENTER);
            AddText("textFieldNoteOnKachikire", 0f, 0.12f, 1f, 0.21f, "有利な局面から、最強のAIに勝ち切ろう", 0.6f, EAlignment.TOP_CENTER);


            // --- 月の切り替え
            var firstDayOfMonth = new DateTime(_kachikireYear, _kachikireMonth, 1);
            var previousMonth = firstDayOfMonth.AddMonths(-1);
            var nextMonth = firstDayOfMonth.AddMonths(1);
            var firstDayOfTodayMonth = new DateTime(_kachikireToday.Year, _kachikireToday.Month, 1);
            var firstDayOfStartMonth = new DateTime(_kachikireStartDate.Year, _kachikireStartDate.Month, 1);

            window.Button.Add(
                name: "buttonPreviousMonthOnKachikire",
                area: new FWindowElementArea(origin: new Vector2(0f, 0.23f), endPoint: new Vector2(0.2f, 0.35f)),
                actionOnTap: () => { _menuManager.OnTapKachikireChangeMonth(year: previousMonth.Year, month: previousMonth.Month); },
                text: "◀",
                buttonDesignType: EUIButtonDesignType.DEFAULT,
                isPlaySE: true,
                // 開始日より前の月には局面が無い
                isEnabled: previousMonth >= firstDayOfStartMonth);

            AddText("textFieldMonthOnKachikire", 0.2f, 0.23f, 0.8f, 0.35f, _kachikireYear + "年" + _kachikireMonth + "月", 0.9f, EAlignment.MIDDLE_CENTER);

            window.Button.Add(
                name: "buttonNextMonthOnKachikire",
                area: new FWindowElementArea(origin: new Vector2(0.8f, 0.23f), endPoint: new Vector2(1f, 0.35f)),
                actionOnTap: () => { _menuManager.OnTapKachikireChangeMonth(year: nextMonth.Year, month: nextMonth.Month); },
                text: "▶",
                buttonDesignType: EUIButtonDesignType.DEFAULT,
                isPlaySE: true,
                // 未来の月には局面が無い
                isEnabled: nextMonth <= firstDayOfTodayMonth);


            // --- 曜日
            const float cellWidth = 1f / 7f;
            string[] weekdayTextList = { "日", "月", "火", "水", "木", "金", "土" };
            for (int i = 0; i < weekdayTextList.Length; i++)
            {
                var colorType = i == 0 ? EWindowTextColorType.WARNING : (i == 6 ? EWindowTextColorType.LINK : EWindowTextColorType.NORMAL);
                AddText("textFieldWeekday" + i + "OnKachikire", cellWidth * i, 0.37f, cellWidth * (i + 1), 0.45f, weekdayTextList[i], 0.6f, EAlignment.MIDDLE_CENTER, colorType);
            }


            // --- 日付 (6行 × 7列)
            const float gridTop = 0.46f;
            const float rowHeight = 0.12f;
            const float margin = 0.006f;
            int offset = (int)firstDayOfMonth.DayOfWeek;
            int daysInMonth = DateTime.DaysInMonth(_kachikireYear, _kachikireMonth);

            for (int dayNum = 1; dayNum <= daysInMonth; dayNum++)
            {
                var date = new DateTime(_kachikireYear, _kachikireMonth, dayNum);
                var index = offset + dayNum - 1;
                var x0 = cellWidth * (index % 7) + margin;
                var y0 = gridTop + rowHeight * (index / 7) + margin;
                var x1 = cellWidth * (index % 7 + 1) - margin;
                var y1 = gridTop + rowHeight * (index / 7 + 1) - margin;

                var kachikireDay = FindKachikireDay(date);
                var isSelected = _kachikireSelectedDate == date;

                void Select()
                {
                    _kachikireSelectedDate = date;
                    BuildWindowKachikireCalendar();
                }

                // 局面が無い日・未来の日・開始日より前の日は選べないので、ボタンを出さずに日付だけを書く
                if (kachikireDay.HasValue == false)
                {
                    AddText("textFieldDay" + dayNum + "OnKachikire", x0, y0, x1, y1, dayNum.ToString(), 0.7f,
                        EAlignment.MIDDLE_CENTER, EWindowTextColorType.DISABLED);
                    continue;
                }

                var buttonName = "buttonDay" + dayNum + "OnKachikire";
                window.Button.Add(
                    name: buttonName,
                    area: new FWindowElementArea(origin: new Vector2(x0, y0), endPoint: new Vector2(x1, y1)),
                    actionOnTap: Select,
                    text: dayNum.ToString(),
                    buttonDesignType: isSelected ? EUIButtonDesignType.POSITIVE : EUIButtonDesignType.DEFAULT,
                    isPlaySE: true,
                    // 勝利済みの日も再挑戦できる
                    isEnabled: true);

                if (kachikireDay.Value.IsCleared)
                {
                    // 勝利済みの日は、右上に緑のチェックマークを重ねる
                    var markSize = (y1 - y0) * 0.45f;
                    window.Button.SetCompanionImage(
                        name: buttonName,
                        area: new FWindowElementArea(origin: new Vector2(x1 - markSize, y0), width: markSize, height: markSize),
                        filePath: KACHIKIRE_CLEAR_MARK_FILE_PATH);
                }
            }


            // --- 選択中の日と対局開始
            var selectedDay = _kachikireSelectedDate.HasValue ? FindKachikireDay(_kachikireSelectedDate.Value) : null;

            string selectedText;
            if (_kachikireSelectedDate.HasValue == false)
            {
                selectedText = "日付を選んでください";
            }
            else if (selectedDay.HasValue == false)
            {
                selectedText = _kachikireSelectedDate.Value.Month + "月" + _kachikireSelectedDate.Value.Day + "日の局面はありません";
            }
            else
            {
                selectedText = _kachikireSelectedDate.Value.Month + "月" + _kachikireSelectedDate.Value.Day + "日の局面" + (selectedDay.Value.IsCleared ? "（勝利済み）" : "");
            }
            AddText("textFieldSelectedDateOnKachikire", 0f, 1.19f, 1f, 1.27f, selectedText, 0.7f, EAlignment.MIDDLE_CENTER);

            window.Button.Add(
                name: "buttonStartOnKachikire",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 1.275f),
                    width: 1.0f,
                    height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
                actionOnTap: () => {
                    var date = _kachikireSelectedDate.Value;
                    window.Hide(onDidHideAction: () => { _menuManager.OnTapKachikireStart(date: date); });
                },
                text: "対局開始",
                buttonDesignType: EUIButtonDesignType.POSITIVE,
                isPlaySE: true,
                isEnabled: selectedDay.HasValue);

            window.Button.Add(
                name: "buttonCloseOnKachikire",
                area: new FWindowElementArea(
                    origin: new Vector2(0f, 1.65f - AppManager.Instance.UICreator.GetBaseButtonHeight()),
                    width: 1.0f,
                    height: AppManager.Instance.UICreator.GetBaseButtonHeight()),
                actionOnTap: () => { window.Hide(onDidHideAction: null); },
                text: "閉じる",
                buttonDesignType: EUIButtonDesignType.DEFAULT,
                isPlaySE: true,
                isEnabled: true);
        }



        // ===============================================
        // MARK: - ● Alpha ─────

        public override void SetAlpha(float alpha)
        {
            throw new NotImplementedException();
        }



        // ===============================================
        // MARK: - ● Show / Hide with Animation ─────

        public void ShowWithAnimation()
        {
            Assert.IsTrue(_isShown == false);

            _buttonBehaviourList.ForEach(i => i.FadeIn());

            _buttonDebugMenu?.FadeIn();

            _titleLogoBehaviour.ShowWithAnimation(isFull: true);

            // TODO: この数字を決めうちにしないこと
            var waitDuration = 0.5f;
            var action = CAction.CreateActionFire(actionBehaviour: this, duration: waitDuration, action: () => { OnDidShowAnimation(); });
            RegisterAction(action: action);

            _isShown = true;
        }


        public void OnDidShowAnimation()
        {
            _menuManager.OnDidShowAnimation();
        }
 

        public void HideWithAnimationAndChangeScene(string nextSceneName, EStageType nextStageType)
        {
            Assert.IsTrue(_isShown);

            FadeOut();

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


        private void HideButtonWithAnimation()
        {
            _buttonBehaviourList.ForEach(i => i.FadeOut());
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

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

            HideButton();

            _buttonDebugMenu?.Hide();

            _titleLogoBehaviour.Hide();

            _isShown = false;
        }


        private void HideButton()
        {
            _buttonBehaviourList.ForEach(i => i.Hide());
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            Assert.IsTrue(_isShown);

            HideButtonWithAnimation();
            _buttonDebugMenu?.FadeOut();
            _titleLogoBehaviour.FadeOut();

            _isShown = false;
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            _layouter?.Destroy();

            _menuManager = null;
            _canvas = null;
        }
    }
}