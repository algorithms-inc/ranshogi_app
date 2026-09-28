using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

namespace GFramework
{
    public class ImageTextBehaviour : ImageActionBehaviour
    {
        // ===============================================
        // MARK: - ● Property & Enum ─────

        public enum ETextAlign
        {
            LEFT,
            RIGHT
        }


        private protected int _designID;

        private protected int _maxLength;

        private protected EDirection _anchorDirection;

        private protected ETextAlign _textAlign;

        private protected string _presentText = "";

        private protected float[] _spacingRatioList;



        public string PresentText => _presentText;



        private bool _isShown = true;
        public bool IsShown => _isShown;

        public override float Alpha => GetAlpha(index: 0);


        // -----------------------------------------------
        // MARK: - Game Object

        protected SingleImageActionBehaviour[] imageTextSingleImageActionBehaviourList;


        // ===============================================
        // MARK: - ● Setup Component ─────

        public void SetupComponent(FImageTextInitializer imageTextInitializer)
        {
            _designID = imageTextInitializer.DesignID;
            _maxLength = imageTextInitializer.MaxLength;
            _textAlign = imageTextInitializer.TextAlign;
            _spacingRatioList = imageTextInitializer.SpacingRatioList;
            _anchorDirection = imageTextInitializer.AnchorDirection;

            imageTextSingleImageActionBehaviourList = new SingleImageActionBehaviour[imageTextInitializer.MaxLength];

            transform.position = new(0f, 0f, 0f);

            SetupGameObject(sortingOrder: imageTextInitializer.SortingOrder);

            Hide();
        }


        private void SetupGameObject(int sortingOrder)
        {
            var baseSize = Config.IMAGE_TEXT_CHAR__IMAGE_SIZE;


            float sumOfSpacing = baseSize.x * _spacingRatioList.Sum();

            Vector2 sizeOfTextArea = new(
                x: baseSize.x * _maxLength + sumOfSpacing,
                y: baseSize.y);
            var position = _anchorDirection switch
            {
                EDirection.UP => new Vector2(x: sizeOfTextArea.x / -2 + baseSize.x / 2, y: sizeOfTextArea.y / -2),
                EDirection.UP_RIGHT => new Vector2(x: sizeOfTextArea.x * -1 + baseSize.x / 2, y: sizeOfTextArea.y / -2),
                EDirection.RIGHT => new Vector2(x: sizeOfTextArea.x * -1 + baseSize.x / 2, y: 0),
                EDirection.DOWN_RIGHT => new Vector2(x: sizeOfTextArea.x * -1 + baseSize.x / 2, y: sizeOfTextArea.y / 2),
                EDirection.DOWN => new Vector2(x: sizeOfTextArea.x / -2 + baseSize.x / 2, y: sizeOfTextArea.y / 2),
                EDirection.DOWN_LEFT => new Vector2(x: baseSize.x / 2, y: sizeOfTextArea.y / 2),
                EDirection.LEFT => new Vector2(x: baseSize.x / 2, y: 0),
                EDirection.UP_LEFT => new Vector2(x: baseSize.x / 2, y: sizeOfTextArea.y / -2),
                EDirection.STAY => new Vector2(x: sizeOfTextArea.x / -2 + baseSize.x / 2, y: 0),
                _ => throw new InvalidOperationException(),
            };
            for (int index = 0; index <= imageTextSingleImageActionBehaviourList.Length - 1; index++)
            {
                var singleImageActionBehaviour = UtlSprite.CreateGameObjectWithSpriteAndSingleImageActionBehaviour(
                    name: "ImageText" + index.ToString(),
                    texturePathUnderResources: Locator.Instance.textureManager.GetFilePathOfImageText(designID: _designID, character: ' '),
                    willCacheIfNeeded: true,
                    sortingOrder: sortingOrder,
                    transformParent: transform,
                    initializer: new FSingleImageActionBehaviourInitializer(
                        defaultAlpha: 1,
                        durationFadeIn: Config.ANIMATION_DURATION_2,
                        durationFadeOut: Config.ANIMATION_DURATION_2)
                );

                singleImageActionBehaviour.transform.position = position / Config.PIXEL_PER_UNIT;

                // 最終文字の場合には Spacing の値がないため、スキップする
                if (index < imageTextSingleImageActionBehaviourList.Length - 1)
                {
                    position.x += baseSize.x * (1.0f + _spacingRatioList[index]);
                }

                imageTextSingleImageActionBehaviourList[index] = singleImageActionBehaviour;

                _presentText += " ";
            }
        }



        // ===============================================
        // MARK: - ● Validation ─────

        protected bool IsValidIndex(int index) =>
            (0 <= index && index <= _maxLength - 1);



        // ===============================================
        // MARK: - ● Get / Set Alpha ─────

        public float GetAlpha(int index)
        {
            Assert.IsTrue(IsValidIndex(index: index));

            return imageTextSingleImageActionBehaviourList[index].Alpha;
        }


