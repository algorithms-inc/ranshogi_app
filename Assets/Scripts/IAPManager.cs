using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Events;

public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IAPManager instance = null;

    protected static IStoreController storeController;
    protected static IExtensionProvider extensionProvider;

    public static string subscriptionProductId = "fun.algorithms.ranshogi.premium_user";

    protected UnityAction onSuccessHandler;
    protected UnityAction<Product> onRestoreSuccessHandler;
    protected UnityAction<string> onErrorHandler;
    protected HashSet<string> processedTransactions = new HashSet<string>();
    protected bool isInBuyingProcess = false;
    protected bool gotRestoreResult = false;


    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        if (storeController == null)
        {
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        Debug.Log("### InitializePurchasing.");
        if (IsInitialized())
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(subscriptionProductId, ProductType.Subscription);
        UnityPurchasing.Initialize(this, builder);
    }

    public void SetHandlers(UnityAction successHandler, UnityAction<Product> restoreSuccessHandler, UnityAction<string> errorHandler) {
        this.onSuccessHandler = successHandler;
        this.onRestoreSuccessHandler = restoreSuccessHandler;
        this.onErrorHandler = errorHandler;
    }

    private bool IsInitialized()
    {
        return storeController != null && extensionProvider != null;
    }

    public void BuySubscription()
    {
        if(Auth.instance.GetUser().isPayedUser) {
            onErrorHandler("すでにプレミアム会員です。");
            return;
        }
        BuyProductID(subscriptionProductId);
    }


    public Product GetProduct() {
        if (IsInitialized()) {
            return storeController.products.WithID(subscriptionProductId);
        } else {
            return null;
        }
    }

    public string GetPrice() {
        var product = GetProduct();
        if(product == null) { return "" ; }
        Debug.Log(product.metadata.isoCurrencyCode);
        if(product.metadata.isoCurrencyCode == "JPY") {

            return Decimal.ToInt32(product.metadata.localizedPrice).ToString() + "円";
        } else {
            return product.metadata.localizedPriceString;
        }
    }

    public void Restore()
    {

        var product = GetProduct();
        if(product != null && IsSubscribedTo(product)) {
            Debug.Log("2ですです is subscripted to です");
            this.onRestoreSuccessHandler(product);
            return; 
        }
        Debug.Log("##### no product subscription");

        isInBuyingProcess = false;
        gotRestoreResult = false;
        extensionProvider.GetExtension<IAppleExtensions> ().RestoreTransactions ((result, error) => {
            Debug.Log("restore result: " + result + " " + error);
            if (result) {

                StartCoroutine("WaitRestoring");
                

            } else {
                this.onErrorHandler("エラーが発生しました。");
                // Restoration failed.
            }
        });
    }

    IEnumerator WaitRestoring() {
        yield return new WaitForSeconds(5);
        if(gotRestoreResult) {
        } else {
            this.onErrorHandler("");
        }
    }

    void BuyProductID(string productId)
    {
        gotRestoreResult = false;
        isInBuyingProcess = true;
        if (IsInitialized())
        {
            Product product = GetProduct();
            if (product != null && product.availableToPurchase)
            {
                Debug.Log($"Purchasing product asychronously: {product.definition.id}");
                storeController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
            this.onErrorHandler("エラーが発生しました。しばらく時間を置いてから再度お試しください。");
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("###Successfully IAP has been initialized.");
        storeController = controller;
        extensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log($"OnInitializeFailed InitializationFailureReason:{error}, message:{message}");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"OnInitializeFailed InitializationFailureReason:{error}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureReason}");
        if(failureReason == PurchaseFailureReason.UserCancelled) {
            onErrorHandler("プレミアム会員登録処理がキャンセルされました。");
        } else {
            onErrorHandler($"プレミアム会員登録処理に失敗しました。 {failureReason}");
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log("#### In ProcessPurchase");
        gotRestoreResult = true;
        string transactionId = args.purchasedProduct.transactionID;
        if (processedTransactions.Contains(transactionId)) {
            return PurchaseProcessingResult.Complete;
        }
        processedTransactions.Add(transactionId);

        var product = GetProduct();
        if(!isInBuyingProcess && product != null && IsSubscribedTo(product)) {
            Debug.Log("ですです is subscripted to です");
            this.onRestoreSuccessHandler(product);
            return PurchaseProcessingResult.Complete;
        }

        if (String.Equals(args.purchasedProduct.definition.id, subscriptionProductId, StringComparison.Ordinal))
        {
            string receipt = args.purchasedProduct.receipt;
            SendReceipt(args.purchasedProduct, false);
            return PurchaseProcessingResult.Pending;
        }
        else
        {
            Debug.Log($"ProcessPurchase: FAIL. Unrecognized product: '{args.purchasedProduct.definition.id}'");
            onErrorHandler("プレミアム会員登録処理に失敗しました。しばらく時間を置いてからお試しください。");
            return PurchaseProcessingResult.Complete;
        }
    }

    public void SendReceipt(Product product, bool doRestore) {

        #if UNITY_ANDROID 
            GoogleAppReceiptRequest request = new GoogleAppReceiptRequest();
            GoogleAppReceiptRequest.Receipt receipt = new GoogleAppReceiptRequest.Receipt();
            receipt.receipt = product.receipt;
            receipt.do_restore = doRestore;
            request._requestJson = JsonUtility.ToJson(receipt);
            Debug.Log(receipt.receipt);
            var context = SynchronizationContext.Current;
            Api.RegisterGoogleSubscription(request, (receipt) => {
                context.Post((state2) => {
                    //AlertViewController alertView = Instantiate(alertPrefab, canvas.transform, false);
                    //alertView.Init("ニックネームを更新しました。");
                    if(doRestore) {
                        this.onSuccessHandler();
                        return;
                    }
                    storeController.ConfirmPendingPurchase(product);
                    if(receipt.receiptState == ReceiptState.ValidNewSubscription) {
                        this.onSuccessHandler();
                    } else if(receipt.receiptState == ReceiptState.ValidRestorable) {
                        this.onRestoreSuccessHandler(product);
                    } else {
                        this.onErrorHandler("購読に失敗しました。乱将棋お問い合わせより問い合わせください。");
                    } 
                    isInBuyingProcess = false;
                }, null);
            }, (errorMessage) => {
                context.Post((state2) => {
                    this.onErrorHandler(errorMessage);
                    isInBuyingProcess = false;
                }, null);
            });
        #endif

        #if UNITY_IOS 
            AppleAppReceiptRequest request = new AppleAppReceiptRequest();
            AppleAppReceiptRequest.Receipt receipt = new AppleAppReceiptRequest.Receipt();
            receipt.receipt = product.receipt;
            receipt.do_restore = doRestore;
            request._requestJson = JsonUtility.ToJson(receipt);
            Debug.Log(receipt.receipt);
            var context = SynchronizationContext.Current;
            Api.RegisterAppleSubscription(request, (receipt) => {
                context.Post((state2) => {
                    //AlertViewController alertView = Instantiate(alertPrefab, canvas.transform, false);
                    //alertView.Init("ニックネームを更新しました。");
                    if(doRestore) {
                        this.onSuccessHandler();
                        return;
                    }
                    storeController.ConfirmPendingPurchase(product);
                    if(receipt.receiptState == ReceiptState.ValidNewSubscription) {
                        this.onSuccessHandler();
                    } else if(receipt.receiptState == ReceiptState.ValidRestorable) {
                        this.onRestoreSuccessHandler(product);
                    } else {
                        this.onErrorHandler("購読に失敗しました。乱将棋お問い合わせより問い合わせください。");
                    } 
                    isInBuyingProcess = false;
                }, null);
            }, (errorMessage) => {
                context.Post((state2) => {
                    this.onErrorHandler(errorMessage);
                    isInBuyingProcess = false;
                }, null);
            });
        #endif
    }

    // 復元のDBの書き換えを行う処理
    public void DoRestore(Product product, UnityAction successHandler, UnityAction<string> errorHandler) {
        this.onSuccessHandler = successHandler;
        this.onErrorHandler = errorHandler;
        SendReceipt(product, true);
    }

    bool IsSubscribedTo(Product subscription)
    {
        // If the product doesn't have a receipt, then it wasn't purchased and the user is therefore not subscribed.
        if (subscription.receipt == null)
        {
            return false;
        }
        //The intro_json parameter is optional and is only used for the App Store to get introductory information.
        var subscriptionManager = new SubscriptionManager(subscription, null);

        // The SubscriptionInfo contains all of the information about the subscription.
        // Find out more: https://docs.unity3d.com/Packages/com.unity.purchasing@3.1/manual/UnityIAPSubscriptionProducts.html
        var info = subscriptionManager.getSubscriptionInfo();

        return info.isSubscribed() == Result.True;
    }
}