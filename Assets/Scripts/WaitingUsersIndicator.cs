using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using GFramework;
using GFramework.Core;

/// <summary>
/// 対局待機中のユーザがいるとき、iPhoneのプッシュ通知のように画面上部から降りてくるバナーを出す。
/// シーンをまたいで1つだけ存在し、マッチングサーバの待機人数APIを定期的に取得する。
/// 人間と対局している間(対局画面でAI対戦でないとき)は出さない。
///
/// - 上にスワイプすると消える。消した後は、待機人数が一度0に戻るまで出し直さない。
/// - 「対局する」を押すとマッチング(対局画面)へ移る。AI対戦中でも確認せずに移る。
/// - バナーの下にあるボタンが一緒に反応しないよう、バナーの範囲はタップ入力をふさぐ。
/// </summary>
public class WaitingUsersIndicator : MonoBehaviour
{
    private const string MessageText = "対局待機中のユーザがいます";
    private const string PlayButtonText = "対局する";

    // 待機人数を取得する間隔(秒)。待機中のユーザはすぐにマッチするか抜けるので短めにする
    private const float PollIntervalSeconds = 2f;

    // 降りてくる・戻るのに掛ける時間(秒)
    private const float SlideDurationSeconds = 0.25f;

    // 上にこれだけ動かしたら消す(バナーの高さに対する比)
    private const float SwipeToDismissRatio = 0.3f;

    // これ以下の動きはタップとみなす(バナーの高さに対する比)
    private const float TapMoveRatio = 0.12f;

    // 見た目を確認するために、待機人数に関係なく出しっぱなしにする(確認用。ふだんは false)
    private const bool AlwaysShowForDesignCheck = false;

    // 下地の色(紺色)
    private static readonly UnityEngine.Color BodyColor = new UnityEngine.Color(r: 0.11f, g: 0.16f, b: 0.35f, a: 1f);

    private static WaitingUsersIndicator _instance;

    private SingleImageActionBehaviour _body;
    private SingleImageActionBehaviour _playButtonBody;
    private TextFieldBehaviour _text;
    private TextFieldBehaviour _playButtonTextField;
    private TapInputBehaviour _tapInput;
    private Canvas _canvas;

    private enum EState
    {
        HIDDEN,       // 画面外
        SLIDING_IN,   // 降りてくる途中
        SHOWN,        // 表示中(指で動かしている間も含む)
        SLIDING_OUT,  // 画面外へ戻る途中
    }

    private EState _state = EState.HIDDEN;

    // 0=画面外(上), 1=表示位置
    private float _slideRatio = 0f;

    // 指で上に引っ張っている量(ピクセル。0以上)
    private float _dragUpPixel = 0f;
    private bool _isDragging = false;
    private Vector2 _dragStartScreenPosition = Vector2.zero;

    // 直近に取得した、AI・管理者を除いた待機人数
    private int _waitingCount = 0;

    // スワイプで消したか。待機人数が0に戻るまで出し直さない
    private bool _isDismissedByUser = false;

    // 「対局する」を押してシーンを移している最中
    private bool _isNavigating = false;

    private Vector2 _screenSizeOnLayout = Vector2.zero;
    private FArea _bannerArea = FArea.ZERO;

    // 下地に使っている画像の大きさ(無地を伸ばして使うので、拡大率の計算に使う)
    private Vector2 _bodyTextureSize = Vector2.one;


    /// <summary>まだなければ作る。AppManager.Setup() の後に呼ぶこと。</summary>
    public static void EnsureCreated()
    {
        if (_instance != null) return;

        var gameObject = new GameObject(name: "WaitingUsersIndicator");
        DontDestroyOnLoad(gameObject);
        _instance = gameObject.AddComponent<WaitingUsersIndicator>();
        _instance.Setup();
    }


    // ===============================================
    // MARK: - ● 組み立て ─────

    private void Setup()
    {
        // ルートは画面中央に置き、子の位置は画面中央からの相対(Yアップ・ピクセル)で指定する
        transform.position = Utl.ConvertImageSizeToUnit(imageSize: Locator.Instance.layoutManager.AreaBackground.CenterPoint);

        _bannerArea = CalcBannerArea();
        _screenSizeOnLayout = Utl.GetScreenSize();

        // ウィンドウより手前に出す(通知なので、何が出ていても一番上に見せる)
        var baseSortingOrder = Config.BACKGROUND_EDGE__LAYERED_ORDERID.SortingOrder;

        // 下地は無地(8x8の白い画像)を伸ばして使う。
        // ウィンドウの画像は枠の模様が入っていて、バナーのように平たく伸ばすと線が出てしまう。
        _body = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
            name: "WaitingUsersIndicatorBody",
            texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(textureType: ETextureTypeForCurtain.WHITE),
            willCacheIfNeeded: true,
            sortingOrder: baseSortingOrder,
            transformParent: transform,
            initializer: new FSingleImageActionBehaviourInitializer(
                defaultAlpha: 1,
                durationFadeIn: 0.2f,
                durationFadeOut: 0.2f));

