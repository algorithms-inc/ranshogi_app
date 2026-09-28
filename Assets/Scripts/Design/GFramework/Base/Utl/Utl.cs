using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GFramework.Core;
using System.Drawing;


namespace GFramework
{
    public static class Utl
    {
        #region GameObject Destroy
        // GameObjectが複数のbehaviourを持つことができると思われる。複数のbehaviour にて SafeDesotroyをしても大丈夫？
        public static void SafeDestroy(MonoBehaviour behaviour)
        {
            if (behaviour)
            {
                GameObject.Destroy(behaviour.gameObject);
            }
        }

		public static void SafeDestroy(in List<MonoBehaviour> behaviourList) => behaviourList.ForEach(i => SafeDestroy(behaviour: i));

		// Generics 同士の変換はできない様子。個別に作る必要あり。
		public static void SafeDestroy(in List<TextFieldBehaviour> behaviourList) => behaviourList.ForEach(i => SafeDestroy(behaviour: i));

		// Generics 同士の変換はできない様子。個別に作る必要あり。
		public static void SafeDestroy(in List<ButtonBehaviour> behaviourList) => behaviourList.ForEach(i => SafeDestroy(behaviour: i));

		// Generics 同士の変換はできない様子。個別に作る必要あり。
		public static void SafeDestroy(in List<ImageBoxBehaviour> behaviourList) => behaviourList.ForEach(i => SafeDestroy(behaviour: i));

		// Generics 同士の変換はできない様子。個別に作る必要あり。
		public static void SafeDestroy(in List<InputFieldBehaviour> behaviourList) => behaviourList.ForEach(i => SafeDestroy(behaviour: i));

		// Generics 同士の変換はできない様子。個別に作る必要あり。
		public static void SafeDestroy(in List<CheckBoxBehaviour> behaviourList) => behaviourList.ForEach(i => SafeDestroy(behaviour: i));

		// Generics 同士の変換はできない様子。個別に作る必要あり。
		public static void SafeDestroy(in List<RadioButtonGroupBehaviour> behaviourList) => behaviourList.ForEach(i => SafeDestroy(behaviour: i));

		// Generics 同士の変換はできない様子。個別に作る必要あり。
		public static void SafeDestroy(in List<SliderBehaviour> behaviourList) => behaviourList.ForEach(i => SafeDestroy(behaviour: i));

		#endregion


		#region Range

		/// 範囲として正しいか
		public static bool IsInRange<T>(ref T value, ref T min, ref T max) where T : IComparable => (min.CompareTo(value) != 1) && (value.CompareTo(max) != 1);

		#endregion


		#region Screen

		// スクリーンの幅と高さを取得
		public static Vector2 GetScreenSize() => new Vector2(x: Screen.width, y: Screen.height);


		/// スクリーンを取得(原点中央, Yダウン座標系)
		public static FArea GetScreenArea()
        {
            Vector2 size = Utl.GetScreenSize();
            Vector2 origin = size / -2;

            return new FArea(origin: origin, size: size);
        }


        /// スクリーンの短辺÷長辺の比率
        public static float GetScreenRatio()
        {
            float a = Screen.width;
            float b = Screen.height;

            return (a < b) ? a / b : b / a;
        }


        /// スクリーンのセーフエリアを取得(原点中央, Yダウン座標系)
        public static FArea GetScreenSafeArea()
        {
            // 次の２点に注意
            // 1) システム上のセーフエリアは、Yアップ座標系・左下原点になっている
            // 2) セーフエリアはスクリーンの中心にあるとは限らない

            var screenArea = GetScreenArea();

            Rect safeAreaRect = Screen.safeArea;

            Vector2 tempVector = new Vector2(
                x: safeAreaRect.xMin,
                y: screenArea.size.y - safeAreaRect.size.y - safeAreaRect.yMin);

            return new FArea(origin: screenArea.origin + tempVector, size: safeAreaRect.size);
        }

		#endregion


		#region Device

		public static bool IsIPhone() => SystemInfo.deviceModel.Contains("iPhone");

		public static bool IsIPad() => SystemInfo.deviceModel.Contains("iPad");


