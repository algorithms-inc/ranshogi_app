using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


namespace GFramework
{
    public static class Config
    {
        /// <summary>
        /// 1 Unit あたりのピクセル数
        /// </summary>
        /// <remarks>
        /// - この値は Unity におけるテクスチャのデフォルトの値であるため、100 から変更しないようにすること
        /// - ピクセルと画面のサイズを一致させるためには、この値を 1 にしたうえで、すべてのテクスチャの同設定を 1 にする必要がある。
        /// </remarks>
        public const int PIXEL_PER_UNIT = 100;



        // ===============================================
        // MARK: - ● フォント ─────

        /// <summary>
        /// セリフ体フォント
        /// </summary>
        public const string FONT_FILENAME__SERIF = "NotoSerifJP-Medium";

        /// <summary>
        /// ブロック体フォント
        /// </summary>
        public const string FONT_FILENAME__BLOCK = "NotoSansJP-Medium";



        // ===============================================
        // MARK: - ● 画像テキスト ─────

        /// <summary>
        /// 画像テキスト１文字の画像サイズ
        /// </summary>

        public static readonly Vector2 IMAGE_TEXT_CHAR__IMAGE_SIZE = new(70, 92);



        // ===============================================
        // MARK: - ● アニメーション ─────

        public const float ANIMATION_DURATION_0 = 0.1f;
        public const float ANIMATION_DURATION_1 = 0.2f;
        public const float ANIMATION_DURATION_2 = 0.3f;
        public const float ANIMATION_DURATION_3 = 0.4f;
        public const float ANIMATION_DURATION_4 = 0.5f;
        public const float ANIMATION_DURATION_5 = 0.6f;
        public const float ANIMATION_DURATION_10 = 1.0f;
        public const float ANIMATION_DURATION_99 = 3f;


        // ===============================================
        // MARK: - ● 表示順 ─────

        #region LAYERER ORDERID

        public const int ORDERID_GAP_BETWEEN_LAYER = 1000;

        #endregion


        // -----------------------------------------------
        // MARK: - for STATUSBAR

        #region STATUSBAR

        public static int LifeMax = 3;

        #endregion


        // -----------------------------------------------
        // MARK: - for BACKGROUND

        #region BACKGROUND

        public static FLayeredOrderID BASE_LAYERED_ORDERID_BACKGROUND = new(layerType: ELayerType.BACKGROUND, orderID: 0);

        /// <summary>
        /// 背景の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID BACKGROUND_BODY_A__LAYERED_ORDERID = BASE_LAYERED_ORDERID_BACKGROUND;
        public static readonly FLayeredOrderID BACKGROUND_BODY_B__LAYERED_ORDERID = BASE_LAYERED_ORDERID_BACKGROUND.Increment();

        /// <summary>
        /// 背景の波紋の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID BACKGROUND_RIPPLE_A__LAYERED_ORDERID = BASE_LAYERED_ORDERID_BACKGROUND.Increment();
        public static readonly FLayeredOrderID BACKGROUND_RIPPLE_B__LAYERED_ORDERID = BASE_LAYERED_ORDERID_BACKGROUND.Increment();
        public static readonly FLayeredOrderID BACKGROUND_RIPPLE_C__LAYERED_ORDERID = BASE_LAYERED_ORDERID_BACKGROUND.Increment();
        public static readonly FLayeredOrderID BACKGROUND_RIPPLE_D__LAYERED_ORDERID = BASE_LAYERED_ORDERID_BACKGROUND.Increment();

        /// <summary>
        /// スクリーン両サイドのカバー部
        /// - 最上部に表示させる
        /// </summary>
        public static readonly FLayeredOrderID BACKGROUND_EDGE__LAYERED_ORDERID = new(layerType: ELayerType.SCREEN_EDGE, orderID: 0);

        #endregion


        // -----------------------------------------------
        // MARK: - for MENU STAGE
        #region MENU_STAGE

        public static FLayeredOrderID BASE_LAYERED_ORDERID_MENU_STAGE = new(layerType: ELayerType.MENU_STAGE, orderID: 0);

        /// <summary>
        /// タイトル画像(バックライト)の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID MENU_STAGE_TITLE_HALO___LAYERED_ORDERID = BASE_LAYERED_ORDERID_MENU_STAGE;

        /// <summary>
        /// タイトル画像の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID MENU_STAGE_TITLE_LOGO_BODY__LAYERED_ORDERID = BASE_LAYERED_ORDERID_MENU_STAGE.Increment();

        /// <summary>
        /// タイトルのボタンの表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID MENU_STAGE_BUTTON__LAYERED_ORDERID = BASE_LAYERED_ORDERID_MENU_STAGE.Increment();

        /// <summary>
        /// タイトルの入力キャンバス表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID MENU_STAGE_INPUT_CANVAS__LAYERED_ORDERID = BASE_LAYERED_ORDERID_MENU_STAGE.Increment();

        #endregion