        public override void SetAlpha(float alpha)
        {
            for (int index = 0; index <= imageTextSingleImageActionBehaviourList.Length - 1; index++)
            {
                SetAlpha(index: index, alpha: alpha);
            }
        }


        public void SetAlpha(int index, float alpha)
        {
            Assert.IsTrue(IsValidIndex(index: index));

            imageTextSingleImageActionBehaviourList[index].SetAlpha(alpha: alpha);
        }


        // ===============================================
        // MARK: - ● Set Sorting Order ─────

        protected void SetSortingOrder(int sortingOrder)
        {
            // 配列は LINQ の ForEach を利用できない
            foreach (var i in imageTextSingleImageActionBehaviourList)
            {
                UtlSprite.SetSortingOrder(gameObject: i.gameObject, sortingOrder: sortingOrder);
            }
        }



        // ===============================================
        // MARK: - ● Set Text ─────

        public void SetText(int index, char character)
        {
            Assert.IsTrue(IsValidIndex(index: index));

            if (character != _presentText[index])
            {
                string texturePathUnderResources = Locator.Instance.textureManager.GetFilePathOfImageText(designID: _designID, character: character);

                UtlSprite.ChangeTexture(gameObject: imageTextSingleImageActionBehaviourList[index].gameObject, texturePathUnderResources: texturePathUnderResources, willCacheIfNeeded: true);

                _presentText.ReplaceAt(index: index, character: character);
            }
        }


        public void SetText(string text)
        {
            string newText = text;
            int deltaCountOfTextLength = _maxLength - newText.Length;

            if (deltaCountOfTextLength >= 1)
            {
                string addingBlank = "";

                for (int i = 1; i <= deltaCountOfTextLength; i++)
                {
                    addingBlank += " ";
                }

                switch (_textAlign)
                {
                    case ETextAlign.LEFT:
                        newText += addingBlank;
                        break;

                    case ETextAlign.RIGHT:
                        newText = addingBlank + newText;
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            if (deltaCountOfTextLength <= -1)
            {
                Debug.Log($"Can't show full text of '{newText}' (maxLength = {_maxLength})");
            }

            
            for (int index = 0; index < _maxLength; index++)
            {
                if (newText[index] != _presentText[index])
                {
                    string texturePathUnderResources = Locator.Instance.textureManager.GetFilePathOfImageText(designID: _designID, character: newText[index]);

                    UtlSprite.ChangeTexture(gameObject: imageTextSingleImageActionBehaviourList[index].gameObject, texturePathUnderResources: texturePathUnderResources, willCacheIfNeeded: true);
                }
            }

            _presentText = newText;
        }


        public void ClearText()
        {
            SetText(text: "");
        }



        // ===============================================
        // MARK: - ● Change Color ─────

        // 色を変更する（正確には掛け合わせることで、特定の要素を変更する）
        public void MultipleColor(int index, FColor3 color3)
        {
            Assert.IsTrue(IsValidIndex(index: index));

            float presentAlpha = GetAlpha(index: index);

            imageTextSingleImageActionBehaviourList[index].gameObject.GetComponent<SpriteRenderer>().material.color = color3.AsSystemColor(a: presentAlpha);
        }


        public void MultipleColor(FColor3 color3)
        {
            for (int index = 0; index <= imageTextSingleImageActionBehaviourList.Length - 1; index++)
            {
                MultipleColor(index: index, color3: color3);
            }
        }


        // ===============================================
        // MARK: - ● Show / Hide ─────

        public override void Show()
        {
            if (_isShown) { return; };

            // 配列は LINQ の ForEach を利用できない
            foreach (var i in imageTextSingleImageActionBehaviourList)
            {
                i.Show();
            }

            _isShown = true;
        }


        public override void Hide()
        {
            if (_isShown == false) { return; };

            // 配列は LINQ の ForEach を利用できない
            foreach (var i in imageTextSingleImageActionBehaviourList)
            {
                i.Hide();
            }

            _isShown = false;
        }



        public override void FadeIn(Action actionOnDidFinish = null)
        {
            if (_isShown) { return; };

            // 配列は LINQ の ForEach を利用できない
            foreach (var i in imageTextSingleImageActionBehaviourList)
            {
                i.FadeIn();
            }

            _isShown = true;
        }


        public override void FadeOut(Action actionOnDidFinish = null)
        {
            if (_isShown == false) { return; };

            // 配列は LINQ の ForEach を利用できない
            foreach (var i in imageTextSingleImageActionBehaviourList)
            {
                i.FadeOut();
            }

            _isShown = false;
        }



        // ===============================================
        // MARK: - ● On Destroy ─────

        private new void OnDestroy()
        {
            base.OnDestroy();

            Array.Clear(imageTextSingleImageActionBehaviourList, 0, imageTextSingleImageActionBehaviourList.Length);
        }
    }
}