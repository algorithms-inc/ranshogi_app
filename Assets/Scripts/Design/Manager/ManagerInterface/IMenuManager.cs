using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFramework.Core;


public interface IMenuManager
{
    // 表示アニメーションが完了
    void OnDidShowAnimation();

    // 対局(66)を開始ボタンを押下
    void OnTapButtonGame66();

    // 勝ち切れ乱将棋ボタンを押下
    void OnTapButtonKachikire();

    // 勝ち切れ乱将棋のカレンダーで月を切り替え
    void OnTapKachikireChangeMonth(int year, int month);

    // 勝ち切れ乱将棋のカレンダーで対局開始ボタンを押下
    void OnTapKachikireStart(System.DateTime date);

    // 友達と対局ボタンを押下
    void OnTapButtonGameWithFriend();

    // 対局履歴ボタンを押下
    void OnTapButtonHistory();

    // 成績ボタンを押下(ランキングボタンがあった場所。これまでのマイページを開く)
    void OnTapButtonRecord();

    // 設定ボタンを押下
    void OnTapButtonSetting();

    // ヘルプボタンを押下
    void OnTapButtonHelp();
}