        // -----------------------------------------------
        // MARK: - for BOARD
        #region BOARD

        public static FLayeredOrderID BASE_LAYERED_ORDERID_BOARD = new(layerType: ELayerType.BOARD, orderID: 0);


        /// <summary>
        /// 盤の影の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID BOARD_SHADOW__LAYERED_ORDERID = BASE_LAYERED_ORDERID_BOARD;

        /// <summary>
        /// 駒台の影の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID WAITING_SPACE_SHADOW__LAYERED_ORDERID = BASE_LAYERED_ORDERID_BOARD.Increment();

        /// <summary>
        /// 盤の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID BOARD_BODY__LAYERED_ORDERID = BASE_LAYERED_ORDERID_BOARD.Increment();

        /// <summary>
        /// 駒台の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID WAITING_SPACE_BODY__LAYERED_ORDERID = BASE_LAYERED_ORDERID_BOARD.Increment();

        /// <summary>
        /// セル選択状態の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID CELL__LAYERED_ORDERID = BASE_LAYERED_ORDERID_BOARD.Increment();

        /// <summary>
        /// 投了ボタンの表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID ON_GAME_BUTTON_RESIGN__LAYERED_ORDERID = BASE_LAYERED_ORDERID_BOARD.Increment();

        /// <summary>
        /// 対局中設定ボタンの表示オーダー
        /// - 勝敗結果のエンブレムを目立たせるため、カーテンよりも下にボタンを表示する。結果画面で押すことはできる
        /// </summary>
        public static readonly FLayeredOrderID ON_GAME_BUTTON_SETTING__LAYERED_ORDERID = BASE_LAYERED_ORDERID_BOARD.Increment();

        #endregion


        // -----------------------------------------------
        // MARK: - for PIECE
        #region PIECE

        public static FLayeredOrderID BASE_LAYERED_ORDERID_PIECE = new(layerType: ELayerType.PIECE, orderID: 0);


        /// <summary>
        /// 駒の影(DEFAULT)の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID PIECE_SHADOW__LAYERED_ORDERID__DEFAULT = BASE_LAYERED_ORDERID_PIECE;

        /// <summary>
        /// 駒の表示(DEFAULT)オーダー
        /// </summary>
        public static readonly FLayeredOrderID PIECE_BODY__LAYERED_ORDERID__DEFAULT = BASE_LAYERED_ORDERID_PIECE.Increment();

        /// <summary>
        /// 駒の影(MOVINGTOWAITINGSPACE)の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID PIECE_SHADOW__LAYERED_ORDERID__MOVINGTOWAITINGSPACE = BASE_LAYERED_ORDERID_PIECE.Increment();

        /// <summary>
        /// 駒の表示(MOVINGTOWAITINGSPACE)オーダー
        /// </summary>
        public static readonly FLayeredOrderID PIECE_BODY__LAYERED_ORDERID__MOVINGTOWAITINGSPACE = BASE_LAYERED_ORDERID_PIECE.Increment();

        /// <summary>
        /// 駒の影(SELECTED)の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID PIECE_SHADOW__LAYERED_ORDERID__SELECTED = BASE_LAYERED_ORDERID_PIECE.Increment();

        /// <summary>
        /// 駒の表示(SELECTED)オーダー
        /// </summary>
        public static readonly FLayeredOrderID PIECE_BODY__LAYERED_ORDERID__SELECTED = BASE_LAYERED_ORDERID_PIECE.Increment();

        /// <summary>
        /// 駒数のレイヤー
        /// </summary>
        /// - 駒の選択状態よりも大きくすること
        public static readonly FLayeredOrderID PIECE_COUNT__LAYERED_ORDERID = BASE_LAYERED_ORDERID_PIECE.Increment();

        #endregion

        // -----------------------------------------------
        // MARK: - for STATUS_BAR

        #region STATUS BAR

        public static FLayeredOrderID BASE_LAYERED_ORDERID_STATUS_BAR = new(layerType: ELayerType.STATUS_BAR, orderID: 0);


        /// <summary>
        /// ステータスバーの土台の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID STATUS_BAR_BODY__LAYERED_ORDERID = BASE_LAYERED_ORDERID_STATUS_BAR;


        /// <summary>
        /// テキストの表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID STATUS_BAR_TEXT__LAYERED_ORDERID = BASE_LAYERED_ORDERID_STATUS_BAR.Increment();


        /// <summary>
        /// ステータスバーのライフ(穴)の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID STATUS_BAR_HOLE__LAYERED_ORDERID = BASE_LAYERED_ORDERID_STATUS_BAR.Increment();

        /// <summary>
        /// ステータスバーのライフ(本体)の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID STATUS_BAR_LIFE_BODY__LAYERED_ORDERID = BASE_LAYERED_ORDERID_STATUS_BAR.Increment();

