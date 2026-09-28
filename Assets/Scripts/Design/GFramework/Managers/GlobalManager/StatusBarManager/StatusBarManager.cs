using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;


// TODO: Player2のステータスバーが削除された際には、_statusBarPlayer2 = null をセットしたい
namespace GFramework
{
    public class StatusBarManager : IStatusBarManager
    {
        private StatusBarBehaviour _statusBarPlayer1;
        private StatusBarBehaviour _statusBarPlayer2;

        private StatusBarLayouter _layouter;
        public StatusBarLayouter Layouter => _layouter;

        private bool _isNameShownForPlayer1 = true;
        private bool _isNameShownForPlayer2 = true;

        public StatusBarManager()
        {
            // Do Nothing;
        }


        // ===============================================
        // MARK: - ● Setup  ─────


        // ステータスバー内にGameObjectを生成するさいに、Locator 経由で StatubBarManager にアクセスする必要がある。そのため、コンストラクタで生成したのちに Setup を行う。
        public void Setup()
        {
            _layouter = new StatusBarLayouter(countOfLife: Config.LifeMax);

            SetupGameObject();
        }


       
        private void SetupGameObject()
        {
            var gameObjectStatusBarPlayer1 = new GameObject("StatusBarPlayer1");
            _statusBarPlayer1 = gameObjectStatusBarPlayer1.AddComponent<StatusBarBehaviour>();
            _statusBarPlayer1.SetupComponent(playerType: EPlayerType.PLAYER1, playerName: "---");

            Utl.SetDontDestroyOnLoad(targetObject: gameObjectStatusBarPlayer1);
        }


        public void SetupGameObjectForPlayer2(GameObject parentGameObject, string playerName, EMoveOrderType moveOrderType, EGradeRankType gradeRankType, EBoardType boardType)
        {
            var gameObjectStatusBarPlayer2 = new GameObject("StatusBarPlayer2");
            _statusBarPlayer2 = gameObjectStatusBarPlayer2.AddComponent<StatusBarBehaviour>();
            _statusBarPlayer2.SetupComponent(playerType: EPlayerType.PLAYER2, playerName: playerName);

            gameObjectStatusBarPlayer2.transform.parent = parentGameObject.transform;

            _statusBarPlayer2.SetMoveOrder(moveOrderType: moveOrderType, gradeRankType: gradeRankType, boardType: boardType);

            if (moveOrderType == EMoveOrderType.SECOND)
            {
                _statusBarPlayer2.SetMoveOrderOnOff(isOn: false);
            }
        }


        // ===============================================
        // MARK: - ● SetStatusBarForBehaviourPlayer2NullWhenCloseGameStage  ─────

        public void SetStatusBarForBehaviourPlayer2NullWhenCloseGameStage()
        {
            _statusBarPlayer2 = null;
        }


        // ===============================================
        // MARK: - ● Get StatusBarBehaviour  ─────

        private StatusBarBehaviour GetStatusBarBehaviour(EPlayerType playerType)
        {
            return playerType switch
            {
                EPlayerType.PLAYER1 => _statusBarPlayer1,
                EPlayerType.PLAYER2 => _statusBarPlayer2,
                _ => throw new InvalidOperationException(),
            };
        }


        public void Show(EPlayerType playerType)
        {
            GetStatusBarBehaviour(playerType: playerType).FadeIn();
        }

        public void Hide(EPlayerType playerType)
        {
            GetStatusBarBehaviour(playerType: playerType).FadeOut();
        }


		public string GetPlayerName(EPlayerType playerType) => GetStatusBarBehaviour(playerType: playerType).PlayerName;

		public void SetPlayerNameForPlayer1(string playerName) => _statusBarPlayer1.SetPlayerName(playerName: playerName);

		public void SetPlayerNameForPlayer2(string playerName) => _statusBarPlayer2.SetPlayerName(playerName: playerName);


		public EMoveOrderType GetMoveOrderType(EPlayerType playerType) => GetStatusBarBehaviour(playerType: playerType).GetMoveOrderType();


		public void SetLife(int count)
        {
            _statusBarPlayer1.SetLife(count: count);
        }

        public void SetRate(int rate)
        {
            _statusBarPlayer1.SetRatePlayer1(rate: rate);
        }



        public void SetTimeLeft(EPlayerType playerType, int minute, int second)
        {
            GetStatusBarBehaviour(playerType: playerType).SetTimeLeft(minute: minute, second: second);
        }


        public void SetTimeLeftText(EPlayerType playerType, string text)
        {
            GetStatusBarBehaviour(playerType: playerType).SetTimeLeftText(text: text);
        }