		/// <summary>
		/// スマートフォンかどうか
		/// </summary>
		/// <returns></returns>
		/// <note>
		/// - iPhoneであるかどうかは区別可能。Android は便宜上画面比で決定。
		/// - スマートフォンである場合、Portrait での上下反転を行わない事が普通であるため、その判定に利用
		/// </note>
		public static bool IsSmartPhone()
        {
            if (IsIPhone())
            {
                return true;
            }
            else
            {
                return (GetScreenRatio() < Config.MIN_THRESHOLD_RATIO_OF_TABLET);
            }
        }

        #endregion

        #region Orientation

        public static void SetScreenOrientation(EScreenOrientation value)
        {
            Screen.orientation = value switch
            {
                EScreenOrientation.PORTRAIT => ScreenOrientation.Portrait,
                EScreenOrientation.PORTRAIT_UPSIDEDOWN => ScreenOrientation.PortraitUpsideDown,
                EScreenOrientation.LANDSCAPE_LEFT => ScreenOrientation.LandscapeLeft,
                EScreenOrientation.LANDSCAPE_RIGHT => ScreenOrientation.LandscapeRight,
                _ => throw new InvalidOperationException(),
            };
        }


		public static DeviceOrientation GetDeviceOrientation() => Input.deviceOrientation;

		#endregion


		#region Camera
		public static Camera GetMainCamera() => Camera.main;

		public static void SetComponentPhysics2DRaycasterToMainCamera()
        {
            Camera mainCamera = GetMainCamera();

            if (mainCamera.gameObject.GetComponent(typeof(Physics2DRaycaster)) as Physics2DRaycaster != null)
            {
                mainCamera.gameObject.AddComponent<Physics2DRaycaster>();
            }
            else
            {
                // Debug.Log("Main Camera has already Component Physics2DRaycaster");
            }
        }


		public static void SetMainCameraSize(float pixelY) => GetMainCamera().orthographicSize = pixelY / 2 / Config.PIXEL_PER_UNIT;


		public static void UpdateMainCameraSize() => SetMainCameraSize(pixelY: GetScreenSize().y);

		#endregion


		#region Tap

		public static bool HasTap() => Input.touchCount > 0;


		// タップされた座標を得る（左下原点、右上スクリーンサイズ）
		// - HasTouch() == false である場合、最後にタップされた場所が返される（起動時は、右上座標か？不定？）
		public static Vector2 GetTapPosition()
        {
            var positionVector3 = Input.mousePosition;

            return new Vector2(x: positionVector3.x, y: positionVector3.y);
        }

        #endregion



        public static GameObject CreateGameObjectWithCanvas(
            string name,
            Vector2 size,
            Vector2 localScale,
            Transform parentTransform,
            int sortingOrder)
        {
            GameObject gameObjectWithCanvas = new GameObject(
                name: name,
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));

            var canvas = gameObjectWithCanvas.GetComponent<Canvas>();

            var rectTransform = gameObjectWithCanvas.GetComponent<RectTransform>();
            rectTransform.sizeDelta = size;

            canvas.sortingOrder = sortingOrder;

            // 他のゲームオブジェクトと同等に扱う設定にする
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;

            gameObjectWithCanvas.transform.localScale = localScale;
            // RectTransform を保有するオブジェクトは transform.parent = と設定するのではなく、SetParent() を用いる必要がある。
            gameObjectWithCanvas.transform.SetParent(parentTransform);

            return gameObjectWithCanvas;
        }


        


        public static TapInputBehaviour CreateTapInputFullScreen(Canvas canvas, bool isEnabled, Action<ETapInputType, int> action)
        {
            var newGameObject = new GameObject("TapInputFullScreen");
            newGameObject.transform.parent = canvas.transform;

            var tapInputFullScreenBehaviour = newGameObject.AddComponent<TapInputBehaviour>();

            tapInputFullScreenBehaviour.SetupComponentForFullScreen(
                isEnabled: isEnabled,
                actionOnPointerClick: action);

            return tapInputFullScreenBehaviour;
        }


        public static TapInputBehaviour CreateTapInputBlocker(Canvas canvas, bool isEnabled)
        {
            var newGameObject = new GameObject("TapInputBlocker");
            newGameObject.transform.parent = canvas.transform;

            var tapInputFullScreenBehaviour = newGameObject.AddComponent<TapInputBehaviour>();

            tapInputFullScreenBehaviour.SetupComponentForFullScreen(
                isEnabled: isEnabled);

            return tapInputFullScreenBehaviour;
        }