        /// <summary>
        /// ステータスバーのエンブレムの表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID STATUS_BAR_EMBLEM__LAYERED_ORDERID = BASE_LAYERED_ORDERID_STATUS_BAR.Increment();

        /// <summary>
        /// ステータスバーの番手の表示オーダー(DEFAULT)
        /// </summary>
        public static readonly FLayeredOrderID STATUS_BAR_MOVE_ORDER_DEFAULT__LAYERED_ORDERID = BASE_LAYERED_ORDERID_STATUS_BAR.Increment();

        /// <summary>
        /// ステータスバーの番手の表示オーダー(ON)
        /// </summary>
        public static readonly FLayeredOrderID STATUS_BAR_MOVE_ORDER_ON__LAYERED_ORDERID = BASE_LAYERED_ORDERID_STATUS_BAR.Increment();


        /// <summary>
        /// レート(PLAYER2)の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID STATUS_BAR_RATE_PLAYER2__LAYERED_ORDERID = BASE_LAYERED_ORDERID_STATUS_BAR.Increment();


        /// <summary>
        /// 残り時間(PLAYER1)の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID STATUS_BAR_TIMELEFT_PLAYER1__LAYERED_ORDERID = BASE_LAYERED_ORDERID_STATUS_BAR.Increment();

        /// <summary>
        /// 残り時間(PLAYER2)の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID STATUS_BAR_TIMELEFT_PLAYER2__LAYERED_ORDERID = BASE_LAYERED_ORDERID_STATUS_BAR.Increment();

        #endregion


        // -----------------------------------------------
        // MARK: - for OPPONENT SEARCHER

        #region OPPONENT SEARCHER

        public static FLayeredOrderID BASE_LAYERED_ORDERID_OPPONENT_SEARCHER = new(layerType: ELayerType.OPPONENT_SEARCHER, orderID: 0);

        /// <summary>
        /// OPPONENT SEARCHER のあにめアニメーション表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID OPPONENT_SEARCHER_ANIMATION_LAYERED_ORDERID = BASE_LAYERED_ORDERID_OPPONENT_SEARCHER;

        /// <summary>
        /// OPPONENT SEARCHER のボタン表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID OPPONENT_SEARCHER_BUTTON__LAYERED_ORDERID = BASE_LAYERED_ORDERID_OPPONENT_SEARCHER.Increment();

        /// <summary>
        /// OPPONENT SEARCHER の入力CANVAS表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID OPPONENT_SEARCHER_INPUT_CANVAS__LAYERED_ORDERID = BASE_LAYERED_ORDERID_OPPONENT_SEARCHER.Increment();


        #endregion


        // -----------------------------------------------
        // MARK: - for GAME_RESULT

        #region GAME RESULT

        public static FLayeredOrderID BASE_LAYERED_ORDERID_GAME_RESULT = new(layerType: ELayerType.GAME_RESULT, orderID: 0);

        /// <summary>
        /// カーテン(NORMAL)の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID CURTAIN_GAME_RESULT__LAYERED_ORDERID = BASE_LAYERED_ORDERID_GAME_RESULT;

        /// <summary>
        /// GAME RESULT の勝敗画像表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID GAME_RESULT_GAME_RESULT_IMAGE__LAYERED_ORDERID = BASE_LAYERED_ORDERID_GAME_RESULT.Increment();


        /// <summary>
        /// GAME RESULT の勝敗連続数表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID GAME_RESULT_CONSECUTIVE_COUNT__LAYERED_ORDERID = BASE_LAYERED_ORDERID_GAME_RESULT.Increment();


        /// <summary>
        /// GAME RESULT のレート結果(土台)数表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID GAME_RESULT_RATE_BODY__LAYERED_ORDERID = BASE_LAYERED_ORDERID_GAME_RESULT.Increment();

        /// <summary>
        /// GAME RESULT のレート結果(各種表示)数表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID GAME_RESULT_RATE_MAIN__LAYERED_ORDERID = BASE_LAYERED_ORDERID_GAME_RESULT.Increment();


        /// <summary>
        /// GAME RESULT のテキスト表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID GAME_RESULT_TEXT__LAYERED_ORDERID = BASE_LAYERED_ORDERID_GAME_RESULT.Increment();

        /// <summary>
        /// GAME RESULT のボタン表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID GAME_RESULT_BUTTON__LAYERED_ORDERID = BASE_LAYERED_ORDERID_GAME_RESULT.Increment();

        /// <summary>
        /// GAME RESULT の(旧)段位表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID GAME_RESULT_GRADE_RANK_ICON_PREVIOUS__LAYERED_ORDERID = BASE_LAYERED_ORDERID_GAME_RESULT.Increment();

        /// <summary>
        /// GAME RESULT の(新)段位表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID GAME_RESULT_GRADE_RANK_ICON_NEXT__LAYERED_ORDERID = BASE_LAYERED_ORDERID_GAME_RESULT.Increment();


