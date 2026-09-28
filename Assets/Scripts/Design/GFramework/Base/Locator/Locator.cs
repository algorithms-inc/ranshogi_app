using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GFramework.Core;

// Unity のエディタにおいてはプレイ中に生成されたオブジェクトは、次回のプレイ開始時およびエディタの終了時に破棄され、デストラクタが呼ばれる仕様となっている。
// アプリ終了(=エディタ内実行終了)タイミングで、デストラクタに変わる破壊処理を行うには、MonoBehaviourを継承して、OnDestory()を利用する必要がある。
// そのために、Locator を単なるクラスではなく、MonoBehaviour を継承したコンポーネントにし、GameObject に持たせる必要がある。
// シングルトンという性質をもたせつつ、シーンが変わっても破壊されないように、DontDestroyOnLoad に登録している。

namespace GFramework
{
    public sealed class Locator : MonoBehaviour
    {
        // Singleton Instance
        private static Locator locatorInstance;

        static bool _isOneceDestroyed = false;

        // Get Instance
        public static Locator Instance
        {
            get
            {
                if (locatorInstance == null)
                {
                    if (_isOneceDestroyed)
                    {
                        Debug.Log("Locator is already destroyed.");
                        return null;
                    }
                    else
                    {
                        var gameObjectLocator = new GameObject(name: "Locator");
                        locatorInstance = gameObjectLocator.AddComponent<Locator>();

                        Utl.SetDontDestroyOnLoad(targetObject: gameObjectLocator);
                    }
                }

                return locatorInstance;
            }
        }



        // Managers
        public IAppManager appManager;

        public IColorManager colorManager;

        public IFontManager fontManager;

        public ITextureManager textureManager;

        public IAudioManager audioManager;

        public IDesignManager designManager;

        public ILayoutManager layoutManager;

        public IBackgroundManager backgroundManager;

        public IUICreatorManager UICreatorManager;

        public IStatusBarManager statusBarManager;

        public IDebugManager debugManager;


        // Constructor
        private Locator()
        {
            // do nothing
        }

        public void SetAppManager(IAppManager appManager)
        {
            this.appManager = appManager;
        }


        void OnDestroy()
        {
            // TODO: その他のマネジャについても、破壊処理を行う
            appManager?.Destroy();
            textureManager?.Destroy();
            backgroundManager?.Destroy();
            audioManager?.Destroy();
            designManager?.Destroy();
            UICreatorManager?.Destroy();
            debugManager?.Destroy();

            _isOneceDestroyed = true;
        }
    }
}
