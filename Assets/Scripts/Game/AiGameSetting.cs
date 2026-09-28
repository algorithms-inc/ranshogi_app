using System.Collections.Generic;

/// <summary>
/// AI対戦を始めるための設定。検討画面で作って GameManager 経由で対局画面へ渡す。
/// これが null でないときだけ、GameController はサーバーではなくエンジンと対戦する。
/// </summary>
public class AiGameSetting
{
    /// <summary>対戦の開始局面(アプリ形式のsfen。棋譜の初期局面)。</summary>
    public string rootSfen;

    /// <summary>開始局面からその局面までの指し手。検討画面で表示していた手数まで。待ったで開き直したときは、それまでに指した手も入る。</summary>
    public List<Move> initialMoves = new List<Move>();

    /// <summary>対局を始めたときの手数。待ったでこれより前には戻さない。-1 なら未設定(対局画面で initialMoves の数を入れる)。</summary>
    public int startMoveCount = -1;

    /// <summary>プレイヤーが持つ側。</summary>
    public Color playerColor = Color.Black;

    /// <summary>AIの強さ。</summary>
    public RanshogiEngine.Level level = RanshogiEngine.Level.Hard;

    /// <summary>元になった棋譜の対局番号(結果画面の表示用)。</summary>
    public int gameNum;

    public string PlayerName = "あなた";
    public string AiName     = "AI";

    /// <summary>勝ち切れ乱将棋の局面の日付(YYYY-MM-DD)。勝ち切れ乱将棋のときだけ入り、勝ったらサーバに記録する。</summary>
    public string kachikireDate;

    /// <summary>勝ち切れ乱将棋の正解手順(USI形式)。結果画面の「正解手順」で検討画面に出す。古い局面では空。</summary>
    public string kachikirePv;
}