        public void SetGradeRankTypeForPlayer1(EMoveOrderType moveOrderType, EGradeRankType gradeRankType, EBoardType boardType)
        {
            _statusBarPlayer1.SetMoveOrder(moveOrderType: moveOrderType, gradeRankType: gradeRankType, boardType: boardType);

            if (moveOrderType == EMoveOrderType.FIRST)
            {
                _statusBarPlayer1.SetMoveOrderOnOff(isOn: true);
            }
            else
            {
                _statusBarPlayer1.SetMoveOrderOnOff(isOn: false);
            }
        }



		public void SetGradeRankTypeForPlayer2(EMoveOrderType moveOrderType, EGradeRankType gradeRankType, EBoardType boardType)
		{
			_statusBarPlayer2.SetMoveOrder(moveOrderType: moveOrderType, gradeRankType: gradeRankType, boardType: boardType);

            if (moveOrderType == EMoveOrderType.FIRST)
            {
                _statusBarPlayer2.SetMoveOrderOnOff(isOn: true);
            }
            else
            {
                _statusBarPlayer2.SetMoveOrderOnOff(isOn: false);
            }
		}



        public void ChangeStatusBarModeForPlayer1(StatusBarBehaviour.EStatusBarMode statusBarMode)
        {
            _statusBarPlayer1.ChangeStatubBarMode(statusBarMode: statusBarMode);
        }



        public void SetStatusBarUserType(EStatusBarUserType userType)
        {
            _statusBarPlayer1.SetStatusBarUserType(userType: userType);
        }


        public void ChangePresentPlayer(EPlayerType playerType)
        {
            switch (playerType)
            {
                case EPlayerType.PLAYER1:
                    _statusBarPlayer1.SetMoveOrderOnOff(isOn: true);
                    _statusBarPlayer2.SetMoveOrderOnOff(isOn: false);
                    break;

                case EPlayerType.PLAYER2:
                    _statusBarPlayer1.SetMoveOrderOnOff(isOn: false);
                    _statusBarPlayer2.SetMoveOrderOnOff(isOn: true);
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }


        // ===============================================
        // MARK: - ● Name On StatusBar  ─────

        // 名前表示非表示の値を保存する
        // - 表示自体は変更されない
        public void SetValueIsNameShown(EPlayerType playerType, bool value)
        {
            switch (playerType)
            {
                case EPlayerType.PLAYER1:
                    _isNameShownForPlayer1 = value;
                    break;

                case EPlayerType.PLAYER2:
                    _isNameShownForPlayer2 = value;
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }


        public bool IsNameShown(EPlayerType playerType)
        {
            switch (playerType)
            {
                case EPlayerType.PLAYER1:
                    return _isNameShownForPlayer1;

                case EPlayerType.PLAYER2:
                    return _isNameShownForPlayer2;

                default:
                    throw new InvalidOperationException();
            }
        }

        public void ShowName(EPlayerType playerType)
        {
            switch (playerType)
            {
                case EPlayerType.PLAYER1:
                    _isNameShownForPlayer1 = true;
                    _statusBarPlayer1.UpdateNameDisplay();
                    break;

                case EPlayerType.PLAYER2:
                    _isNameShownForPlayer2 = true;
                    _statusBarPlayer2?.UpdateNameDisplay();
                    break;

                default:
                    throw new InvalidOperationException();
            }

            UpdateNameDisplay(playerType: playerType);
        }

        public void HideName(EPlayerType playerType)
        {
            switch (playerType)
            {
                case EPlayerType.PLAYER1:
                    _isNameShownForPlayer1 = false;
                    break;

                case EPlayerType.PLAYER2:
                    _isNameShownForPlayer2 = false;
                    break;

                default:
                    throw new InvalidOperationException();
            }

            UpdateNameDisplay(playerType: playerType);
        }


        private void UpdateNameDisplay(EPlayerType playerType)
        {
            // Player 2 のステータスバーが存在しない場合はあるので、その際には処理を行わない
            GetStatusBarBehaviour(playerType: playerType)?.UpdateNameDisplay();
        }



        // ===============================================
        // MARK: - ● Destroy  ─────

        public void Destroy()
        {
            _layouter.Destroy();

            UnityEngine.Object.Destroy(_statusBarPlayer1.gameObject);
            _statusBarPlayer1 = null;

            UnityEngine.Object.Destroy(_statusBarPlayer2.gameObject);
            _statusBarPlayer2 = null;
        }
	}
}