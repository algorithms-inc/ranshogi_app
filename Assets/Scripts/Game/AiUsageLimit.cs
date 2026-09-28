using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// AI機能(AIと対戦・AI評価値)の1日の利用回数の制限。
/// 無料では合わせて1日3回まで(日本時間の日付で数え、端末に保存する)。
/// プレミアム会員と、広告を見て「本日無制限」になったユーザは数えない。
/// 「本日無制限」はサーバの unlimitedUntil で、対局回数の制限と共通(広告を1回見れば両方とも無制限になる)。
/// 勝ち切れ乱将棋は1日1局面の別の機能なので数えない。
/// </summary>
public static class AiUsageLimit
{
    public const int FreeUsesPerDay = 3;

    const string DateKey  = "AiUsageDate";
    const string CountKey = "AiUsageCount";

    static string Today => DateTime.UtcNow.AddHours(9).ToString("yyyy-MM-dd");

    static int UsedToday => PlayerPrefs.GetString(DateKey, "") == Today ? PlayerPrefs.GetInt(CountKey, 0) : 0;


    static bool IsUnlimitedUser()
    {
        var user = Auth.instance.GetUser();
        return user.isPayedUser || user.IsUnlimited();
    }


    /// <summary>
    /// AI機能を1回使う。使えるなら回数を数えて true を返す。
    /// 使えないときは広告を見るかプレミアム会員になるかのウィンドウを出して false を返し、
    /// 広告を見終わったら(本日無制限になってから) onUnlocked を呼ぶ。
    /// </summary>
    public static bool TryUse(UnityAction onUnlocked)
    {
        if (IsUnlimitedUser()) return true;

        int used = UsedToday;
        if (used < FreeUsesPerDay)
        {
            PlayerPrefs.SetString(DateKey, Today);
            PlayerPrefs.SetInt(CountKey, used + 1);
            PlayerPrefs.Save();
            return true;
        }

        ShowLimitWindow(onUnlocked: onUnlocked);
        return false;
    }


    static void ShowLimitWindow(UnityAction onUnlocked)
    {
        string limitMessage = $"AI機能（AIと対戦・AI評価値）は1日{FreeUsesPerDay}回までです。広告を見ると本日中はAI機能も対局も無制限になります。";

        var view = UnityEngine.Object.FindFirstObjectByType<SubscriptionView>();
        if (view == null)
        {
            // 広告のウィンドウを置いていない画面では案内だけ出す
            AlertViewController.ShowOKConfirmation($"AI機能（AIと対戦・AI評価値）は1日{FreeUsesPerDay}回までです。");
            return;
        }

        view.SetOnUserEarnedReward(onUnlocked);
        view.ShowAdOrBuySubscriptionConfirmation(limitMessage: limitMessage, unlimitedFeatures: "AI機能と対局");
    }
}