        /// <summary>
        /// GAME RESULT の入力CANVAS表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID GAME_RESULT_INPUT_CANVAS__LAYERED_ORDERID = BASE_LAYERED_ORDERID_GAME_RESULT.Increment();


        #endregion



        // -----------------------------------------------
        // MARK: - for HISTORY_STAGE

        #region HISTORY STAGE
        public static FLayeredOrderID BASE_LAYERED_ORDERID_HISTORY_STAGE = new(layerType: ELayerType.HISTORY_STAGE, orderID: 0);


        #endregion

        // -----------------------------------------------
        // MARK: - for HISTORY_DETAIL_STAGE

        #region HISTORY DETAIL STAGE
        public static FLayeredOrderID BASE_LAYERED_ORDERID_HISTORY_DETAIL_STAGE = new(layerType: ELayerType.HISTORY_DETAIL_STAGE, orderID: 0);


        #endregion

        // -----------------------------------------------
        // MARK: - for WINDOW_TOP

        #region WINDOW TOP

        public static FLayeredOrderID BASE_LAYERED_ORDERID_WINDOW_TOP = new(layerType: ELayerType.WINDOW_TOP, orderID: 0);

        /// カーテン(TOP)の表示オーダー
        public static readonly FLayeredOrderID CURTAIN_TOP__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_TOP;

        /// ウィンドウ(TOP)の表示オーダー
        public static readonly FLayeredOrderID WINDOW_TOP_BODY__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_TOP.Increment();

        /// ウィンドウ(TOP)のイメージボックスの表示オーダー
        public static readonly FLayeredOrderID WINDOW_TOP_IMAGE_BOX__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_TOP.Increment();

        /// ウィンドウ(TOP)のボタンの表示オーダー
        public static readonly FLayeredOrderID WINDOW_TOP_BUTTON__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_TOP.Increment();

        /// ウィンドウ(TOP)の入力フィールドの表示オーダー
        public static readonly FLayeredOrderID WINDOW_TOP_INPUT_FIELD__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_TOP.Increment();

        /// ウィンドウ(TOP)のチェックボックスの表示オーダー
        public static readonly FLayeredOrderID WINDOW_TOP_CHECK_BOX__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_TOP.Increment();

        /// ウィンドウ(TOP)のスライダーの表示オーダー
        public static readonly FLayeredOrderID WINDOW_TOP_SLIDER__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_TOP.Increment();

        /// ウィンドウ(TOP)のテキストの表示オーダー(その他のUI要素より上位に表示されるようにする)
        public static readonly FLayeredOrderID WINDOW_TOP_TEXT__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_TOP.Increment();
        
        #endregion


        // -----------------------------------------------
        // MARK: - for WINDOW_NORMAL

        #region WINDOW NORMAL

        public static FLayeredOrderID BASE_LAYERED_ORDERID_WINDOW_NORMAL = new(layerType: ELayerType.WINDOW_NORMAL, orderID: 0);

        /// カーテン(NORMAL)の表示オーダー
        public static readonly FLayeredOrderID CURTAIN_NORMAL__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_NORMAL;

        /// ウィンドウ(NORMAL)の表示オーダー
        public static readonly FLayeredOrderID WINDOW_NORMAL_BODY__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_NORMAL.Increment();

        /// ウィンドウ(NORMAL)のイメージボックスの表示オーダー
        public static readonly FLayeredOrderID WINDOW_NORMAL_IMAGE_BOX__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_NORMAL.Increment();

        /// ウィンドウ(NORMAL)のボタンの表示オーダー
        public static readonly FLayeredOrderID WINDOW_NORMAL_BUTTON__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_NORMAL.Increment();

        /// ウィンドウ(NORMAL)の入力フィールドの表示オーダー
        public static readonly FLayeredOrderID WINDOW_NORMAL_INPUT_FIELD__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_NORMAL.Increment();

        /// ウィンドウ(NORMAL)のチェックボックスの表示オーダー
        public static readonly FLayeredOrderID WINDOW_NORMAL_CHECK_BOX__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_NORMAL.Increment();

        /// ウィンドウ(NORMAL)のスライダーの表示オーダー
        public static readonly FLayeredOrderID WINDOW_NORMAL_SLIDER__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_NORMAL.Increment();

        /// ウィンドウ(NORMAL)のテキストの表示オーダー(その他のUI要素より上位に表示されるようにする)
        public static readonly FLayeredOrderID WINDOW_NORMAL_TEXT__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_NORMAL.Increment();
        
        #endregion


        // -----------------------------------------------
        // MARK: - for WINDOW_CONFIRMATION

        #region WINDOW CONFIRMATION

        public static FLayeredOrderID BASE_LAYERED_ORDERID_WINDOW_CONFIRMATION = new(layerType: ELayerType.WINDOW_CONFIRMATION, orderID: 0);

        /// カーテン(CONFIRMATION)の表示オーダー
        public static readonly FLayeredOrderID CURTAIN_CONFIRMATION__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_CONFIRMATION;

