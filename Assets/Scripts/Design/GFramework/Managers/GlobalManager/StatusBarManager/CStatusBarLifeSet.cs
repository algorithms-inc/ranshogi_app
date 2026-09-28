using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;

namespace GFramework
{
    public class CStatusBarLifeSet
    {
        // ===============================================
        // MARK: - ● Property  ─────

        private bool _isShown = true;
        public bool IsShown => _isShown;

        private readonly int _lifeMax;

        private List<StatusBarLifeBehaviour> _statusbarLifeBehaviourList = new List<StatusBarLifeBehaviour>();

        

        // ===============================================
        // MARK: - ● Constructor ─────
        public CStatusBarLifeSet(Transform transform)
        {
            // 途中で変更できないように、コピーして設定する
            _lifeMax = Config.LifeMax;

            Setup(transform: transform);
        }



        // ===============================================
        // MARK: - ● Setup ─────

        private void Setup(Transform transform)
        {
            // 最初はライフなしで表示する
            StatusBarLifeBehaviour Support(int index)
            {
                var gameObject = new GameObject("Life"+ index.ToString());
                var resultStatusBarLifeBehaviour = gameObject.AddComponent<StatusBarLifeBehaviour>();
                gameObject.transform.parent = transform;
                resultStatusBarLifeBehaviour.SetupComponent(area: Locator.Instance.statusBarManager.Layouter.GetAreaLife(index: index).AsYReversed());

                return resultStatusBarLifeBehaviour;
            }

            for (int index = 0; index < _lifeMax; index++)
            {
                _statusbarLifeBehaviourList.Add(Support(index: index));
            }

            Hide();
        }



        // ===============================================
        // MARK: - ● Set Life ─────

        public void SetLife(int count)
        {
            Assert.IsTrue(0 <= count);

            // 設定されるライフ数は、表示最大数より多く設定されることはありうる
            int displayLifeCount = count <= _lifeMax ? count : _lifeMax;

            for (int index = 0; index < _lifeMax; index++)
            {
                if (index < displayLifeCount)
                {
                    _statusbarLifeBehaviourList[index].SetIsLifeBodyOn(value: true);
                }
                else
                {
                    _statusbarLifeBehaviourList[index].SetIsLifeBodyOn(value: false);
                }
            }
        }



        // ===============================================
        // MARK: - ● Show / Hide ─────

        public void Show()
        {
            if (_isShown) { return; }

            _statusbarLifeBehaviourList.ForEach(i => i.Show());

            _isShown = true;
        }


        public void Hide()
        {
            if (_isShown == false) { return; }

            _statusbarLifeBehaviourList.ForEach(i => i.Hide());

            _isShown = false;
        }


        public void FadeIn()
        {
            if (_isShown) { return; }

            _statusbarLifeBehaviourList.ForEach(i => i.FadeIn());

            _isShown = true;
        }


        public void FadeOut()
        {
            if (_isShown == false) { return; }

            _statusbarLifeBehaviourList.ForEach(i => i.FadeOut());

            _isShown = false;
        }



        // ===============================================
        // MARK: - ● Destroy ─────

        public void Destroy()
        {
            _statusbarLifeBehaviourList.Clear();
        }
    }
}