        _bodyTextureSize = UtlSprite.GetTextureSize(gameObject: _body.gameObject);

        // 白い文字が読めるよう、紺色に着色する。
        // スプライト側の色を掛けるだけなので、同じ画像を使う他の表示には影響しない。
        // Color は将棋の手番の型なので、色は UnityEngine.Color と書く
        _body.gameObject.GetComponent<SpriteRenderer>().color = BodyColor;

        _playButtonBody = UtlSprite.CreateGameObjectWithResizableSpriteAndSingleImageBehaviour(
            name: "WaitingUsersIndicatorPlayButton",
            texturePathUnderResources: Locator.Instance.textureManager.GetFilePath(
                buttonImageType: EButtonImageType.BUTTON_RESIZABLE_POSITIVE,
                buttonStatusType: EButtonStatusType.DEFAULT),
            willCacheIfNeeded: true,
            sortingOrder: baseSortingOrder + 1,
            transformParent: transform,
            border: new Vector4(x: 64f, y: 32f, z: 64f, w: 32f),
            initializer: new FSingleImageActionBehaviourInitializer(
                defaultAlpha: 1,
                durationFadeIn: 0.2f,
                durationFadeOut: 0.2f));

        var gameObjectWithCanvas = Utl.CreateGameObjectWithCanvas(
            name: "Canvas",
            size: Locator.Instance.layoutManager.AreaUICanvas.size,
            localScale: Locator.Instance.layoutManager.ScaleUICanvas,
            parentTransform: transform,
            sortingOrder: baseSortingOrder + 2);
        _canvas = gameObjectWithCanvas.GetComponent<Canvas>();

        _text = CreateTextField(name: "WaitingUsersIndicatorText", text: MessageText, alignment: EAlignment.MIDDLE_LEFT);
        _playButtonTextField = CreateTextField(name: "WaitingUsersIndicatorPlayButtonText", text: PlayButtonText, alignment: EAlignment.MIDDLE_CENTER);

        // バナーの下にあるボタンが一緒に反応しないよう、バナーの範囲でタップ入力を受け止める
        var gameObjectTapInput = new GameObject(name: "WaitingUsersIndicatorTapInput");
        gameObjectTapInput.transform.parent = _canvas.transform;
        _tapInput = gameObjectTapInput.AddComponent<TapInputBehaviour>();
        _tapInput.SetupComponent(
            size: _bannerArea.size,
            position: _bannerArea.CenterPoint,
            isEnabled: false,
            actionOnPointerDown: (tapInputType, durationFrame) => { OnPointerDown(); },
            actionOnPointerUp: (tapInputType, durationFrame) => { OnPointerUp(); });

        ApplyLayout();
        HideParts();