        /// ウィンドウ(CONFIRMATION)の表示オーダー
        public static readonly FLayeredOrderID WINDOW_CONFIRMATION_BODY__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_CONFIRMATION.Increment();

        /// ウィンドウ(CONFIRMATION)のイメージボックスの表示オーダー
        public static readonly FLayeredOrderID WINDOW_CONFIRMATION_IMAGE_BOX__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_CONFIRMATION.Increment();

        /// ウィンドウ(CONFIRMATION)のボタンの表示オーダー
        public static readonly FLayeredOrderID WINDOW_CONFIRMATION_BUTTON__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_CONFIRMATION.Increment();

        /// ウィンドウ(CONFIRMATION)の入力フィールドの表示オーダー
        public static readonly FLayeredOrderID WINDOW_CONFIRMATION_INPUT_FIELD__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_CONFIRMATION.Increment();

        /// ウィンドウ(CONFIRMATION)のチェックボックスの表示オーダー
        public static readonly FLayeredOrderID WINDOW_CONFIRMATION_CHECK_BOX__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_CONFIRMATION.Increment();

        /// ウィンドウ(CONFIRMATION)のスライダーの表示オーダー
        public static readonly FLayeredOrderID WINDOW_CONFIRMATION_SLIDER__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_CONFIRMATION.Increment();

        /// ウィンドウ(CONFIRMATION)のテキストの表示オーダー(その他のUI要素より上位に表示されるようにする)
        public static readonly FLayeredOrderID WINDOW_CONFIRMATION_TEXT__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_CONFIRMATION.Increment();


        #endregion


        // -----------------------------------------------
        // MARK: - for WINDOW_ERROR

        #region WINDOW ERROR

        public static FLayeredOrderID BASE_LAYERED_ORDERID_WINDOW_ERROR = new(layerType: ELayerType.WINDOW_ERROR, orderID: 0);

        /// カーテン(ERROR)の表示オーダー
        public static readonly FLayeredOrderID CURTAIN_ERROR__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_ERROR;

        /// ウィンドウ(ERROR)の表示オーダー
        public static readonly FLayeredOrderID WINDOW_ERROR_BODY__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_ERROR.Increment();

        /// ウィンドウ(ERROR)のイメージボックスの表示オーダー
        public static readonly FLayeredOrderID WINDOW_ERROR_IMAGE_BOX__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_ERROR.Increment();

        /// ウィンドウ(ERROR)のボタンの表示オーダー
        public static readonly FLayeredOrderID WINDOW_ERROR_BUTTON__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_ERROR.Increment();

        /// ウィンドウ(ERROR)の入力フィールドの表示オーダー
        public static readonly FLayeredOrderID WINDOW_ERROR_INPUT_FIELD__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_ERROR.Increment();

        /// ウィンドウ(ERROR)のチェックボックスの表示オーダー
        public static readonly FLayeredOrderID WINDOW_ERROR_CHECK_BOX__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_ERROR.Increment();

        /// ウィンドウ(ERROR)のスライダーの表示オーダー
        public static readonly FLayeredOrderID WINDOW_ERROR_SLIDER__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_ERROR.Increment();

        /// ウィンドウ(ERROR)のテキストの表示オーダー(その他のUI要素より上位に表示されるようにする)
        public static readonly FLayeredOrderID WINDOW_ERROR_TEXT__LAYERED_ORDERID = BASE_LAYERED_ORDERID_WINDOW_ERROR.Increment();


        #endregion

        // -----------------------------------------------
        // MARK: - for STAGE INPUT

        #region STAGE INPUT

        public static FLayeredOrderID BASE_LAYERED_ORDERID_STAGE_INPUT = new(layerType: ELayerType.STAGE_INPUT, orderID: 0);

        /// <summary>
        /// STAGE INPUT判定の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID STAGE_INPUT__LAYERED_ORDERID = BASE_LAYERED_ORDERID_STAGE_INPUT.Increment();

        #endregion

        // -----------------------------------------------
        // MARK: - for PROMOTION_SELECTOR

        #region PROMOTION SELECTOR

        public static FLayeredOrderID BASE_LAYERED_ORDERID_PROMOTION_SELECTOR = new(layerType: ELayerType.PROMOTION_SELECTOR, orderID: 0);


        /// <summary>
        /// 成り選択の影の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID PROMOTION_SELECTOR_SHADOW__LAYERED_ORDERID__DEFAULT = BASE_LAYERED_ORDERID_PROMOTION_SELECTOR;

        /// <summary>
        /// 成り選択の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID PROMOTION_SELECTOR_BODY__LAYERED_ORDERID__DEFAULT = BASE_LAYERED_ORDERID_PROMOTION_SELECTOR.Increment();

        /// <summary>
        /// 成り選択のボタン表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID PROMOTION_SELECTOR_BUTTON__LAYERED_ORDERID__DEFAULT = BASE_LAYERED_ORDERID_PROMOTION_SELECTOR.Increment();

