using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GFramework
{
    public class CBoardDiffBoardData
    {
        private List<FBoardDiffBoardDataElement> _dataList = new List<FBoardDiffBoardDataElement>();
        public List<FBoardDiffBoardDataElement> Data => _dataList;

        private List<bool> _checkList = new List<bool>();


        public List<string> AsStringList()
        {
            var result = new List<string>();

            foreach (var data in _dataList)
            {
                result.Add(data.AsString());
            }

            return result;
        }

        public CBoardDiffBoardData()
        {
            // do nothing
        }

        public void RegisterData(FBoardDiffBoardDataElement data)
        {
            Assert.IsTrue(IsPlayerTypeAndBoardCrdIsUnique(data));
            
            _dataList.Add(data);
            _checkList.Add(false);
        }

        // TODO: 実際には、PLAYERが異なっていても座標が裏返っていて重なることがありえるので、そのチェックも行いたい
        private bool IsPlayerTypeAndBoardCrdIsUnique(in FBoardDiffBoardDataElement targeData)
        {
            foreach (var data in _dataList)
            {
                if ((data.PlayerType == targeData.PlayerType) && (data.Crd == targeData.Crd))
                {
                    return false;
                }
            }

            return true;
        }


        public CBoardDiffResultForBoard CompareAndCopy(CBoardDiffBoardData other)
        {
            var result = Compare(other: other);
            Copy(other: other);

            return result;
        }


        private void Copy(CBoardDiffBoardData other)
        {
            _dataList.Clear();

            foreach (var data in other._dataList)
            {
                _dataList.Add(data);
            }
        }


        /// <summary>
        /// 差分があった駒の種類とその数を返す
        /// - this と other に同じデータがあれば無視をする
        /// - this にしかないデータは removeList に入れる
        /// - other にしかないデータは addList に入れる
        /// </summary>
        /// <param name="other">比較対象の CBoardDiffBoardData オブジェクト</param>
        /// <returns>差分結果を含む FBoardDiffResultForBoard オブジェクト</returns>
        private CBoardDiffResultForBoard Compare(CBoardDiffBoardData other)
        {
            var result = new CBoardDiffResultForBoard();

            var maxIndexOfThis = this.Data.Count - 1;

            // other を捜査して this と比べてゆく

            foreach (var otherDataElement in other.Data)
            {
                var hasSameCrdPiece = false;

                
                for (int i = 0; i <= maxIndexOfThis; i++)
                {
                    var thisDataElement = this.Data[i];

                    // すでにチェック済みのものは無視する
                    if (IsChecked(index: i)) {
                        continue;
                        }

                    // 完全一致である場合、無視する
                    if ((otherDataElement.PlayerType == thisDataElement.PlayerType) &&
                        (otherDataElement.Crd == thisDataElement.Crd) &&
                        (otherDataElement.PieceType == thisDataElement.PieceType) &&
                        (otherDataElement.IsNari == thisDataElement.IsNari))
                    {
                        Check(index: i);
                        hasSameCrdPiece = true;
                        break;
                    }

                    // 同じ座標のものがあれば、Next を Add し、Present を Remove する
                    if (otherDataElement.Crd == thisDataElement.Crd)
                    {
                        result.AddList.Add(otherDataElement);
                        result.RemoveList.Add(thisDataElement);
                        Check(index: i);
                        hasSameCrdPiece = true;
                        break;
                    }
                }

                // 同じ座標のものがなかった場合、追加対象にする
                if (hasSameCrdPiece == false)
                {
                    result.AddList.Add(otherDataElement);
                }
            }


            // this の DataElement で Checked が false のものを Remove 対象にする

            for (int i = 0; i <= maxIndexOfThis; i++)
            {
                if (IsChecked(index: i) == false)
                {
                    result.RemoveList.Add(this.Data[i]);
                }
            }

            return result;
        }

        public void Check(int index)
        {
            _checkList[index] = true;
        }

        public bool IsChecked(int index)
        {
            return _checkList[index];
        }

        public void ClearCheckList()
        {
            _checkList = new List<bool>();

            var count = _dataList.Count;

            if (count == 0) { return; }
            
            for (int i = 0; i < count; i++)
            {
                _checkList.Add(false);
            }
        }
    }
}