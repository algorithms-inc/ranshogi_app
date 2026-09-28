using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFramework.Core;


public interface IHistoryDetailManager
{
    // 表示アニメーションが完了
    void OnDidShowAnimation();


    // 戻るボタンを押下
    void OnTapButtonBack();

    // 一手進むボタンを押下
    void OnTapButtonStepForward();

    // 一手戻るボタンを押下
    void OnTapButtonStepBack();

    // 最後へジャンプボタンを押下
    void OnTapButtonJumpToLast();

    // 最初へジャンプボタンを押下
    void OnTapButtonJumpToFirst();

    // 「AI評価値」ボタンを押下
    void OnTapButtonAiEvaluation();

    // 「本譜」ボタンを押下。駒を動かしたり読み筋を再生したりしてできた変化をやめ、本譜に戻る
    void OnTapButtonBackToActualPlay();

    // AI評価値の吹き出しで「手順を再生」を押下
    void OnTapButtonReplayAiVariation();

    // 盤上の駒がないセルをタップ
    void OnTap(Square sq);

    // 盤上の駒をタップ(先手・後手どちらの駒も)
    void OnTap(Piece piece, Square sq);

    // 駒台の駒をタップ(先手・後手どちらの駒台も)
    void OnTapOnWaitingSpace(Piece piece);

    // 盤・駒台の外をタップ
    void OnTapOutOfBoard();

    // 棋譜データ保存ボタンを押下
    void OnTapExportData(EKifuDataType kifuDataType);

    // 「AIと対戦」ボタンを押下。対戦できる局面なら IHistoryDetailStage.ShowWindowAiMatch() でウィンドウを出す
    void OnTapButtonAiMatch();

    // AI対戦ウィンドウで対戦開始ボタンを押下
    void OnTapButtonStartAiMatch(Color playerColor, RanshogiEngine.Level level);
}