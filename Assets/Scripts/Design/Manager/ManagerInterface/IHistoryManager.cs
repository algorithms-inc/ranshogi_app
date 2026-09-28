using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFramework.Core;


public interface IHistoryManager
{
    // 表示アニメーションが完了
    void OnDidShowAnimation();


    // 戻るボタンを押下
    void OnTapButtonBack();

    // 棋譜を選択するボタンを押下
    void OnTapButtonShowHistoryDetail(int historyDetailID);

    // 次のページへボタンを押下
    void OnTapButtonNextPage();

    // 前のページへボタンを押下
    void OnTapButtonPreviousPage();

    // 最初のページへボタンを押下
    void OnTapButtonFirstPage();

    // 最後のページへボタンを押下
    void OnTapButtonLastPage();
}