		public static float ConvertPixelToUnit(float pixel) => pixel / Config.PIXEL_PER_UNIT;


		public static float ConvertImageSizeToUnit(float imageSize) => imageSize / Config.PIXEL_PER_UNIT;


		public static Vector2 ConvertImageSizeToUnit(Vector2 imageSize) => imageSize / Config.PIXEL_PER_UNIT;


		public static float ConvertUnitToImageSize(float unit) => unit * Config.PIXEL_PER_UNIT;


		public static Vector2 ConvertUnitSizeToImageSize(Vector2 unitSize) => unitSize * Config.PIXEL_PER_UNIT;


		public static Vector2 ConvertRatioToUnit(Vector2 baseImageSize, Vector2 ratio)
        {
            Vector2 localPositionPixel = new Vector2(baseImageSize.x * ratio.x, baseImageSize.y * ratio.y);

            return Utl.ConvertImageSizeToUnit(imageSize: localPositionPixel);
        }



		public static void SetPosition(GameObject gameObject, Vector2 unit) => gameObject.transform.position = unit;

		public static void SetPositionByRatio(GameObject gameObject, Vector2 baseImageSize, Vector2 ratio) => gameObject.transform.position = ConvertRatioToUnit(baseImageSize: baseImageSize, ratio: ratio);


		public static void SetLocalPosition(GameObject gameObject, Vector2 unit) => gameObject.transform.localPosition = unit;

		public static void SetLocalPositionByRatio(GameObject gameObject, Vector2 baseImageSize, Vector2 ratio) => gameObject.transform.localPosition = ConvertRatioToUnit(baseImageSize: baseImageSize, ratio: ratio);



		public static bool CanBeGyokushou(EPieceType pieceType) => pieceType == EPieceType.OUSHOU;

		public static bool CanBeNari(EPieceType pieceType) => (pieceType != EPieceType.OUSHOU && pieceType != EPieceType.KINSHOU);

		public static bool CanBeNariAndNotKeima(EPieceType pieceType)
        {
            if (pieceType == EPieceType.KEIMA) { return false; }

            return CanBeNari(pieceType: pieceType);
        }

		public static bool CanBeOnWaitingSpace(EPieceType pieceType) => !CanBeGyokushou(pieceType: pieceType);



		public static EDirection DecomposedToLR(EDirection direction)
        {
            switch (direction)
            {
                case EDirection.UP_LEFT:   // fall through
                case EDirection.LEFT:      // fall through
                case EDirection.DOWN_LEFT:
                    return EDirection.LEFT;

                case EDirection.UP_RIGHT:  // fall through
                case EDirection.RIGHT:     // fall through
                case EDirection.DOWN_RIGHT:
                    return EDirection.RIGHT;

                case EDirection.UP:        // fall through
                case EDirection.STAY:      // fall through
                case EDirection.DOWN:
                    return EDirection.STAY;

                case EDirection.NULL:
                default:
                    throw new InvalidOperationException();
            }
        }


        public static EDirection DecomposedToUD(EDirection direction)
        {
            switch (direction)
            {
                case EDirection.UP_LEFT:    // fall through
                case EDirection.UP:         // fall through
                case EDirection.UP_RIGHT:
                    return EDirection.UP;

                case EDirection.DOWN_LEFT:  // fall through
                case EDirection.DOWN:       // fall through
                case EDirection.DOWN_RIGHT:
                    return EDirection.DOWN;

                case EDirection.LEFT:       // fall through
                case EDirection.STAY:       // fall through
                case EDirection.RIGHT:
                    return EDirection.STAY;

                case EDirection.NULL:       // fall through
                default:
                    throw new InvalidOperationException();
            }
        }


        public static EPieceType ConvertIndexOnWaitingSpaceToPieceType(int index)
        {
            return index switch
            {
                0 => EPieceType.FUHYOU,
                1 => EPieceType.KYOUSHA,
                2 => EPieceType.KEIMA,
                3 => EPieceType.GINSHOU,
                4 => EPieceType.KINSHOU,
                5 => EPieceType.KAKUGYOU,
                6 => EPieceType.HISHA,
                _ => throw new InvalidOperationException(),
            };
        }


