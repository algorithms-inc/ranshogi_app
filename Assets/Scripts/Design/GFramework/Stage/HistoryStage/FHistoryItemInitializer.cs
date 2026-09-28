using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework.Core
{
    public enum EHistoryItemResult
    {
        // 勝ち
        WIN,
        // 負け
        LOSE,
        // 引き分け
        DRAW,
        // 不明
        UNKNOWN
    }


    public readonly struct FHistoryItemInitializer
    {
        // 項目ID
        private readonly int _historyDetailID;
        public readonly int HistoryDetailID => _historyDetailID;

        // 対戦回数
        private readonly int _gameNum;
        public readonly int GameNum => _gameNum;

        // 盤の種類
        private readonly EBoardType _boardType;
        public readonly EBoardType BoardType => _boardType;

        // 対局結果
        private readonly EHistoryItemResult _historyItemResult;
        public readonly EHistoryItemResult HistoryItemResult => _historyItemResult;

        // ゲーム勝敗の理由
        private readonly EGameResultReasonType _gameResultReasonType;
        public readonly EGameResultReasonType GameResultReasonType => _gameResultReasonType;

        // 日付
        private readonly string _date;
        public readonly string Date => _date;

        // 日付付帯情報
        private readonly string _dateAdditionalInfo;
        public readonly string DateAdditionalInfo => _dateAdditionalInfo;

        // 先手・後手
        private readonly Color _color;
        public readonly Color Color => _color;

        // プレイヤー名
        private readonly string _playerName;
        public readonly string PlayerName => _playerName;

        // 対局相手名
        private readonly string _opponentName;
        public readonly string OpponentName => _opponentName;

        // プレイヤーの段位
        private readonly EGradeRankType _gradeRankType;
        public readonly EGradeRankType GradeRankType => _gradeRankType;

        // 対局相手の段位
        private readonly EGradeRankType _opponentGradeRankType;
        public readonly EGradeRankType OpponentGradeRankType => _opponentGradeRankType;

        // 対局前レート
        private readonly int _previousRate;
        public readonly int PreviousRate => _previousRate;

        // 対局後レート
        private readonly int _nextRate;
        public readonly int NextRate => _nextRate;

        // 対局前段位
        private readonly EGradeRankType _previousGradeRankType;
        public readonly EGradeRankType PreviousGradeRankType => _previousGradeRankType;

        // 対局後段位
        private readonly EGradeRankType _nextGradeRankType;
        public readonly EGradeRankType NextGradeRankType => _nextGradeRankType;

        // 棋譜データ出力可否定
        private readonly bool _canExportData;
        public readonly bool CanExportData => _canExportData;

        // プレイヤー名（先手・後手）
        public readonly string PlayerBlackName => _color == Color.Black ? _playerName : _opponentName;
        public readonly string PlayerWhiteName => _color == Color.White ? _playerName : _opponentName;

        private readonly bool _isEnabled;
        public bool IsEnabled => _isEnabled;

        public FHistoryItemInitializer(
            int historyDetailID,
            int gameNum,
            EBoardType boardType,
            EHistoryItemResult historyItemResult,
            EGameResultReasonType gameResultReasonType,
            string date,
            string dateAdditionalInfo,
            Color color,
            string playerName,
            string opponentName,
            EGradeRankType gradeRankType,
            EGradeRankType opponentGradeRankType,
            int previousRate,
            int nextRate,
            EGradeRankType previousGradeRankType,
            EGradeRankType nextGradeRankType,
            bool canExportData,
            bool isEnabled)
        {
            _historyDetailID = historyDetailID;
            _gameNum = gameNum;
            _boardType = boardType;
            _historyItemResult = historyItemResult;
            _gameResultReasonType = gameResultReasonType;
            _date = date;
            _dateAdditionalInfo = dateAdditionalInfo;
            _color = color;
            _playerName = playerName;
            _opponentName = opponentName;
            _gradeRankType = gradeRankType;
            _opponentGradeRankType = opponentGradeRankType;
            _previousRate = previousRate;
            _previousGradeRankType = previousGradeRankType;
            _nextGradeRankType = nextGradeRankType;
            _nextRate = nextRate;
            _canExportData = canExportData;
            _isEnabled = isEnabled;
        }


        public int GetDeltaRate()
        {
            return _nextRate - _previousRate;
        }
    }
}