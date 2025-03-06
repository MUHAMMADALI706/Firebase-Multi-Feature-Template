using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
//using Assets.SimpleGoogleSignIn.Scripts;
using System.Collections;
using Firebase;
using Firebase.Auth;
using System.Text.RegularExpressions;
using UnityEngine.Rendering;

public class AuthController : MonoBehaviour
{

    [Header("Utilities")]
    public TMP_Text aError;
    public TMP_Text aHeading;
    public TMP_Text aPageToggleText;

    [Header("Auth Fields")]
    public TMP_InputField aUsername;
    public TMP_InputField aPassword;
    public TMP_InputField aReEnterPassword;
    public TextMeshProUGUI aTriggerButton;
    //public GameObject aForgetPassword;

    [Header("Other Fields")]
    public GameObject loginOrRegisterPanel;
    public GameObject logoutPanel;

    private string mAuthType;

    void Start()
    {
        this.mAuthType=AuthConstant.sAuthTypeLogin;
        onInit(null,null);
    }
    public void Logout()
    {
        FirebaseManager.Instance.OnLogout();
        mAuthType=AuthConstant.sAuthTypeLogin;
        logoutPanel.SetActive(false);
        loginOrRegisterPanel.SetActive(true);
        onInit(null,null);
    }
    public void onInit(Dictionary<string, object> pData, Action<object> callback)
    {
        //this.mAuthType = (string)pData.GetValueOrDefault(AuthKey.sAuthType, AuthConstant.sAuthTypeSignup);
        if (this.mAuthType == AuthConstant.sAuthTypeLogin)
        {
            aHeading.text = "Login";
            aTriggerButton.text = "Continue";
            aPageToggleText.text = "New user? Register new account";
            //aForgetPassword.SetActive(true);
            logoutPanel.SetActive(false);
        }
        else if (this.mAuthType == AuthConstant.sAuthTypeSignup)
        {
            aHeading.text = "Register";
            aTriggerButton.text = "next";
            aPageToggleText.text = "Already have an acount?";
            //aForgetPassword.SetActive(false);
            aReEnterPassword.gameObject.SetActive(true);
            logoutPanel.SetActive(false);
        }
    }
    public void SwitchBetweenLoginOrRegister()
    {
        if (this.mAuthType == AuthConstant.sAuthTypeLogin)
        {
            this.mAuthType=AuthConstant.sAuthTypeSignup;
        }
        else if (this.mAuthType == AuthConstant.sAuthTypeSignup)
        {
            this.mAuthType=AuthConstant.sAuthTypeLogin;
        }
        aError.text="";
        onInit(null,null);
    }


    public void onSignIn()
    {
        print("sign in");
        loginOrRegisterPanel.SetActive(false);
        logoutPanel.SetActive(true);
    }
    
    public void OnTrigger()
    {
        if (aUsername.text == "")
        {
            aError.text = "Invalid or emtpy email";
            return;
        }
        else if (aPassword.text.Length < 8)
        {
            aError.text = "Password: minimum 8 characters";
            return;
        }
        //aError.text="";
        if (this.mAuthType == AuthConstant.sAuthTypeLogin)
        {
            Action mCallBackEmailNotVerified = () =>
            {
                string message="Email is still not Verified";
                print(message);
                aError.text = message;
                aError.gameObject.SetActive(false);
                aError.gameObject.SetActive(true);
                return;
            };
            Action mCallbackSuccess = () =>
            {
                onSignIn();
            };
            Action<FirebaseException> callbackFailure = (pError) =>
            {
                print(pError);
                var errorMessage = pError.InnerException != null
                   ? pError.InnerException.Message
                   : pError.Message;
                aError.text = ErrorManager.Instance.getTranslateError(errorMessage);

                // AuthError errorCode = (AuthError)pError.ErrorCode;
                // string message = "Login Failed!";
                // switch (errorCode)
                // {
                //     case AuthError.MissingEmail:
                //         message = "Missing Email";
                //         break;
                //     case AuthError.MissingPassword:
                //         message = "Missing Password";
                //         break;
                //     case AuthError.WrongPassword:
                //         message = "Wrong Password";
                //         break;
                //     case AuthError.InvalidEmail:
                //         message = "Invalid Email";
                //         break;
                //     case AuthError.UserNotFound:
                //         message = "Account does not exist";
                //         break;
                // }
                // aError.text = message;
            };

            print("login");
            FirebaseManager.Instance.OnTryLogin(this.aUsername.text, this.aPassword.text, mCallbackSuccess, mCallBackEmailNotVerified, callbackFailure);
        }
        else if (this.mAuthType == AuthConstant.sAuthTypeSignup)
        {
            // if (!IsPasswordValid(aPassword.text))
            // {
            //     aError.text = "Password must include digit, uppercase and lowercase";
            //     aError.gameObject.SetActive(true);
            //     return;
            // }
            if (aPassword.text != aReEnterPassword.text)
            {
                aError.text = "Password does't match";
                aError.gameObject.SetActive(true);
                return;
            }
            Action callbackSuccess = () =>
            {
                string message="Verification send on mail. Verifiy it to login";
                print(message);
                aError.text = message;
                aError.gameObject.SetActive(false);
                aError.gameObject.SetActive(true);
            };

            Action<AggregateException> callbackFailure = (pError) =>
            {
                var errorMessage = pError.InnerException != null
                   ? pError.InnerException.Message
                   : pError.Message;
                aError.text = ErrorManager.Instance.getTranslateError(errorMessage);
            };

            FirebaseManager.Instance.OnTryRegisterNewAccount(this.aUsername.text, this.aPassword.text, callbackSuccess, callbackFailure);
        }
    }


    public void OnForgotPassword()
    {
        if (aUsername.text == "")
        {
            aError.text = "Invalid or emtpy email";
            return;
        }
        Action callbackSuccess = () =>
        {
            Action callbackSuccess = () =>
            {
                Application.OpenURL("mailto:");
            };

        };
        Action<FirebaseException> callbackFailure = (pError) =>
        {
            string errorMessage = pError?.InnerException != null
               ? pError.InnerException.Message
               : pError?.Message ?? "An unknown error occurred.";
            aError.text = ErrorManager.Instance.getTranslateError(errorMessage);
            
        };
        FirebaseManager.Instance.OnTryPasswordReset(aUsername.text, callbackSuccess, callbackFailure);

    }

    public bool IsPasswordValid(string password)
    {
        bool hasNumber = Regex.IsMatch(password, @"\d");       // Checks for a digit (0-9)
        bool hasLower = Regex.IsMatch(password, @"[a-z]");     // Checks for a lowercase letter (a-z)
        bool hasUpper = Regex.IsMatch(password, @"[A-Z]");     // Checks for an uppercase letter (A-Z)

        return hasNumber && hasLower && hasUpper;
    }



}