        public static int ConvertPieceTypeToIndexOnWaitingSpace(EPieceType pieceType)
        {
            return pieceType switch
            {
                EPieceType.OUSHOU => throw new InvalidOperationException(),
                EPieceType.HISHA => 6,
                EPieceType.KAKUGYOU => 5,
                EPieceType.KINSHOU => 4,
                EPieceType.GINSHOU => 3,
                EPieceType.KEIMA => 2,
                EPieceType.KYOUSHA => 1,
                EPieceType.FUHYOU => 0,
                _ => throw new InvalidOperationException(),
            };
        }


        public static EPieceSizeType ConvertPieceTypeToPieceSizeType(EPieceType pieceType)
        {
            return pieceType switch
            {
                EPieceType.OUSHOU => EPieceSizeType.SIZE_OUSHOU,
                EPieceType.HISHA => EPieceSizeType.SIZE_HISHA_GROUP,
                EPieceType.KAKUGYOU => EPieceSizeType.SIZE_HISHA_GROUP,
                EPieceType.KINSHOU => EPieceSizeType.SIZE_KIN_GROUP,
                EPieceType.GINSHOU => EPieceSizeType.SIZE_KIN_GROUP,
                EPieceType.KEIMA => EPieceSizeType.SIZE_KEIMA,
                EPieceType.KYOUSHA => EPieceSizeType.SIZE_KYOUSHA,
                EPieceType.FUHYOU => EPieceSizeType.SIZE_FUHYOU,
                _ => throw new InvalidOperationException(),
            };
        }


        public static (EPieceType pieceType, bool isNari) ConvertPieceTypeMainToGFrameWork(
            PieceType pieceTypeMain)
        {
            return pieceTypeMain switch
            {
                PieceType.No_Piece => throw new InvalidOperationException(),
                PieceType.Pawn => (pieceType: EPieceType.FUHYOU, isNari: false),
                PieceType.Lance => (pieceType: EPieceType.KYOUSHA, isNari: false),
                PieceType.Knight => (pieceType: EPieceType.KEIMA, isNari: false),
                PieceType.Silver => (pieceType: EPieceType.GINSHOU, isNari: false),
                PieceType.Bishop => (pieceType: EPieceType.KAKUGYOU, isNari: false),
                PieceType.Rook => (pieceType: EPieceType.HISHA, isNari: false),
                PieceType.Gold => (pieceType: EPieceType.KINSHOU, isNari: false),
                PieceType.King => (pieceType: EPieceType.OUSHOU, isNari: false),
                PieceType.Pro_Pawn => (pieceType: EPieceType.FUHYOU, isNari: true),
                PieceType.Pro_Lance => (pieceType: EPieceType.KYOUSHA, isNari: true),
                PieceType.Pro_Knight => (pieceType: EPieceType.KEIMA, isNari: true),
                PieceType.Pro_Silver => (pieceType: EPieceType.GINSHOU, isNari: true),
                PieceType.Horse => (pieceType: EPieceType.KAKUGYOU, isNari: true),
                PieceType.Dragon => (pieceType: EPieceType.HISHA, isNari: true),
                _ => throw new InvalidOperationException(),
            };
        }


        // deprecated の関数の廃止と合わせてこの関数も廃止すること
        public static List<FPieceAndSquare> ConvertPieceAndSquareTupleToStruct(in List<(PieceType pieceType, Square square)> playerPieceList)
        {
            var resultList = new List<FPieceAndSquare>();

            foreach (var i in playerPieceList)
            {
                resultList.Add(new FPieceAndSquare(pieceType: i.pieceType, square: i.square));
            }

            return resultList;
        }


        public static List<FPiecePlaceData> ConvertPieceTypeMainListToGFrameworkList(
            EMoveOrderType moveOrderTypeOfPlayer1,
            in List<FPieceAndSquare> playerPieceList,
            EBoardType boardType)
        {
            var resultList = new List<FPiecePlaceData>();

            foreach (var i in playerPieceList)
            {
                var temp = ConvertPieceTypeMainToGFrameWork(pieceTypeMain: i.PieceType);

                resultList.Add(
                    new FPiecePlaceData(
                        pieceType: temp.pieceType,
                        isNari: temp.isNari,
                        boardType: boardType,
                        boardCrd: new FBoardCrd(boardType: boardType, square: i.Square, moveOrderType: moveOrderTypeOfPlayer1)));
            };

            return resultList;
        }

