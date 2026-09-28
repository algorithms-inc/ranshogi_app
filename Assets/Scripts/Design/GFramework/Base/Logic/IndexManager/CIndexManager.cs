using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CIndexManager
{
    // 常にソートしておくこと
    private List<int> _indexList = new();

    public string AsString()
    {
        string str = "conunt: " + _indexList.Count + ", list: [";

        foreach (var index in _indexList)
        {
            str += index + " ";
        }

        str += "]";

        return str;
    }


    private void AddIndex(int index)
    {
        Assert.IsTrue(IsUniqueIndex(index));

        _indexList.Add(index);

        // ソートする
        _indexList.Sort();
    }


    private bool IsUniqueIndex(int index)
    {
        return !_indexList.Contains(index);
    }


    public void RemoveIndex(int index)
    {
        Assert.IsTrue(_indexList.Contains(index));

        _indexList.Remove(index);
    }


    public void ClearIndex()
    {
        _indexList.Clear();
    }


    private int GetNextUnUsedIndex()
    {
        if (_indexList.Count == 0)
        {
            return 0;
        }
    
        for (int i = 0; i < _indexList.Count - 1; i++)
        {
            if (_indexList[i] + 1 < _indexList[i + 1])
            {
                return _indexList[i] + 1;
            }
        }
    
        return _indexList[_indexList.Count - 1] + 1;
    }


    // 未使用の最小 index を生成する
    public int GenerateNextIndex()
    {
        int index = GetNextUnUsedIndex();
        AddIndex(index);

        return index;
    }
}
