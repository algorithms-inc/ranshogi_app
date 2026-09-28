using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// Manager での生成を行う
// (6) 生成・Destroy


// TODO: Disabled にした場合、新たなインプットには反応しなくても、そのときインプットがあればOnイベントがファイヤーしてしまうと思われる。if で弾いているが、挙動確認すること。

namespace GFramework
{
    // ホバー状態での表示を再現するために IPointerEnterHandler, IPointerExitHandler を利用する場合、次のような点を考慮する必要がある。
    // - タップの場合、タップしながら対象上部を通過しても Enter / Exit イベントが発生する。
    // - しかし、Up / Down / Click は対象内かどうかの判定がからむ。
    // - そのため、あるボタンについて、ホバー表示を実装しても正しくクリックが判定できないことがある（例：ボタン外からボタンに侵入→ボタン表示を変更→タップを話しても、Up/Down/Clickイベントはおきない）。
    // - 次のような利用シーンは問題ない
    //   - Down 発生でボタン色を変更する→Exit 発生でボタン色を通常色に戻す ボタンの押下判定は Click で行う
    public abstract class AInputBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        // ===============================================
        // MARK: - ● Property ─────

        private bool _isEnabled = true;
        public bool IsEnabled => _isEnabled;


        // ===============================================
        // MARK: - ● Setup ─────

        // 継承先ではsetupComoponent()内で呼ぶこと
        protected void SetupBase(Vector2 size, Vector2 position, bool isEnabled)
        {
            Image imageComponent = gameObject.AddComponent<Image>();

            // 透明であっても判定を行うようにする
            imageComponent.alphaHitTestMinimumThreshold = 0.0f;

            // 非表示にする
            HideArea();

            // サイズ変更
            SetSize(size: size);

            // 位置変更
            SetPosition(position: position);

            // 有効・無効
            SetIsEnabled(value: isEnabled);
        }



        // ===============================================
        // MARK: - ● Size / Position ─────

        public void SetSize(Vector2 size)
        {
            GetComponent<RectTransform>().sizeDelta = size;
        }


        public void SetPosition(Vector2 position)
        {
            GetComponent<RectTransform>().localPosition = position;
        }

        public FArea GetAreaAsCanvasCoordinate()
        {
            var centerPoint = GetComponent<RectTransform>().position;
            var size = GetComponent<RectTransform>().sizeDelta;

            return new FArea(p1: centerPoint, p2: size, areaConstructorType: FArea.EAreaConstructorType.CENTER_SIZE);
        }



        // ===============================================
        // MARK: - ● Area Color / Show / Hide ─────

        // エリアの色を設定 (主にデバック表示用)
        public void SetColor(Vector3 color)
        {
            Image image = gameObject.GetComponent<Image>();

            float presentAlpha = image.GetAlpha();
            image.color = new UnityEngine.Color(r: color.x, g: color.y, b: color.z, a: presentAlpha);
        }


        // エリアを半透明で表示 (主にデバック表示用)
        public void ShowArea()
        {
            Image image = gameObject.GetComponent<Image>();

            image.SetAlpha(alpha: 0.5f);
        }


        public void HideArea()
        {
            Image image = gameObject.GetComponent<Image>();

            image.SetAlpha(alpha: 0.0f);
        }



        // ===============================================
        // MARK: - ● Enabled / Disabled ─────

        public void SetIsEnabled(bool value)
        {
            _isEnabled = value;
            
            Image imageComponent = gameObject.GetComponent<Image>();
            imageComponent.raycastTarget = _isEnabled;
        }



        // ===============================================
        // MARK: - ● On Event ─────

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isEnabled == false) { return; }

            MainOnPointerDown();
        }


        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isEnabled == false) { return; }

            MainOnPointerUp();
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isEnabled == false) { return; }

            MainOnPointerClick();
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isEnabled == false) { return; }

            MainOnPointerEnter();
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            // TODO: アプリ終了時に、OnPointerExitが走ることがあり、結果としてボタンでLocatorを引くことができず、エラーが出る。OnPointExitを廃止してもいいかも。
            if (_isEnabled == false) { return; }

            MainOnPointerExit();
        }


        // ===============================================
        // MARK: - ● On Event Main (abstruct) ─────

        protected abstract void MainOnPointerDown();
        protected abstract void MainOnPointerUp();
        protected abstract void MainOnPointerClick();
        protected abstract void MainOnPointerEnter();
        protected abstract void MainOnPointerExit();
    }
}