        public static PieceType ConvertPieceTypeGFrameWorkToMain(EPieceType pieceType, bool isNari)
        {
            if (isNari == false)
            {
                return pieceType switch
                {
                    EPieceType.OUSHOU => PieceType.King,
                    EPieceType.HISHA => PieceType.Rook,
                    EPieceType.KAKUGYOU => PieceType.Bishop,
                    EPieceType.KINSHOU => PieceType.Gold,
                    EPieceType.GINSHOU => PieceType.Silver,
                    EPieceType.KEIMA => PieceType.Knight,
                    EPieceType.KYOUSHA => PieceType.Lance,
                    EPieceType.FUHYOU => PieceType.Pawn,
                    _ => throw new InvalidOperationException(),
                };
            }
            else
            {
                return pieceType switch
                {
                    EPieceType.OUSHOU => throw new InvalidOperationException(),
                    EPieceType.HISHA => PieceType.Dragon,
                    EPieceType.KAKUGYOU => PieceType.Horse,
                    EPieceType.KINSHOU => throw new InvalidOperationException(),
                    EPieceType.GINSHOU => PieceType.Pro_Silver,
                    EPieceType.KEIMA => PieceType.Pro_Knight,
                    EPieceType.KYOUSHA => PieceType.Pro_Lance,
                    EPieceType.FUHYOU => PieceType.Pro_Pawn,
                    _ => throw new InvalidOperationException(),
                };
            }
        }


        public static EPlayerType ConvertToOppositePlayerType(EPlayerType playerType)
        {
            return playerType switch
            {
                EPlayerType.PLAYER1 => EPlayerType.PLAYER2,
                EPlayerType.PLAYER2 => EPlayerType.PLAYER1,
                _ => throw new InvalidOperationException(),
            };
        }



        public static Piece ConvertPieceTypeFrameworkToPieceMain(EPlayerType playerType, EMoveOrderType moveOrderTypeOfPlayer1, EPieceType pieceType, bool isNari)
        {
            bool isBlack;

            if ((playerType == EPlayerType.PLAYER1) && (moveOrderTypeOfPlayer1 == EMoveOrderType.FIRST)
                || (playerType == EPlayerType.PLAYER2) && (moveOrderTypeOfPlayer1 == EMoveOrderType.SECOND))
            {
                isBlack = true;
            }
            else
            {
                isBlack = false;
            }


            if (isBlack)
            {
                if (isNari)
                {
                    return pieceType switch
                    {
                        EPieceType.OUSHOU => throw new InvalidOperationException(),
                        EPieceType.HISHA => Piece.B_Dragon,
                        EPieceType.KAKUGYOU => Piece.B_Horse,
                        EPieceType.KINSHOU => throw new InvalidOperationException(),
                        EPieceType.GINSHOU => Piece.B_Pro_Silver,
                        EPieceType.KEIMA => Piece.B_Pro_Knight,
                        EPieceType.KYOUSHA => Piece.B_Pro_Lance,
                        EPieceType.FUHYOU => Piece.B_Pro_Pawn,
                        _ => throw new InvalidOperationException(),
                    };
                }
                else
                {
                    return pieceType switch
                    {
                        EPieceType.OUSHOU => Piece.B_King,
                        EPieceType.HISHA => Piece.B_Rook,
                        EPieceType.KAKUGYOU => Piece.B_Bishop,
                        EPieceType.KINSHOU => Piece.B_Gold,
                        EPieceType.GINSHOU => Piece.B_Silver,
                        EPieceType.KEIMA => Piece.B_Knight,
                        EPieceType.KYOUSHA => Piece.B_Lance,
                        EPieceType.FUHYOU => Piece.B_Pawn,
                        _ => throw new InvalidOperationException(),
                    };
                }
            }
            else
            {
                if (isNari)
                {
                    return pieceType switch
                    {
                        EPieceType.OUSHOU => throw new InvalidOperationException(),
                        EPieceType.HISHA => Piece.W_Dragon,
                        EPieceType.KAKUGYOU => Piece.W_Horse,
                        EPieceType.KINSHOU => throw new InvalidOperationException(),
                        EPieceType.GINSHOU => Piece.W_Pro_Silver,
                        EPieceType.KEIMA => Piece.W_Pro_Knight,
                        EPieceType.KYOUSHA => Piece.W_Pro_Lance,
                        EPieceType.FUHYOU => Piece.W_Pro_Pawn,
                        _ => throw new InvalidOperationException(),
                    };
                }
                else
                {
                    return pieceType switch
                    {
                        EPieceType.OUSHOU => Piece.W_King,
                        EPieceType.HISHA => Piece.W_Rook,
                        EPieceType.KAKUGYOU => Piece.W_Bishop,
                        EPieceType.KINSHOU => Piece.W_Gold,
                        EPieceType.GINSHOU => Piece.W_Silver,
                        EPieceType.KEIMA => Piece.W_Knight,
                        EPieceType.KYOUSHA => Piece.W_Lance,
                        EPieceType.FUHYOU => Piece.W_Pawn,
                        _ => throw new InvalidOperationException(),
                    };
                }
            }
        }


