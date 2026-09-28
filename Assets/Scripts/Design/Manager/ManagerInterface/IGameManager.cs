using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFramework.Core;


public interface IGameManager
{
    // ===============================================
    // MARK: - ● 待機画面 ─────

    // 待機画面で戻るを押下
    void OnTapBackOnOpponentSearcher();

    // 待機画面の非表示化が完了(対戦開始時)
    void OnDidHideOpponentSearcherForGame();



    // ===============================================
    // MARK: - ● 対局画面 ─────

    // 盤セットの表示完了
    void OnDidShowBoardSet();

    // 盤上の駒がないセルをタップ
    void OnTap(Square sq);

    // 盤上の駒をタップ
    void OnTap(Piece piece, Square sq);

    // 自軍の駒台の駒をタップ
    void OnTapOnWaitingSpace(Piece piece);

    // 盤・自軍駒台外をタップ
    void OnTapOutOfBoard();

    // 成る／成らないの選択完了(66)
    // - プレイヤーが「成る／成らない」の選択を行ったときに呼び出される
    // - 「成る」が選ばれた場合画面表示上はすでに、駒の表示は変更されている。それに合わせて、データ内で指定の駒の状態を「成る」に変更する必要がある。
    void OnDidSelectPromotion(Piece piece, Square sq, EPromotionResultType promotionResultType);

    // 投了ボタンをタップ
    void OnTapButtonResign();

    // AI対戦で「待った」ボタンをタップ
    void OnTapButtonUndoInAiGame();

    // AI対戦で「戻る」ボタンをタップ
    void OnTapButtonBackInAiGame();


    // ===============================================
    // MARK: - ● 対局終了後 ─────

    // 対局結果表示で「検討」押下
    void OnTapButtonHistoryDetail();

    // 対局結果表示で「次の対局へ／友達ともう一度対局する」押下
    void OnTapButtonNextGameOnGameResult();

    // 対局結果表示で「HOME」押下
    void OnTapButtonHomeOnGameResult();


    // ===============================================
    // MARK: - ● 対局中設定 ─────

    // 対局中設定で「音を消す」を操作
    void OnTapPlayAudio(bool isOn);

    // 対局結果表示で「相手の名前を表示」を操作
    void OnTapShowNamePlayer2(bool isOn);

    // 対局結果表示で「自分の名前を表示」を操作
    void OnTapShowNamePlayer1(bool isOn);
}