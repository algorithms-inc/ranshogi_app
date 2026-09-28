using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace GFramework
{
    public class ActionBehaviour : MonoBehaviour
    {
        // ===============================================
        // MARK: - ● Property ─────

        private readonly List<CAction> _actionList = new();

        public bool HasAction => _actionList.Count != 0;


        // ===============================================
        // MARK: - ● Method ─────

        public void UpdateAction()
        {
            if (HasAction)
            {
                _actionList[0].Update();
            }
        }


        public void PurgeAction(CAction fromAnimation)
        {
            Assert.IsTrue(HasAction);
            Assert.IsTrue(_actionList[0] == fromAnimation);

            _actionList.RemoveAt(0);
        }


        public void RegisterAction(CAction action) => _actionList.Add(action);


        // この処理では、進行中のアニメーションは強制終了されない
        // 強制終了するには、ForceFinishActionIfNeeded() を呼ぶこと
        public void ClearAction() => _actionList.Clear();


        public bool ForceFinishActionIfNeeded()
        {
            if (HasAction == false) { return false; }

            _actionList[0].ForceFinishAction();
            return true;
        }


        // オーバーライドする場合、base での呼び出しを忘れないこと。
        public void Update() => UpdateAction();


        // オーバーライドする場合、base での呼び出しを忘れないこと。
        protected virtual void OnDestroy() => ClearAction();
    }
}