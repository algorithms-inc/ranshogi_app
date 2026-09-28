using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Threading;
using GFramework.Core;
using System.Threading.Tasks;

public class Auth : MonoBehaviour
{
    public Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser firebaseUser;
    private string _idToken = null;
    private bool idTokenExist = false;
    private User user = null;
    private bool userExist = false;
    public static Auth instance = null;

    public delegate void RegistrationDelegate();
    public delegate void ErrorDelegate(string errorMessage);


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializeFirebase();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

    }

    // Handle initialization of the necessary firebase modules:
    public void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Debug.Log("AuthStateChanged Called");
        if(auth.CurrentUser != null) { 
            Debug.Log("Sign in state now.");
            Debug.Log(auth.CurrentUser.UserId);
        } else {
            Debug.Log("Not Signin state now.");
        }
        if (auth.CurrentUser != firebaseUser)
        {
            bool signedIn = firebaseUser != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && firebaseUser != null)
            {
                Debug.Log("Signed out " + firebaseUser.UserId);
            }
            firebaseUser = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Authstate changed. Newly signed in " + firebaseUser.UserId);
                if (_idToken == null)
                {
                    TokenAsync(() => {}, (errorMessage) => {});
                }
            }
        }
    }

    public void TokenAsync(UnityAction OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        auth.CurrentUser.TokenAsync(true).ContinueWith(task2 =>
        {
            if (task2.IsCanceled)
            {
                Debug.LogError("TokenAsync was canceled.");
                return;
            }

            if (task2.IsFaulted)
            {
                Debug.LogError("TokenAsync encountered an error: " + task2.Exception);
                OnErrorHandler("TokenAsync encountered an error: " + task2.Exception);
                return;
            }

            SetIdToken(task2.Result);
            OnSuccessHandler();
            //Debug.Log(_idToken);
        });
    }

    public async Task<bool> RefreshToken() {
        firebaseUser = auth.CurrentUser;
        if (firebaseUser == null) {
            Debug.LogError("No user is signed in.");
            return false;
        }

        try {
            // trueを渡すことで、有効期限に関わらず強制的に最新のトークンを取得（更新）します
            string newToken = await firebaseUser.TokenAsync(true);
            SetIdToken(newToken);
            
            Debug.Log("FirebaseAuth token refreshed successfully.");
            return true;
        } catch (System.Exception e) {
            Debug.LogError($"Failed to refresh Firebase token: {e.Message}");
            return false;
        }
    }

    public void SigninWithCustomtokenAsync(string token, UnityAction OnSuccessHandler, UnityAction<string> OnErrorHandler)
    {
        auth.SignInWithCustomTokenAsync(token).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCustomTokenAsync was canceled.");
                OnErrorHandler("エラーが発生しました。しばらく時間を置いてからお試しください。code: SWCA1");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCustomTokenAsync encountered an error: " + task.Exception);
                OnErrorHandler("エラーが発生しました。しばらく時間を置いてからお試しください。code: SWCA2");
                return;
            }

            firebaseUser = task.Result.User;
            firebaseUser.TokenAsync(true).ContinueWith(task2 => {
                if (task2.IsCanceled)
                {
                    Debug.LogError("SignInWithCustomTokenAsync was canceled.");
                    return;
                }

                if (task2.IsFaulted)
                {
                    Debug.LogError("SignInWithCustomTokenAsync encountered an error: " + task2.Exception);
                    return;
                }

                SetIdToken(task2.Result);
                OnSuccessHandler();
            });
        });
    }

    public void SigninWithEmailAndPassword(string email, string password, UnityAction OnSuccessHandler, UnityAction<string> OnErrorHandler)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                OnErrorHandler("エラーが発生しました。時間をおいてからお試しください。(101)");
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                OnErrorHandler("メールアドレスかパスワードが違います。");
                return;
            }

            firebaseUser = task.Result.User;
            if(firebaseUser.IsEmailVerified) {
                firebaseUser.TokenAsync(true).ContinueWith(task2 => {
                    if (task2.IsCanceled)
                    {
                        OnErrorHandler("Error: code 201.");
                        Debug.LogError("TokenAsync was canceled.");
                        return;
                    }

                    if (task2.IsFaulted)
                    {
                        OnErrorHandler("メールアドレスかパスワードが違います。");
                        return;
                    }

                    SetIdToken(task2.Result);
                    OnSuccessHandler();
                });
            } else {
                Signout(false);
                OnErrorHandler("メールアドレスの確認が完了していません。");
            }
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                firebaseUser.DisplayName, firebaseUser.UserId);
        });
    }


    public bool isLoggedIn() {
        return auth.CurrentUser != null && idTokenExist;
    }

    public bool CurrentUserExist()
    {
        return auth.CurrentUser != null;
    }

    public bool UserExist()
    {
        return userExist;
    }

    public bool IdTokenExist()
    {
        return idTokenExist;
    }

    public string idToken() {
        return _idToken;
    }

    public void SetUserPayed() {
        user.isPayedUser = true;
    }

    public void SetShowNameOnRanking(bool showNameOnRanking) {
        user.showNameOnRanking = showNameOnRanking;
    }

    public void SetIdToken(string idToken) {
        _idToken = idToken;
        idTokenExist = true;
    }


    public User GetUser() {
        return user;
    }


    public void SetUser(User u) {
        user = u;
        userExist = true;
    }

    public void SetNewRating(int newRating, EBoardType boardType) {
        if(boardType == EBoardType.BOARD_66) {
            user.ratingRanshogi66 = newRating;
        } else {
            user.ratingShogi = newRating;
        }
    }

    public void Signout(bool withLoadingScene = true)
    {
        user = null;
        _idToken = null;
        idTokenExist = false;
        userExist = false;
        auth.SignOut();
        if(withLoadingScene) {
            SceneManager.LoadScene("Introduction");
        }
    }

    public void FirebaseSignout()
    {
        auth.SignOut();
         
    }

    public bool isUserRegistered() {
        return auth.CurrentUser.Email != null && auth.CurrentUser.Email != "";
    }


    public bool IsUserVerified() {
        return isUserRegistered() && auth.CurrentUser.IsEmailVerified;
    }

    public void RegisterUser(string email, string password, UnityAction onSuccessHandler, UnityAction<string> onErrorHandler) {
        CustomTokenRequest req = new CustomTokenRequest();
        Api.CustomToken(req, (customToken) => {
            Debug.Log("get custom token succeeded");
            Debug.Log(customToken.token);
            SigninWithCustomtokenAsync(customToken.token, () => {

                Debug.Log("login by custom token succeeded");
                Firebase.Auth.Credential credential;
                try {
                    credential = Firebase.Auth.EmailAuthProvider.GetCredential(email, password);
                } catch {
                    return;
                }
                Auth.instance.auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWith(task => {
                    if (task.IsCanceled) {
                        Debug.LogError("LinkWithCredentialAsync was canceled.");
                        onErrorHandler("登録処理がキャンセルされました。しばらく時間を置いてからお試しください。");
                    } else if (task.IsFaulted) {
                        //AlertViewController alertView = Instantiate(alertPrefab, canvas.transform, false);
                        Firebase.FirebaseException error = task.Exception.InnerExceptions[0] as Firebase.FirebaseException;
                        Debug.Log(error.ErrorCode);
                        Debug.Log(error.ToString());
                        onErrorHandler(FirebaseErrorCodeToMessage(error.ErrorCode));
                        //alertView.Init("" + error.ToString());
                    } else {
                        Firebase.Auth.FirebaseUser newUser = task.Result.User;
                        Debug.LogFormat("Credentials successfully linked to Firebase user: {0} ({1}) {2}",
                            newUser.DisplayName, newUser.UserId, newUser.IsEmailVerified);
                        SendEmailVerificationAsync(onSuccessHandler, onErrorHandler);
                        
                    }
                });
            }, (errorMessage) => {
                onErrorHandler(errorMessage);
            });
        }, (errorMessage) => {
            onErrorHandler(errorMessage);
        });
        return;
    }


    public void Reauthenticate(string email, string password, UnityAction OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        Firebase.Auth.Credential credential = Firebase.Auth.EmailAuthProvider.GetCredential(email, password);
        if (user != null) {
            user.ReauthenticateAsync(credential).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("ReauthenticateAsync was canceled.");
                    OnErrorHandler("エラーが発生しました。しばらく時間を置いてから再度お試しください。ErrorCode: RA1");
                    return;
                }
                if (task.IsFaulted) {
                    OnErrorHandler("エラーが発生しました。しばらく時間を置いてから再度お試しください。ErrorCode: RA2");
                    return;
                }
                OnSuccessHandler();

                Debug.Log("User reauthenticated successfully.");
            });
        } else {
            OnErrorHandler("エラーが発生しました。しばらく時間を置いてから再度お試しください。ErrorCode: RA3");
        }
    }

    public void SendEmailVerificationAsync(UnityAction OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        firebaseUser.SendEmailVerificationAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SendEmailVerificationAsync was canceled.");
                OnErrorHandler("エラーが発生しました。しばらく時間を置いてからお試しください。ErrorCode: SE1");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                OnErrorHandler("エラーが発生しました。しばらく時間を置いてからお試しください。ErrorCode: SE2");
                return;
            }
            OnSuccessHandler();
            //AlertViewController alertView = Instantiate(alertPrefab, canvas.transform, false);
            //alertView.Init("ご入力されたメールアドレスに認証用メールを送信しました。");
        });
    }

    public void ReloadUserAsync(UnityAction OnSuccessHandler, UnityAction<string> OnErrorHandler) {
        if (firebaseUser != null)
        {
            firebaseUser.ReloadAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("ReloadAsync was canceled.");
                    OnErrorHandler("エラーが発生しました。しばらく時間を置いてからお試しください。ErrorCode: RU1");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("ReloadAsync encountered an error: " + task.Exception);
                    OnErrorHandler("エラーが発生しました。しばらく時間を置いてからお試しください。ErrorCode: RU2");
                    return;
                }
                OnSuccessHandler();
            });
        }
        else
        {
        }
    }

    private string FirebaseErrorCodeToMessage(int code) {
        switch (code) {
            case 11:
                return "メールアドレスの形式が正しくありません。";
            case 8:
            case 15:
                return "このメールアドレスは使用されています。";
            case 23:
                return "パスワードは6文字以上の半角英数字記号にしてください。";
            case 37:
                return "メールアドレスは必須です。";
            default:
                return "エラーが発生しました。しばらく時間をおいてお試しください。ErrorCode: " + code;
        }
    }
}