        /// <summary>
        /// 成り選択の駒影表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID PROMOTION_SELECTOR_PIECE_SHADOW__LAYERED_ORDERID__DEFAULT = BASE_LAYERED_ORDERID_PROMOTION_SELECTOR.Increment();

        /// <summary>
        /// 成り選択の駒本体表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID PROMOTION_SELECTOR_PIECE_BODY__LAYERED_ORDERID__DEFAULT = BASE_LAYERED_ORDERID_PROMOTION_SELECTOR.Increment();

        /// <summary>
        /// 成り選択のINPUT表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID PROMOTION_SELECTOR_INPUT__LAYERED_ORDERID__DEFAULT = BASE_LAYERED_ORDERID_PROMOTION_SELECTOR.Increment();

        #endregion

        // -----------------------------------------------
        // MARK: - for RESIGN_SELECTOR

        #region RESIGN SELECTOR

        public static FLayeredOrderID BASE_LAYERED_ORDERID_RESIGN_SELECTOR = new(layerType: ELayerType.RESIGN_SELECTOR, orderID: 0);


        /// <summary>
        /// 投了選択の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID RESIGN_SELECTOR_BODY__LAYERED_ORDERID__DEFAULT = BASE_LAYERED_ORDERID_RESIGN_SELECTOR.Increment();

        /// <summary>
        /// 投了選択のボタン表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID RESIGN_SELECTOR_BUTTON__LAYERED_ORDERID__DEFAULT = BASE_LAYERED_ORDERID_RESIGN_SELECTOR.Increment();

        /// <summary>
        /// 投了選択のINPUT表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID RESIGN_SELECTOR_INPUT__LAYERED_ORDERID__DEFAULT = BASE_LAYERED_ORDERID_RESIGN_SELECTOR.Increment();

        #endregion


        // -----------------------------------------------
        // MARK: - for GAME_STAGE_TEXT_BALLOON

        #region GAME STAGE TEXT BALLOON

        public static FLayeredOrderID BASE_LAYERED_ORDERID_GAME_STAGE_TEXT_BALLOON = new(layerType: ELayerType.GAME_STAGE_TEXT_BALLOON, orderID: 0);

        /// <summary>
        /// 投了選択の表示オーダー
        /// </summary>
        public static readonly FLayeredOrderID GAME_STAGE_TEXT_BALLOON__LAYERED_ORDERID = BASE_LAYERED_ORDERID_GAME_STAGE_TEXT_BALLOON.Increment();

        #endregion



        // -----------------------------------------------
        // MARK: - for LAYOUT

        #region LAYOUT

        public static FLayeredOrderID BASE_LAYERED_ORDERID_LAYOUT = new(layerType: ELayerType.LAYOUT, orderID: 0);

        #endregion


        // ===============================================
        // MARK: - ● Promotion Selector ─────

        public static readonly Vector2 PROMOTION_SELECTOR_BODY__IMAGE_SIZE = new(772, 368);



        // ===============================================
        // MARK: - ● Resign Selector ─────

        public static readonly Vector2 RESIGN_SELECTOR_BODY__IMAGE_SIZE = PROMOTION_SELECTOR_BODY__IMAGE_SIZE;


        // ===============================================
        // MARK: - ● カーテン ─────

        public static readonly Vector2 CURTAIN_BODY__IMAGE_SIZE = new(8f, 8f);

        public const float CURTAIN__ALPHA_DEFAULT = 0.5f;


        // ===============================================
        // MARK: - ● ウィンドウ ─────

        public static readonly FVec4 WINDOW_BODY__UNRESIZABLE_SPACE4 = new(top: 128, right: 128, bottom: 128, left: 128);


        // ===============================================
        // MARK: - ● 背景関連サイズ ─────

        public static readonly Vector2 BACKGOURND_BODY__IMAGE_SIZE = new(1365f, 2048f);

        public static readonly Vector2 BACKGOURND_RIPPLE_A__IMAGE_SIZE = new(640f, 818f);
        public static readonly Vector2 BACKGOURND_RIPPLE_B__IMAGE_SIZE = new(1464f, 904f);
        public static readonly Vector2 BACKGOURND_RIPPLE_C__IMAGE_SIZE = new(1440f, 912f);
        public static readonly Vector2 BACKGOURND_RIPPLE_D__IMAGE_SIZE = new(744f, 736f);


        


        

        public const float BACKGROUND_RIPPLE__ALPHA_DEFAULT = 0.15f;




        // ===============================================
        // MARK: - ● ステータスバー関連サイズ ─────

        public static readonly Vector2 STATUS_BAR_BODY__IMAGE_SIZE = new(1220f, 108f);

        public static readonly Vector2 STATUS_BAR_LIFE__IMAGE_SIZE = new(88f, 96f);

        public static readonly Vector2 STATUS_BAR_EMBLEM__IMAGE_SIZE = new(432f, 132f);

