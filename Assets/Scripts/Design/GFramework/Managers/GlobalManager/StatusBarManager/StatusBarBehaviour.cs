using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;

namespace GFramework
{
    public enum EStatusBarUserType
    {
        NORMAL,
        PREMIUM
    }


    public sealed class StatusBarBehaviour : ImageActionBehaviour
    {
        public enum EStatusBarMode
        {
            NORMAL,  // EPlayerType.PLAYER1 のみこの状態になる
            ON_GAME
        }


        // ===============================================
        // MARK: - ● Property  ─────

        // -----------------------------------------------
        // MARK: - Game Object

        private SingleImageActionBehaviour _singleImageActionBehaviourStatusBarBody;

        private Canvas _canvas;

        private TextFieldBehaviour _playerNameTextFieldBehaviour;

        private CStatusBarLifeSet _statusBarLifeSet;

        private EStatusBarUserType _statusBarUserType = EStatusBarUserType.NORMAL;
        private SingleImageActionBehaviour _singleImageActionBehaviourStatusBarEmblem;

        private StatusBarGradeRankIconBehaviour _statusBarGradeRankIconBehaviour;

        private ImageTextBehaviour _ratePlayer1ImageTextBehaviour;

        // 残り時間表示
        private ImageTextBehaviour _timeLeftImageTextBehaviour;



        private EPlayerType _playerType = EPlayerType.PLAYER1;
        private EStatusBarMode _statusBarMode = EStatusBarMode.NORMAL;


        private string _playerName = "";
        public string PlayerName => _playerName;

        private bool _isShown = true;



        // TODO: こちらは削除したい。もっと上の action のみ保有するクラスを作成してそちらに移管。
        public override float Alpha => throw new NotImplementedException();


        // ===============================================
        // MARK: - ● Setup ─────

        public void SetupComponent(EPlayerType playerType, string playerName)
        {
            _playerType = playerType;
            _playerName = playerName;

            _statusBarMode = playerType switch
            {
                EPlayerType.PLAYER1 => EStatusBarMode.NORMAL,
                EPlayerType.PLAYER2 => EStatusBarMode.ON_GAME,
                _ => throw new InvalidOperationException(),
            };

            SetupGameObject();
            UpdateNameDisplay();
        }


        private void SetupGameObject()
        {
            SetupBody();

            SetupCanvas();

            SetupPlayerName();

            SetupLifeSet();

            SetupEmblem();

            SetupMoveOrder();

            SetupRatePlayer1();

            SetupTimeLeft();


            SetupBaseImageSize();


            SetupPosition();

            Hide();
        }