        StartCoroutine(PollLoop());
    }


    private TextFieldBehaviour CreateTextField(string name, string text, EAlignment alignment)
    {
        var textFieldBehaviour = new GameObject(name: name).AddComponent<TextFieldBehaviour>();

        textFieldBehaviour.SetupComponent(textInitializer: new FTextFieldInitializer(
            text: text,
            fontType: EFontType.SERIF,
            fontSize: 20,
            alignment: alignment,
            area: _bannerArea,
            lineSpacing: 1f,
            isOverFlowHorizontal: true,
            isOverFlowVertical: true,
            isResizeTextForBestFit: false,
            color: new FColor3(1, 1, 1),
            defaultAlpha: 1f,
            targetCanvas: _canvas));

        return textFieldBehaviour;
    }


    // ===============================================
    // MARK: - ● 位置と大きさ ─────

    // バナーの表示位置(画面上部)。セーフエリアの上端から少し下げる。
    // 座標は画面中央原点・Yアップ(スプライト・テキストの位置にそのまま使える)。
    private static FArea CalcBannerArea()
    {
        var safeArea = Locator.Instance.layoutManager.AreaSafeAreaWithoutStatusBar;

        var width = safeArea.size.x * 0.94f;
        var height = Mathf.Min(Locator.Instance.layoutManager.ButtonBaseHeight * 1.4f, safeArea.size.y * 0.15f);

        var topY = safeArea.CenterPoint.y + (safeArea.size.y / 2f);
        var centerY = topY - (height / 2f) - (height * 0.15f);

        return new FArea(
            p1: new Vector2(safeArea.CenterPoint.x, centerY),
            p2: new Vector2(width, height),
            areaConstructorType: FArea.EAreaConstructorType.CENTER_SIZE);
    }


    // 「対局する」ボタンの位置(バナーの右側)
    private FArea CalcPlayButtonArea(FArea bannerArea)
    {
        var padding = bannerArea.size.y * 0.16f;
        var height = bannerArea.size.y - (padding * 2f);
        var width = Mathf.Min(bannerArea.size.x * 0.26f, height * 3.0f);

        var centerX = bannerArea.CenterPoint.x + (bannerArea.size.x / 2f) - (width / 2f) - padding;

        return new FArea(
            p1: new Vector2(centerX, bannerArea.CenterPoint.y),
            p2: new Vector2(width, height),
            areaConstructorType: FArea.EAreaConstructorType.CENTER_SIZE);
    }


    // 画面外に隠すときのずらし量(上方向)
    private float CalcHiddenOffsetPixel()
    {
        return _bannerArea.size.y * 1.5f;
    }


    // 表示位置からの、いまのずらし量(上方向)
    private float CalcOffsetUpPixel()
    {
        return (CalcHiddenOffsetPixel() * (1f - _slideRatio)) + _dragUpPixel;
    }


    private void ApplyLayout()
    {
        var offsetUp = new Vector2(0f, CalcOffsetUpPixel());

        var bannerArea = new FArea(
            p1: _bannerArea.CenterPoint + offsetUp,
            p2: _bannerArea.size,
            areaConstructorType: FArea.EAreaConstructorType.CENTER_SIZE);
        var playButtonArea = CalcPlayButtonArea(bannerArea: bannerArea);

        _body.transform.localPosition = Utl.ConvertImageSizeToUnit(imageSize: bannerArea.CenterPoint);
        _body.transform.localScale = bannerArea.size / _bodyTextureSize;

        _playButtonBody.transform.localPosition = Utl.ConvertImageSizeToUnit(imageSize: playButtonArea.CenterPoint);
        _playButtonBody.gameObject.GetComponent<SpriteRenderer>().size = Utl.ConvertImageSizeToUnit(imageSize: playButtonArea.size);

        // 文章はボタンの左側に置く。文字を大きく見せたいので左右の余白は狭めにする
        var padding = bannerArea.size.y * 0.16f;
        var textWidth = (playButtonArea.CenterPoint.x - (playButtonArea.size.x / 2f)) - (bannerArea.CenterPoint.x - (bannerArea.size.x / 2f)) - (padding * 2f);
        var textCenterX = bannerArea.CenterPoint.x - (bannerArea.size.x / 2f) + padding + (textWidth / 2f);

        _text.SetSize(size: new Vector2(textWidth, bannerArea.size.y));
        _text.SetLocalPosition(position: new Vector2(textCenterX, bannerArea.CenterPoint.y));
        // 1行に収まる範囲でできるだけ大きくする(文字数が多いので、たいていは幅で決まる)
        _text.SetFontSize(size: (int)Mathf.Min(bannerArea.size.y * 0.46f, textWidth / (MessageText.Length + 0.1f)));

        _playButtonTextField.SetSize(size: playButtonArea.size);
        _playButtonTextField.SetLocalPosition(position: playButtonArea.CenterPoint);
        _playButtonTextField.SetFontSize(size: (int)Mathf.Min(playButtonArea.size.y * 0.5f, playButtonArea.size.x / (PlayButtonText.Length + 1f)));

        _tapInput.SetSize(size: bannerArea.size);
        _tapInput.SetPosition(position: bannerArea.CenterPoint);
    }


    // 画面の回転などで大きさが変わったら置き直す
    private void UpdateLayoutIfScreenSizeChanged()
    {
        var screenSize = Utl.GetScreenSize();
        if (screenSize == _screenSizeOnLayout) return;

        _screenSizeOnLayout = screenSize;
        _bannerArea = CalcBannerArea();
    }


    // ===============================================
    // MARK: - ● 表示・非表示 ─────

    private void ShowParts()
    {
        _body.Show();
        _playButtonBody.Show();
        _text.Show();
        _playButtonTextField.Show();
        _tapInput.SetIsEnabled(value: true);
    }


    private void HideParts()
    {
        _body.Hide();
        _playButtonBody.Hide();
        _text.Hide();
        _playButtonTextField.Hide();
        _tapInput.SetIsEnabled(value: false);
    }


    private static bool IsGameScene()
    {
        return SceneManager.GetActiveScene().name == "Game";
    }

    // 人間と対局中(対局画面で、AI対戦ではない)
    private static bool IsPlayingWithHuman()
    {
        return IsGameScene()
            && GameManager.instance != null
            && GameManager.instance.globalVariables.aiGame == null;
    }


    // バナーを出してよいか
    private bool ShouldShow()
    {
        if (AlwaysShowForDesignCheck) {
            return !_isNavigating;
        }

        return _waitingCount > 0
            && !_isDismissedByUser
            && !_isNavigating
            && !IsPlayingWithHuman();
    }


    private void Update()
    {
        UpdateLayoutIfScreenSizeChanged();

        // 指を離した判定がこぼれることがあるので、入力が無くなったらここでも終わらせる
        if (_isDragging && !Utl.HasTap() && !Input.GetMouseButton(0)) {
            OnPointerUp();
        }

        // 指で上になぞっている間は、バナーも一緒に上へ動かす(下へは動かさない)
        if (_isDragging) {
            _dragUpPixel = Mathf.Max(0f, GetPointerScreenPosition().y - _dragStartScreenPosition.y);
        }

        var shouldShow = ShouldShow();

        switch (_state)
        {
            case EState.HIDDEN:
                if (shouldShow) {
                    _bannerArea = CalcBannerArea();
                    _dragUpPixel = 0f;
                    _state = EState.SLIDING_IN;
                    ShowParts();
                }
                break;

            case EState.SLIDING_IN:
                _slideRatio = Mathf.Min(1f, _slideRatio + (Time.unscaledDeltaTime / SlideDurationSeconds));
                if (!shouldShow) {
                    _state = EState.SLIDING_OUT;
                } else if (_slideRatio >= 1f) {
                    _state = EState.SHOWN;
                }
                break;

            case EState.SHOWN:
                if (!shouldShow) {
                    _state = EState.SLIDING_OUT;
                }
                break;

            case EState.SLIDING_OUT:
                _slideRatio = Mathf.Max(0f, _slideRatio - (Time.unscaledDeltaTime / SlideDurationSeconds));
                if (_slideRatio <= 0f) {
                    _state = EState.HIDDEN;
                    _dragUpPixel = 0f;
                    HideParts();
                } else if (shouldShow && !_isDragging) {
                    _state = EState.SLIDING_IN;
                }
                break;
        }

        if (_state != EState.HIDDEN) {
            ApplyLayout();
        }
    }


    // ===============================================
    // MARK: - ● 指の操作 ─────

    private static Vector2 GetPointerScreenPosition()
    {
        return Utl.HasTap() ? Utl.GetTapPosition() : (Vector2)Input.mousePosition;
    }


    private void OnPointerDown()
    {
        if (_state == EState.HIDDEN || _isNavigating) return;

        _isDragging = true;
        _dragStartScreenPosition = GetPointerScreenPosition();
        _dragUpPixel = 0f;
    }


    private void OnPointerUp()
    {
        if (!_isDragging) return;
        _isDragging = false;

        var screenPosition = GetPointerScreenPosition();
        var moveUpPixel = screenPosition.y - _dragStartScreenPosition.y;

        // 上に大きく動かした → 消す(確認中はすぐにまた降りてくる)
        if (moveUpPixel >= _bannerArea.size.y * SwipeToDismissRatio) {
            _isDismissedByUser = true;

            // 指で動かした分だけ進んだ状態から、続けて画面外へ戻す
            _slideRatio = Mathf.Clamp01(_slideRatio - (_dragUpPixel / CalcHiddenOffsetPixel()));
            _dragUpPixel = 0f;
            _state = EState.SLIDING_OUT;
            return;
        }

        _dragUpPixel = 0f;

        // ほとんど動いていない → タップとして扱う
        var isTap = Mathf.Abs(moveUpPixel) <= _bannerArea.size.y * TapMoveRatio
            && Mathf.Abs(screenPosition.x - _dragStartScreenPosition.x) <= _bannerArea.size.y * TapMoveRatio;

        if (isTap && IsScreenPositionOnPlayButton(screenPosition: screenPosition)) {
            OnTapPlayButton();
        }
    }


    // タップ位置(スクリーン座標・左下原点)が「対局する」ボタンの中か
    private bool IsScreenPositionOnPlayButton(Vector2 screenPosition)
    {
        var screenSize = Utl.GetScreenSize();

        // 画面中央原点・Yアップの座標に直す
        var point = new Vector2(
            screenPosition.x - (screenSize.x / 2f),
            screenPosition.y - (screenSize.y / 2f));

        var bannerArea = new FArea(
            p1: _bannerArea.CenterPoint + new Vector2(0f, CalcOffsetUpPixel()),
            p2: _bannerArea.size,
            areaConstructorType: FArea.EAreaConstructorType.CENTER_SIZE);
        var playButtonArea = CalcPlayButtonArea(bannerArea: bannerArea);

        return Mathf.Abs(point.x - playButtonArea.CenterPoint.x) <= (playButtonArea.size.x / 2f)
            && Mathf.Abs(point.y - playButtonArea.CenterPoint.y) <= (playButtonArea.size.y / 2f);
    }


    // 開いているウィンドウを閉じる。
    // ウィンドウはシーンをまたいで1つずつ存在するので、開いたまま画面を移ると次の画面にも残ってしまう。
    private static void HideOpenWindows()
    {
        var uiCreator = AppManager.Instance.UICreator;

        foreach (var window in new[] { uiCreator.WindowTop, uiCreator.WindowNormal, uiCreator.WindowConfirmation, uiCreator.WindowError })
        {
            if (window.IsShown()) { window.Hide(onDidHideAction: null); }
        }
    }


    // その場で消す。待機人数が一度0に戻るまでは出し直さない
    private void DismissImmediately()
    {
        _isDismissedByUser = true;
        _isDragging = false;
        _dragUpPixel = 0f;
        _slideRatio = 0f;
        _state = EState.HIDDEN;

        HideParts();
    }


    // ===============================================
    // MARK: - ● 「対局する」 ─────

    // 「対局する」を押したとき。どの画面から押されても、残った表示を片付けてからマッチングへ移る。
    private void OnTapPlayButton()
    {
        if (_isNavigating) return;
        _isNavigating = true;

        Locator.Instance.audioManager.Play(SEType: ESEType.TAP_NORMAL);

        // 画面が変わるので、アニメーションを待たずにその場で消す。
        // 消したことにしておかないと、待機者がいる限りすぐにまた降りてきてしまう。
        DismissImmediately();

        // ウィンドウはシーンをまたいで残るので、どの画面から来ても閉じる
        HideOpenWindows();

        GoToMatching();

        _isNavigating = false;
    }


    // マッチングへ移る。画面ごとの後始末は、アプリが元から持っている道すじに任せる。
    // - メニュー: 「乱将棋で対局」と同じ処理を呼ぶ(残りの対局数・広告の確認つき)
    // - 対局画面: AI対戦をやめる処理(エンジンを止める)を通してからメニューへ
    // - それ以外(成績・設定・友達と対局・対局履歴・検討など): いまのステージを片付けてメニューへ
    // メニュー以外から来たときは、メニューに着いた時点で対局を始めてもらう。
    private static void GoToMatching()
    {
        var menuViewController = FindFirstObjectByType<MenuViewController>();
        if (menuViewController != null) {
            menuViewController.OnTapButtonGame66();
            return;
        }

        GameManager.instance.globalVariables.shouldStartMatchingOnMenu = true;

        var gameController = FindFirstObjectByType<GameController>();
        if (gameController != null) {
            gameController.LeaveAiGameForMatching();
            return;
        }

        AppManager.Instance.DestroyStageAndLoadScene(nextSceneName: "Menu");
    }


    // ===============================================
    // MARK: - ● 待機人数の取得 ─────

    private IEnumerator PollLoop()
    {
        while (true)
        {
            // 人間と対局中は表示しないので取得もしない
            if (!IsPlayingWithHuman()) {
                Api.GetMatchingUsersCount(new MatchingUsersCountRequest(), (response) => {
                    // AIのアカウントも待機列に入るので除く(古いサーバは ai_count を返さない → 0)。
                    // 管理者のアカウントは人なので、そのまま数える
                    SetWaitingCount(count: response.count - response.ai_count);
                }, (errorMessage) => {
                    // 取れなかったときは表示を消すだけにする(ユーザには通知しない)
                    SetWaitingCount(count: 0);
                });
            }
            yield return new WaitForSecondsRealtime(PollIntervalSeconds);
        }
    }


    private void SetWaitingCount(int count)
    {
        _waitingCount = count;

        // 待機者がいなくなったら、次に現れたときはまた通知する
        if (count <= 0) {
            _isDismissedByUser = false;
        }
    }
}
