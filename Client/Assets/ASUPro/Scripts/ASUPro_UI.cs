using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ASUPro
{
    public class ASUPro_UI : ASUPro_Core {

        [Tooltip("This is our inputfields for our register page")]
        [Header("Register")]
        [SerializeField]
        InputField RegisterUsername;
        [SerializeField]
        InputField RegisterPassword;
        [SerializeField]
        InputField RegisterEmail;

        [Tooltip("This is our inputfields for our login page")]
        [Header("Login")]
        [SerializeField]
        InputField LoginUsername;
        [SerializeField]
        InputField LoginPassword;

        public void DoTryRegister()
        {
            //check to make sure information has been entered
            if (string.IsNullOrEmpty(RegisterUsername.text) || RegisterUsername.text == "Enter username")
            {
                //if username is invalid then print it 
                Debug.LogError("Invalid username");
                return;
            }
            if (string.IsNullOrEmpty(RegisterPassword.text) || RegisterPassword.text == "Enter password")
            {
                //if password is invalid then print it 
                Debug.LogError("Invalid password");
                return;
            }
            if (string.IsNullOrEmpty(RegisterEmail.text) || RegisterEmail.text == "Enter email")
            {
                //if Email is invalid then print it 
                Debug.LogError("Invalid email");
                return;
            }

            base.DoRegister(RegisterUsername.text, RegisterPassword.text, RegisterEmail.text);
            ResetValues();
        }

        public void DoTryLogin()
        {
            //check to make sure information has been entered
            if (string.IsNullOrEmpty(LoginUsername.text) || LoginUsername.text == "Enter username")
            {
                //if our username is invalid then print it
                Debug.LogError("Username is invalid");
                return;
            }
            if (string.IsNullOrEmpty(LoginPassword.text) || LoginPassword.text == "Enter password")
            {
                //if our Password is invalid then print it
                Debug.Log("Password is invalid");
                return;
            }
            //send the login to the core for proceeding
            base.DoLogin(LoginUsername.text, LoginPassword.text);
            //reset all values
            ResetValues();
        }

        //change panels to advoid collision we just do it this way less code and tidy.
        public void ChangePanel(int id)
        {
            switch (id)
            {
                case 0:
                    base.ToLogPanel();
                break;
                case 1:
                    base.ToRegPanel();
                break;
                case 2:
                    base.ToGamePanel();
                break;
            }
        }

        //reset all active data
        void ResetValues()
        {
            RegisterUsername.text = "Enter username";
            RegisterPassword.text = "Enter password";
            RegisterEmail.text = "Enter email";

            LoginUsername.text = "Enter username";
            LoginPassword.text = "Enter password";
        }
    }
}