        private void SetupBody()
        {
            _singleImageActionBehaviourStatusBarBody = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "StatusBarBody",
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForStatusBar.BODY),
                willCacheIfNeeded: true,
                sortingOrder: Config.STATUS_BAR_BODY__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.2f,
                    durationFadeOut: 0.2f)
                );

            var positionByPixel = Locator.Instance.statusBarManager.Layouter.GetAreaStatusBar(playerType: _playerType).CenterPoint;
            _singleImageActionBehaviourStatusBarBody.transform.position = Utl.ConvertImageSizeToUnit(imageSize: positionByPixel);

            Vector2 bodyImageSizeY = Config.STATUS_BAR_BODY__IMAGE_SIZE;
            Vector2 bodyDisplaySizeY = Locator.Instance.statusBarManager.Layouter.GetAreaStatusBar(playerType: _playerType).size;

            _singleImageActionBehaviourStatusBarBody.transform.localScale = bodyDisplaySizeY / bodyImageSizeY;
        }


        private void SetupCanvas()
        {
            var gameObjectWithCanvas = Utl.CreateGameObjectWithCanvas(
                name: "Canvas",
                size: Locator.Instance.layoutManager.AreaUICanvas.size,
                localScale: Locator.Instance.layoutManager.ScaleUICanvas,
                parentTransform: gameObject.transform,
                sortingOrder: Config.STATUS_BAR_TEXT__LAYERED_ORDERID.SortingOrder);

            _canvas = gameObjectWithCanvas.GetComponent<Canvas>();
        }


        private void SetupPlayerName()
        {
            var gameObjectText = new GameObject(name: "Name");
            _playerNameTextFieldBehaviour = gameObjectText.AddComponent<TextFieldBehaviour>() as TextFieldBehaviour;

            EAlignment alignment = _playerType switch
            {
                EPlayerType.PLAYER1 => EAlignment.MIDDLE_LEFT,
                EPlayerType.PLAYER2 => EAlignment.MIDDLE_RIGHT,
                _ => throw new InvalidOperationException(),
            };


            var textInitializer = new FTextFieldInitializer(
                text: _playerName,
                fontType: EFontType.BLOCK,
                fontSize: Locator.Instance.statusBarManager.Layouter.FontSizeStatusBar,
                alignment: alignment,
                area: Locator.Instance.statusBarManager.Layouter.GetAreaPlayerName(playerType: _playerType).AsYReversed(),
                lineSpacing: 1f,
                isOverFlowHorizontal: true,
                isOverFlowVertical: true,
                isResizeTextForBestFit: false,
                color: new FColor3(1, 1, 1),
                defaultAlpha: 1f,
                targetCanvas: _canvas);

            _playerNameTextFieldBehaviour.SetupComponent(textInitializer: textInitializer);
        }


        private void SetupLifeSet()
        {
            if (_playerType != EPlayerType.PLAYER1) { return; }

            _statusBarLifeSet = new CStatusBarLifeSet(transform: transform);
        }


        private void SetupEmblem()
        {
            if (_playerType != EPlayerType.PLAYER1) { return; }

            _singleImageActionBehaviourStatusBarEmblem = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                name: "StatubBarEmblem",
                texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForStatusBar.EMBLEM_PREMIUM),
                willCacheIfNeeded: true,
                sortingOrder: Config.STATUS_BAR_EMBLEM__LAYERED_ORDERID.SortingOrder,
                transformParent: transform,
                initializer: new FSingleImageActionBehaviourInitializer(
                    defaultAlpha: 1,
                    durationFadeIn: 0.2f,
                    durationFadeOut: 0.2f)
                );

            var positionByPixel = Locator.Instance.statusBarManager.Layouter.GetAreaEmblem().CenterPoint;
            _singleImageActionBehaviourStatusBarEmblem.transform.position = Utl.ConvertImageSizeToUnit(imageSize: positionByPixel);

            Vector2 emblemImageSize = Config.STATUS_BAR_EMBLEM__IMAGE_SIZE;
            Vector2 emblemDisplaySize = Locator.Instance.statusBarManager.Layouter.GetAreaEmblem().size;

            var scaleX = emblemDisplaySize.x / emblemImageSize.x;
            _singleImageActionBehaviourStatusBarEmblem.transform.localScale = new Vector2(scaleX, scaleX); 
        }


        private void SetupMoveOrder()
        {
            // セットアップ時は 66 での表示とする
            var gameObjectMoveOrder = new GameObject("MoveOrder");
            _statusBarGradeRankIconBehaviour = gameObjectMoveOrder.AddComponent<StatusBarGradeRankIconBehaviour>();
            gameObjectMoveOrder.transform.parent = transform;
            _statusBarGradeRankIconBehaviour.SetupComponent(playerType: _playerType, boardType: EBoardType.BOARD_66, isOn: true);

            if (_statusBarMode == EStatusBarMode.NORMAL)
            {
                _statusBarGradeRankIconBehaviour.Hide();
            }
        }


        private void SetupRatePlayer1()
        {
            if (_playerType != EPlayerType.PLAYER1) { return; }

            FImageTextInitializer imageTextInitializer = new FImageTextInitializer(
                designID: 0,
                maxLength: 4,
                spacingRatioList: new float[] { 0.07f, 0.07f, 0.07f },
                textAlign: ImageTextBehaviour.ETextAlign.RIGHT,
                anchorDirection: EDirection.RIGHT,
                sortingOrder: Config.STATUS_BAR_RATE_PLAYER2__LAYERED_ORDERID.SortingOrder);

            var gameObjectRatePlayer1 = new GameObject("RateP1");
            gameObjectRatePlayer1.transform.parent = transform;
            _ratePlayer1ImageTextBehaviour = gameObjectRatePlayer1.AddComponent<ImageTextBehaviour>();
            _ratePlayer1ImageTextBehaviour.SetupComponent(imageTextInitializer: imageTextInitializer);


            // Scale の設定
            var scale = Locator.Instance.statusBarManager.Layouter.GetAreaStatusBarMain(playerType: EPlayerType.PLAYER1).size.y / Config.IMAGE_TEXT_CHAR__IMAGE_SIZE.y * 0.70f;
            gameObjectRatePlayer1.transform.localScale = new Vector2(scale, scale);


            // Translation の設定
            Vector2 rightPoint = Locator.Instance.statusBarManager.Layouter.GetAreaStatusBarMain(playerType: EPlayerType.PLAYER1).RightPoint;
            Vector2 position = Utl.ConvertImageSizeToUnit(imageSize: rightPoint);
            gameObjectRatePlayer1.transform.position = position;


            SetRatePlayer1AsBlank();
        }


        public void SetRatePlayer1AsBlank()
        {
            _ratePlayer1ImageTextBehaviour.SetText(text: "");
        }


        public void SetRatePlayer1(int rate)
        {
            string rateAsString;

            if (rate < 0)
            {
                rateAsString = "0";
            }
            else if (rate > 9999)
            {
                rateAsString = "999";
            }
            else
            {
                rateAsString = rate.ToString();
            }

            _ratePlayer1ImageTextBehaviour.SetText(text: rateAsString);
        }


        private void SetupTimeLeft()
        {
            ImageTextBehaviour Support(
                ImageTextBehaviour.ETextAlign textAlign,
                EDirection anchorDirection,
                int sortingOrder,
                Vector2 anchorPoint
                )
            {
                FImageTextInitializer imageTextInitializer = new FImageTextInitializer(
                    designID: 0,
                    maxLength: 5,
                    spacingRatioList: new float[] { 0.11f, -0.11f, -0.11f, 0.11f },
                    textAlign: textAlign,
                    anchorDirection: anchorDirection,
                    sortingOrder: sortingOrder);

                var gameObject = new GameObject("TimeLeft");
                gameObject.transform.parent = transform;
                var resultBehaviour = gameObject.AddComponent<ImageTextBehaviour>();
                resultBehaviour.SetupComponent(imageTextInitializer: imageTextInitializer);


                // Scale の設定
                var scale = Locator.Instance.statusBarManager.Layouter.GetAreaStatusBarMain(playerType: EPlayerType.PLAYER1).size.y / Config.IMAGE_TEXT_CHAR__IMAGE_SIZE.y * 0.725f;
                gameObject.transform.localScale = new Vector2(scale, scale);

                // Translation の設定
                Vector2 position = Utl.ConvertImageSizeToUnit(imageSize: anchorPoint);
                gameObject.transform.position = position;

                return resultBehaviour;
            }



            _timeLeftImageTextBehaviour = _playerType switch
            {
                EPlayerType.PLAYER2 => Support(
                                        textAlign: ImageTextBehaviour.ETextAlign.LEFT,
                                        anchorDirection: EDirection.LEFT,
                                        sortingOrder: Config.STATUS_BAR_TIMELEFT_PLAYER2__LAYERED_ORDERID.SortingOrder,
                                        anchorPoint: Locator.Instance.statusBarManager.Layouter.GetAreaTimeLeft(playerType: _playerType).AsYReversed().LeftPoint),

                EPlayerType.PLAYER1 => Support(
                                        textAlign: ImageTextBehaviour.ETextAlign.RIGHT,
                                        anchorDirection: EDirection.RIGHT,
                                        sortingOrder: Config.STATUS_BAR_TIMELEFT_PLAYER1__LAYERED_ORDERID.SortingOrder,
                                        anchorPoint: Locator.Instance.statusBarManager.Layouter.GetAreaTimeLeft(playerType: _playerType).AsYReversed().RightPoint),

                _ => throw new InvalidOperationException(),
            };

            ClearTimeLeftAsBlank();
        }



        public void ClearTimeLeftAsBlank()
        {
            _timeLeftImageTextBehaviour.SetText(text: "");
        }


        public void SetTimeLeft(int minute, int second)
        {
            _timeLeftImageTextBehaviour.SetText(text: Utl.MinuteAndSecondToString(minute: minute, second: second));

            if (minute == 0)
            {
                _timeLeftImageTextBehaviour.MultipleColor(color3: Locator.Instance.colorManager.GetColor3(colorPreset: EColorPreset.SYSTEM_RED));
            }
            else
            {
                _timeLeftImageTextBehaviour.MultipleColor(color3: Locator.Instance.colorManager.GetColor3(colorPreset: EColorPreset.SYSTEM_WHITE));
            }
        }


        // 残り時刻の代わりに文字を出す(時間制限のないAI対戦の "--:--")。使える文字は数字と " +-:" だけ
        public void SetTimeLeftText(string text)
        {
            _timeLeftImageTextBehaviour.SetText(text: text);
            _timeLeftImageTextBehaviour.MultipleColor(color3: Locator.Instance.colorManager.GetColor3(colorPreset: EColorPreset.SYSTEM_WHITE));
        }


        private void SetupBaseImageSize()
        {
            _baseImageSize = UtlSprite.GetTextureSize(gameObject: _singleImageActionBehaviourStatusBarBody.gameObject);
        }



        // ===============================================
        // MARK: - ● Get Local Position ─────

        public Vector2 GetLocalPosition()
        {
            return transform.localPosition;
        }



        // ===============================================
        // MARK: - ● Set Local Position ─────

        public void SetLocalPositon(float unitY)
        {
            var unit = new Vector2(transform.localPosition.x, unitY);
            Utl.SetLocalPosition(gameObject: gameObject, unit: unit);
        }


        public void SetLocalPositonByRatio(Vector2 ratio)
        {
            Utl.SetLocalPositionByRatio(gameObject: gameObject, baseImageSize: _baseImageSize, ratio: ratio);
        }



        // ===============================================
        // MARK: - ● Setup Position ─────

        private void SetupPosition()
        {
            Vector2 centerPoint = Locator.Instance.layoutManager.AreaBackground.CenterPoint;

            Vector2 position = Utl.ConvertImageSizeToUnit(imageSize: centerPoint);

            transform.position = position;
        }



        // ===============================================
        // MARK: - ● Set Alpha ─────

        public override void SetAlpha(float alpha)
        {
            throw new NotImplementedException();
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public void ChangeStatubBarMode(EStatusBarMode statusBarMode)
        {
            Assert.IsTrue(_playerType == EPlayerType.PLAYER1);
            if (_statusBarMode == statusBarMode) { return; }

            _statusBarMode = statusBarMode;

            switch (statusBarMode)
            {
                case EStatusBarMode.NORMAL:
                    _statusBarGradeRankIconBehaviour.Hide();
                    _ratePlayer1ImageTextBehaviour.Show();
                    _timeLeftImageTextBehaviour.Hide();
                    break;

                case EStatusBarMode.ON_GAME:
                    _statusBarGradeRankIconBehaviour.Show();
                    _ratePlayer1ImageTextBehaviour.Hide();
                    _timeLeftImageTextBehaviour.Show();
                    break;

                default:
                    throw new InvalidOperationException();
            }

        }


        public override void Show()
        {
            if (_isShown) { return; };

            _singleImageActionBehaviourStatusBarBody.Show();
            _playerNameTextFieldBehaviour.Show();

            if (_playerType == EPlayerType.PLAYER1)
            {
                switch (_statusBarUserType)
                {
                    case EStatusBarUserType.NORMAL:
                        _statusBarLifeSet.Show();
                        break;

                    // TODO: Showを実装する確認する
                    case EStatusBarUserType.PREMIUM:
                        _singleImageActionBehaviourStatusBarEmblem.Show();
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                _statusBarLifeSet.Show();
                _ratePlayer1ImageTextBehaviour.Show();
            }
            else // playerType == EPlayerType.PLAYER2
            {
                _statusBarGradeRankIconBehaviour.Show();
                _timeLeftImageTextBehaviour.Show();
            }

            _isShown = true;
        }


        public override void Hide()
        {
            if (_isShown == false) { return; };


            _singleImageActionBehaviourStatusBarBody.Hide();
            _statusBarGradeRankIconBehaviour.Hide();
            _playerNameTextFieldBehaviour.Hide();

            if (_playerType == EPlayerType.PLAYER1)
            {
                switch (_statusBarUserType)
                {
                    case EStatusBarUserType.NORMAL:
                        _statusBarLifeSet.Hide();
                        break;

                    case EStatusBarUserType.PREMIUM:
                        _singleImageActionBehaviourStatusBarEmblem.Hide();
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                _ratePlayer1ImageTextBehaviour.Hide();
            }
            else // playerType == EPlayerType.PLAYER2
            {
                _timeLeftImageTextBehaviour.Hide();
            }


            _isShown = false;
        }


        // 通常、Player1はタイトル画面で、Player2は対局画面で FadeInする。その際、番手／レートの表示が異なる。
        public override void FadeIn(Action actionOnDidFinish = null)
        {
            _singleImageActionBehaviourStatusBarBody.FadeIn();
            _playerNameTextFieldBehaviour.FadeIn();

            if (_playerType == EPlayerType.PLAYER1)
            {
                switch (_statusBarUserType)
                {
                    case EStatusBarUserType.NORMAL:
                        _statusBarLifeSet.FadeIn();
                        break;

                    case EStatusBarUserType.PREMIUM:
                        _singleImageActionBehaviourStatusBarEmblem.FadeIn();
                        break;

                    default:
                        throw new InvalidOperationException();
                }
                
                _ratePlayer1ImageTextBehaviour.FadeIn();
            }
            else // playerType == EPlayerType.PLAYER2
            {
                _statusBarGradeRankIconBehaviour.FadeIn();
                _timeLeftImageTextBehaviour.FadeIn();
            }

            _isShown = true;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            _singleImageActionBehaviourStatusBarBody.FadeOut();
            _playerNameTextFieldBehaviour.FadeOut();

            if (_playerType == EPlayerType.PLAYER1)
            {
                switch (_statusBarUserType)
                {
                    case EStatusBarUserType.NORMAL:
                        _statusBarLifeSet.FadeOut();
                        break;

                    case EStatusBarUserType.PREMIUM:
                        _singleImageActionBehaviourStatusBarEmblem.FadeOut();
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                _ratePlayer1ImageTextBehaviour.FadeOut();
            }
            else // playerType == EPlayerType.PLAYER2
            {
                _statusBarGradeRankIconBehaviour.FadeOut();
                _timeLeftImageTextBehaviour.FadeOut();
            }

            _isShown = false;
        }



        // ===============================================
        // MARK: - ● Life ─────

        public void SetLife(int count)
        {
            _statusBarLifeSet.SetLife(count: count);
        }


        // ===============================================
        // MARK: - ● User Status ─────

        public void SetStatusBarUserType(EStatusBarUserType userType)
        {
            if (_statusBarUserType == userType) { return; }

            _statusBarUserType = userType;

            switch (_statusBarUserType)
            {
                case EStatusBarUserType.NORMAL:
                    _statusBarLifeSet.Show();
                    _singleImageActionBehaviourStatusBarEmblem.Hide();
                    break;

                case EStatusBarUserType.PREMIUM:
                    _statusBarLifeSet.Hide();
                    _singleImageActionBehaviourStatusBarEmblem.Show();
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }
    


        // ===============================================
        // MARK: - ● Get Move Order Type ─────

        public EMoveOrderType GetMoveOrderType()
        {
            return _statusBarGradeRankIconBehaviour.MoveOrderType;
        }



        // ===============================================
        // MARK: - ● Set Player Name ─────

        public void SetPlayerName(string playerName)
        {
            _playerName = playerName;

            if (Locator.Instance.statusBarManager.IsNameShown(playerType: _playerType))
            {
                _playerNameTextFieldBehaviour.SetText(text: playerName);
            }
        }



        // ===============================================
        // MARK: - ● Move Order  ─────

        public void SetMoveOrder(EMoveOrderType moveOrderType, EGradeRankType gradeRankType, EBoardType boardType)
        {
            _statusBarGradeRankIconBehaviour.SetGradeRankIcon(newMoveOrderType: moveOrderType, newGradeRankType: gradeRankType, boardType: boardType);
        }


        public void ShowMoveOrderVisibility(bool isVisible)
        {
            if (isVisible)
            {
                _statusBarGradeRankIconBehaviour.Show();
            }
            else
            {
                _statusBarGradeRankIconBehaviour.Hide();
            }
        }

        public void SetMoveOrderOnOff(bool isOn)
        {
            if (isOn)
            {
                _statusBarGradeRankIconBehaviour.SetOn();
            }
            else
            {
                _statusBarGradeRankIconBehaviour.SetOff();
            }
        }


        // ===============================================
        // MARK: - ● Show / Hide Name ─────

        public void UpdateNameDisplay()
        {
            if (Locator.Instance.statusBarManager.IsNameShown(playerType: _playerType))
            {
                _playerNameTextFieldBehaviour.SetText(_playerName);
            }
            else
            {
                _playerNameTextFieldBehaviour.SetText("ーーー");
            }
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            _canvas = null;

            _singleImageActionBehaviourStatusBarBody = null;

            _playerNameTextFieldBehaviour = null;

            // PLAYRE2 の場合には null になるため、null チェックを行う
            _statusBarLifeSet?.Destroy();

            _singleImageActionBehaviourStatusBarEmblem = null;

            _statusBarGradeRankIconBehaviour = null;

            _ratePlayer1ImageTextBehaviour = null;

            _timeLeftImageTextBehaviour = null;
        }
    }
}