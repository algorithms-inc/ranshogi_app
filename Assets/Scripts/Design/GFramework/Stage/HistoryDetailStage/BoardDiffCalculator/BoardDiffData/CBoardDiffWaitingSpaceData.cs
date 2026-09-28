using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GFramework
{
    public class CBoardDiffResultForWaitingSpace
    {
        public List<FBoardDiffResultForWaitingSpaceElement> DataList = new List<FBoardDiffResultForWaitingSpaceElement>();

        public List<string> AsStringList()
        {
            var resultList = new List<string>();

            foreach (var data in DataList)
            {
                resultList.Add(data.AsString());
            }

            return resultList;
        }

        public string AsString()
        {
            return string.Join(" ", AsStringList());
        }

        public CBoardDiffResultForWaitingSpace()
        {
            // do nothing
        }
    }


    public class CBoardDiffWaitingSpaceData
    {
        private EPlayerType _playerType;

        private int _countFUHYOU   = 0;
        private int _countKYOUSHA  = 0;
        private int _countKEIMA    = 0;
        private int _countGINSHOU  = 0;
        private int _countKINSHOU  = 0;
        private int _countKAKUGYOU = 0;
        private int _countHISHA    = 0;

        public string AsString()
        {
            return "" + _playerType + "歩:" + _countFUHYOU + " 香:" + _countKYOUSHA + " 桂:" + _countKEIMA + " 銀:" + _countGINSHOU + " 金:" + _countKINSHOU + " 角:" + _countKAKUGYOU + " 飛:" + _countHISHA;
        }

        public CBoardDiffWaitingSpaceData(EPlayerType playerType)
        {
            _playerType = playerType;
        }
        

        public void Increment(EPieceType pieceType, int delta = 1)
        {
            Assert.IsTrue(delta > 0);

            switch (pieceType)
            {
                case EPieceType.FUHYOU:
                    _countFUHYOU += delta;
                    break;
                case EPieceType.KYOUSHA:
                    _countKYOUSHA += delta;
                    break;
                case EPieceType.KEIMA:
                    _countKEIMA += delta;
                    break;
                case EPieceType.GINSHOU:
                    _countGINSHOU += delta;
                    break;
                case EPieceType.KINSHOU:
                    _countKINSHOU += delta;
                    break;
                case EPieceType.KAKUGYOU:
                    _countKAKUGYOU += delta;
                    break;
                case EPieceType.HISHA:
                    _countHISHA += delta;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        public int GetCount(EPieceType pieceType)
        {
            Assert.IsTrue(pieceType != EPieceType.OUSHOU);

            switch (pieceType)
            {
                case EPieceType.FUHYOU:
                    return _countFUHYOU;
                case EPieceType.KYOUSHA:
                    return _countKYOUSHA;
                case EPieceType.KEIMA:
                    return _countKEIMA;
                case EPieceType.GINSHOU:
                    return _countGINSHOU;
                case EPieceType.KINSHOU:
                    return _countKINSHOU;
                case EPieceType.KAKUGYOU:
                    return _countKAKUGYOU;
                case EPieceType.HISHA:
                    return _countHISHA;
                default:
                    throw new InvalidOperationException();
            }
        }


        public CBoardDiffResultForWaitingSpace CompareAndCopy(CBoardDiffWaitingSpaceData other)
        {
            var result = Compare(other: other);
            Copy(other: other);

            return result;
        }


        private void Copy(CBoardDiffWaitingSpaceData other)
        {
            Assert.IsTrue(_playerType == other._playerType);

            _countFUHYOU   = other._countFUHYOU;
            _countKYOUSHA  = other._countKYOUSHA;
            _countKEIMA    = other._countKEIMA;
            _countGINSHOU  = other._countGINSHOU;
            _countKINSHOU  = other._countKINSHOU;
            _countKAKUGYOU = other._countKAKUGYOU;
            _countHISHA    = other._countHISHA;
        }


        // 差分があった駒の種類とその数を返す
        private CBoardDiffResultForWaitingSpace Compare(CBoardDiffWaitingSpaceData other)
        {
            Assert.IsTrue(_playerType == other._playerType);
        
            var result = new CBoardDiffResultForWaitingSpace();
        
            if (_countFUHYOU != other._countFUHYOU) { result.DataList.Add(new FBoardDiffResultForWaitingSpaceElement(EPieceType.FUHYOU, other._countFUHYOU - _countFUHYOU));}
            if (_countKYOUSHA != other._countKYOUSHA) { result.DataList.Add(new FBoardDiffResultForWaitingSpaceElement(EPieceType.KYOUSHA, other._countKYOUSHA - _countKYOUSHA));}
            if (_countKEIMA != other._countKEIMA) { result.DataList.Add(new FBoardDiffResultForWaitingSpaceElement(EPieceType.KEIMA, other._countKEIMA - _countKEIMA));}
            if (_countGINSHOU != other._countGINSHOU) { result.DataList.Add(new FBoardDiffResultForWaitingSpaceElement(EPieceType.GINSHOU, other._countGINSHOU - _countGINSHOU));}
            if (_countKINSHOU != other._countKINSHOU) { result.DataList.Add(new FBoardDiffResultForWaitingSpaceElement(EPieceType.KINSHOU, other._countKINSHOU - _countKINSHOU));}
            if (_countKAKUGYOU != other._countKAKUGYOU) { result.DataList.Add(new FBoardDiffResultForWaitingSpaceElement(EPieceType.KAKUGYOU, other._countKAKUGYOU - _countKAKUGYOU));}
            if (_countHISHA != other._countHISHA) { result.DataList.Add(new FBoardDiffResultForWaitingSpaceElement(EPieceType.HISHA, other._countHISHA - _countHISHA));}
        
            return result;
        }
    }
}