        public static EMoveOrderType ConvertColorToMoveOrderType(Color color)
        {
            if (color == Color.Black)
            {
                return EMoveOrderType.FIRST;
            }
            else
            {
                return EMoveOrderType.SECOND;
            }
        }


        public static Color ConvertMoveOrderTypeToColor(EMoveOrderType moveOrderType)
        {
            if (moveOrderType == EMoveOrderType.FIRST)
            {
                return Color.Black;
            }
            else
            {
                return Color.White;
            }
        }


        public static EMoveOrderType ReverseMoveOrderType(EMoveOrderType moveOrderType)
        {
            return moveOrderType switch
            {
                EMoveOrderType.FIRST => EMoveOrderType.SECOND,
                EMoveOrderType.SECOND => EMoveOrderType.FIRST,
                _ => throw new InvalidOperationException(),
            };
        }


        public static List<FBoardCrd> ConvertSquareListToBoardCrdList(EBoardType boardType, in List<Square> squareList, EMoveOrderType moveOrderType)
        {
            List<FBoardCrd> resultList = new List<FBoardCrd>();

            squareList.ForEach(i => resultList.Add(new FBoardCrd(boardType: boardType, square: i, moveOrderType: moveOrderType)));

            return resultList;
        }


        public static string MinuteAndSecondToString(int minute, int second)
        {
            String minuteAsString = minute.ToString();

            if (minute <= 9)
            {
                minuteAsString = " " + minuteAsString;
            }


            String secondAsString = second.ToString();

            if (second <= 9)
            {
                secondAsString = "0" + secondAsString;
            }

            return minuteAsString + ":" + secondAsString;
        }


        public static void SetDontDestroyOnLoad(GameObject targetObject)
        {
            UnityEngine.Object.DontDestroyOnLoad(targetObject);
        }


        // 将来的には、画面のテイストに合わせて色を変更できるように、別マネジャに切り出したい
        public static FColor3 ConvertUIColorToColor3(EWindowTextColorType textColorType)
        {
            return textColorType switch
            {
                EWindowTextColorType.NORMAL => new FColor3(r: 0f, g: 0f, b: 0f),
                EWindowTextColorType.CAUTION => new FColor3(r: 0.8f, g: 0f, b: 0.2f),
                EWindowTextColorType.WARNING => new FColor3(r: 1f, g: 0f, b: 0f),
                EWindowTextColorType.LINK => new FColor3(r: 0f, g: 0f, b: 1f),
                EWindowTextColorType.DISABLED => new FColor3(r: 0.5f, g: 0.5f, b: 0.5f),
                _ => throw new NotImplementedException(),
            };
        }


        public static TextAnchor ConvertToTextAnchor(EAlignment alignment)
        {
            return alignment switch
            {
                EAlignment.TOP_LEFT => TextAnchor.UpperLeft,
                EAlignment.TOP_CENTER => TextAnchor.UpperCenter,
                EAlignment.TOP_RIGHT => TextAnchor.UpperRight,
                EAlignment.MIDDLE_LEFT => TextAnchor.MiddleLeft,
                EAlignment.MIDDLE_CENTER => TextAnchor.MiddleCenter,
                EAlignment.MIDDLE_RIGHT => TextAnchor.MiddleRight,
                EAlignment.BOTTOM_LEFT => TextAnchor.LowerLeft,
                EAlignment.BOTTOM_CENTER => TextAnchor.LowerCenter,
                EAlignment.BOTTOM_RIGHT => TextAnchor.LowerRight,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