        public static readonly Vector2 STATUS_BAR_ORDER__IMAGE_SIZE = new(128f, 128f);




        // ===============================================
        // MARK: - ● 画像サイズ ─────
        // - 主にレイアウト時の比率計算に使用

        /// <summary>
        /// 駒本体の画像サイズ
        /// </summary>
        /// <remarks>
        /// 王将の画像に合わせて修正する必要あり
        /// </remarks>
        public static readonly Vector2 PIECE_BODY__IMAGE_SIZE = new(376f, 400f);

        /// <summary>
        /// 盤本体の画像サイズ
        /// </summary>
        public static readonly Vector2 BOARD_BODY__IMAGE_SIZE = new(1011f, 1107f);



        /// <summary>
        /// 盤本体における駒の画像表示サイズ(画像基準)
        /// </summary>
        public static readonly Vector2 BOARD_BODY__PIECE__SIZE_66 = new(145f, 155f);
        public static readonly Vector2 BOARD_BODY__PIECE__SIZE_99 = new(99f, 106f);

        /// <summary>
        /// 盤本体における周囲の空白
        /// </summary>
        public static readonly FVec4 BOARD_BODY__INNER_SPACE4_RATIO_66 = new(0.028003613f, 0.03066271f, 0.028003613f, 0.03066271f);
        public static readonly FVec4 BOARD_BODY__INNER_SPACE4_RATIO_99 = new(0.025717111770524f, 0.023486901535682f, 0.025717111770524f, 0.023486901535682f);

        //h27

        /// <summary>
        /// 駒台(横長)本体の画像サイズ
        /// </summary>
        public static readonly Vector2 WAITING_SPACE_HORIZONTAL__IMAGE_SIZE = new(1169f, 237f);

        /// <summary>
        /// 駒台(横長)本体における駒の画像表示サイズ(画像基準)
        /// </summary>
        public static readonly Vector2 WAITING_SPACE_HORIZONTAL__PIECE_SIZE = new(145f, 155f);

        /// <summary>
        /// 駒台(横長)本体における周囲の空白比率(Player1)
        /// </summary>
        public static readonly FVec4 WAITING_SPACE_HORIZONTAL__INNER_SPACE4_RATIO__PLAYER1 = new(0.130801688f, 0.026518392f, 0.130801688f, 0.026518392f);

        /// <summary>
        /// 駒台(横長)本体における周囲の空白比率(Player2)
        /// </summary>
        public static readonly FVec4 WAITING_SPACE_HORIZONTAL__INNER_SPACE4_RATIO__PLAYER2 = new(0.130801688f, 0.026518392f, 0.130801688f, 0.026518392f);



        /// <summary>
        /// 駒台(四角)本体の画像サイズ
        /// </summary>
        public static readonly Vector2 WAITING_SPACE_SQUARE__IMAGE_SIZE = new(537f, 585f);

        /// <summary>
        /// 駒台(四角)本体における駒の画像表示サイズ(画像基準)
        /// </summary>
        public static readonly Vector2 WAITING_SPACE_SQUARE__PIECE_SIZE = new(145f, 155);

        /// <summary>
        /// 駒台(四角)本体における周囲の空白比率(Player1)
        /// </summary>
        /// <remarks>
        /// ★ 値は仮のもの
        /// </remarks>
        public static readonly FVec4 WAITING_SPACE_VERTICAL__INNER_SPACE4_RATIO__PLAYER1 = new(0.130801688f, 0.026518392f, 0.130801688f, 0.026518392f);

        /// <summary>
        /// 駒台(四角)本体における周囲の空白比率(Player2)
        /// </summary>
        /// <remarks>
        /// ★ 値は仮のもの
        /// </remarks>
        public static readonly FVec4 WAITING_SPACE_VERTICAL__INNER_SPACE4_RATIO__PLAYER2 = new(0.130801688f, 0.026518392f, 0.130801688f, 0.026518392f);



        // ===============================================
        // MARK: - ● 盤 ─────



        /// <summary>
        /// 盤の影の位置比率
        /// </summary>
        public static readonly Vector2 BOARD_SHADOW__LOCAL_POSITION_RATIO__DEFAULT = new(0.01f, -0.02f);

        /// <summary>
        /// 盤の影のα
        /// </summary>
        public const float BOARD_SHADOW__ALPHA__DEFAULT = 0.25f;



        // ===============================================
        // MARK: - ● 駒台 ─────

        /// <summary>
        /// 駒台の位置比率
        /// </summary>
        public static readonly Vector2 WAITING_SPACE_SHADOW__LOCAL_POSITION_RATIO__DEFAULT = new(0.02f, -0.08f);


        /// <summary>
        /// 駒台の影のα
        /// </summary>
        public const float WAITING_SPACE_SHADOW__ALPHA__DEFAULT = 0.25f;


        /// <summary>
        /// 駒数の画像サイズ
        /// </summary>
        public static readonly Vector2 PIECE_COUNT__IMAGE_SIZE = new(240, 240);


        

