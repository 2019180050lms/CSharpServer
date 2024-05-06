using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASUPro
{
    public class ASUPro_Core : MonoBehaviour
    {
        //This is the url for the Registering php script
        [Tooltip("Register url, this directs to your webhost")]
        [SerializeField]
        string RegisterUrl = "http://127.0.0.1/ASUPro/Register.php";

        //this is the url for the Login php script
        [Tooltip("Login url, this directs to your webhost")]
        [SerializeField]
        string LoginUrl = "http://127.0.0.1/ASUPro/Login.php";

        [Tooltip("Panels direct you through stages, we use this with the new UI instead of using enums")]
        [Header("Panels")]
        [SerializeField] GameObject LoginPanel;
        [SerializeField] GameObject RegisterPanel;
        [SerializeField] GameObject GamePanel;

        //We attempt to register our details to our host
        public void DoRegister(string username, string password, string email)
        {
            //create a new form
            WWWForm form = new WWWForm();
            //this is our username data we send
            form.AddField("username", username);
            //this is our password data we send
            form.AddField("password", password);
            //this is our email data we send
            form.AddField("email", email);
            //submit the data to our script
            WWW w = new WWW(RegisterUrl, form);
            //start our coroutine to check if we had a response
            StartCoroutine(RegisterPlayer(w));
        }

        //while we are sending the register details, we debug the result
        IEnumerator RegisterPlayer(WWW w)
        {
            //get our register response
            yield return w;
        
            //check if its successful if so proceed
            if (w.text.Contains("Successfully"))
            {
                //our login panel
                ToLogPanel();
            }
        }

        //we attempt to login with our selected details
        public void DoLogin(string username, string password)
        {
            //create new form
            WWWForm form = new WWWForm();
            //this is our username data
            form.AddField("username", username);
            //this is our password data
            form.AddField("password", password);
            //submit the form as a new form to our script
            WWW w = new WWW(LoginUrl, form);
            //wait for a response
            StartCoroutine(Login(w));
        }

        //while we have sent our login details to the php script we wait for a response
        IEnumerator Login(WWW w)
        {
            //our response
            yield return w;

            //wait for a response, if successful then proceed
            if (w.text.Contains("Successfully"))
            {
                //to our game panel
                ToGamePanel();
            }
        }

        //direct to Register panel
        public void ToRegPanel()
        {
            //enable register
            RegisterPanel.SetActive(true);
            //disable login
            LoginPanel.SetActive(false);
            //disable game
            GamePanel.SetActive(false);
        }

        //direct to login panel
        public void ToLogPanel()
        {
            //disable register
            RegisterPanel.SetActive(false);
            //enable login
            LoginPanel.SetActive(true);
            //disable game
            GamePanel.SetActive(false);
        }

        //direct to game panel, once logged in it directs you here.
        public void ToGamePanel()
        {
            //disable register
            RegisterPanel.SetActive(false);
            //disable login
            LoginPanel.SetActive(false);
            //enable game
            GamePanel.SetActive(true);
        }
    }
}