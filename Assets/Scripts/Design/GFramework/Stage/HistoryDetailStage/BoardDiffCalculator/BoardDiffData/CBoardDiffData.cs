using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFramework
{
    public class CBoardDiffData
    {
        public CBoardDiffBoardData BoardData = new CBoardDiffBoardData();
        public CBoardDiffWaitingSpaceData WaitingSpaceDataPlayer1 = new CBoardDiffWaitingSpaceData(playerType: EPlayerType.PLAYER1);
        public CBoardDiffWaitingSpaceData WaitingSpaceDataPlayer2 = new CBoardDiffWaitingSpaceData(playerType: EPlayerType.PLAYER2);


        public CBoardDiffData()
        {
            // do nothing
        }

        public void Clear()
        {
            BoardData = new CBoardDiffBoardData();
            WaitingSpaceDataPlayer1 = new CBoardDiffWaitingSpaceData(playerType: EPlayerType.PLAYER1);
            WaitingSpaceDataPlayer2 = new CBoardDiffWaitingSpaceData(playerType: EPlayerType.PLAYER2);      
        }
    }
}