        public static readonly Vector2 PIECE_COUNT__SCALE_RATIO__DEFAULT = new(1f, 1f);


        /// <summary>
        /// 駒台(水平)における駒数表示サイズ
        /// </summary>
        public static readonly Vector2 WAITING_SPACE_HORIZONTAL__PIECE_COUNT__SIZE = new(72f, 72f);


        /// <summary>
        /// 駒台(四角)における駒数表示サイズ
        /// </summary>
        public static readonly Vector2 WAITING_SPACE_SQUARE__PIECE_COUNT__SIZE = new(72f, -72f);



        // ===============================================
        // MARK: - ●  ─────

        /// <summary>
        /// 盤画像におけるセルのサイズ
        /// </summary>
        public static readonly Vector2 BOARD__CELL__SIZE_66 = new(158, 174);
        public static readonly Vector2 BOARD__CELL__SIZE_99 = new(107, 117);

        /// <summary>
        /// セル選択の画像サイズ
        /// </summary>
        public static readonly Vector2 CELL__IMAGE_SIZE = new(158, 174);



        // ===============================================
        // MARK: - ● 駒 ─────

        public static readonly Vector2 PIECE_BODY__LOCAL_POSITION_RATIO__DEFAULT = new(0f, 0f);
        public static readonly Vector2 PIECE_BODY__LOCAL_POSITION_RATIO__SELECTED = new(0f, 0.2f);
        public static readonly Vector2 PIECE_BODY__LOCAL_POSITION_RATIO__MOVETOWAITINGSPACE_PEAK = new(0f, 0.2f);


        public static readonly Vector2 PIECE_SHADOW__LOCAL_POSITION_RATIO__DEFAULT = new(0.05f, -0.05f);
        public static readonly Vector2 PIECE_SHADOW__LOCAL_POSITION_RATIO__SELECTED = new(0.25f, -0.25f);
        public static readonly Vector2 PIECE_SHADOW__LOCAL_POSITION_RATIO__MOVETOWAITINGSPACE_PEAK = new(0.25f, -0.25f);


        public const float PIECE_SHADOW__ALPHA__DEFAULT = 0.3f;
        public const float PIECE_SHADOW__ALPHA__SELECTED = 0.15f;
        public const float PIECE_SHADOW__ALPHA__MOVETOWAITINGSPACE_PEAK = 0.15f;


        public static readonly Vector2 PIECE_BODY__SCALE_RATIO__DEFAULT = new(1f, 1f);
        public static readonly Vector2 PIECE_BODY__SCALE_RATIO__SELECTED = new(1.2f, 1.2f);


        public static readonly Vector2 PIECE_SHADOW__SCALE_RATIO__DEFAULT = new(1f, 1f);
        public static readonly Vector2 PIECE_SHADOW__SCALE_RATIO__SELECTED = new(1.2f, 1.2f);


        /// <summary>
        /// セルの表示
        /// </summary>
        public const float CELL__ALPHA__PLACE_TARGET = 0.4f;



        // ===============================================
        // MARK: - ● 対局結果 ─────

        /// <summary>
        /// 対局結果におけるレート結果(BODY)のサイズ
        /// </summary>
        public static readonly Vector2 GAME_RESULT_RATE_RESULT_BODY__IMAGE_SIZE = new(999, 120);



        // ===============================================
        // MARK: - ● History Stage ─────


        public static readonly Vector2 HISTORY_DETAIL_STAGE_RESULT_ICON__IMAGE_SIZE = new(256f, 256f);

        public static readonly Vector2 HISTORY_DETAIL_STAGE_BOARD_TYPE_ICON__IMAGE_SIZE = new(118f, 59f);

        



        // ===============================================
        // MARK: - ●  ─────

        /// <summary>
        /// タブレット端末判定用の画面縦横比の最低値(含む)
        /// </summary>
        public const float MIN_THRESHOLD_RATIO_OF_TABLET = 0.62f;


        /// <summary>
        /// 初期画面表示方向
        /// </summary>
        public const EScreenOrientation DEFAULT_DISPLAY_DIRECTION = EScreenOrientation.PORTRAIT;



        // ===============================================
        // MARK: - ●  ─────

        /// <summary>
        /// 成り選択にて、左側が成りボタン・右側が不成ボタンか
        /// </summary>
        public static bool PROMOTION_SELECTOR__LEFT_IS_PROMTE = true;



        // ===============================================
        // MARK: - ● UI ─────
        
        /// <summary>
        /// WindowConfirmation の通常ウィンドウに対するサイズ比
        /// </summary>
        public static readonly Vector2 WindowConfirmationSizeRatio = new(x: 0.9f, y: 0.8f);

        /// <summary>
        /// WindowTop の通常ウィンドウに対するサイズ比
        /// </summary>
        public static readonly Vector2 WindowTopSizeRatio = new(x: 1f, y: 0.5f);
    }
}