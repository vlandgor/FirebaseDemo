using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirebaseSetup
{
    public class FirebaseDemoUI : MonoBehaviour
    {
        [SerializeField] private FirebaseInitializer _firebaseInitializer;
        
        [Header("UI References")]
        [SerializeField] private Button loginButton;
        [SerializeField] private Button writeDbButton;

        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;

        private FirebaseAuthService _authService;
        private FirebaseDatabaseService _dbService;
        private string _userId;
        
        private void Start()
        {
            _firebaseInitializer.FirebaseReady += FirebaseInitializer_FirebaseReady;
            
            loginButton.onClick.AddListener(OnLogin);
            writeDbButton.onClick.AddListener(OnWriteDatabase);
        }

        private void OnDestroy()
        {
            _firebaseInitializer.FirebaseReady -= FirebaseInitializer_FirebaseReady;
        }

        private void OnLogin()
        {
            _authService.SignIn(emailInput.text, passwordInput.text,
                onSuccess: id =>
                {
                    Debug.Log("Login successful, userId: " + id);
                    _userId = id;
                },
                onError: err =>
                {
                    Debug.LogError("Login failed: " + err);
                });
        }

        private void OnWriteDatabase()
        {
            if (string.IsNullOrEmpty(_userId))
            {
                Debug.LogError("You must log in first.");
                return;
            }

            _dbService.WriteTestValue(_userId);
        }
        
        private void FirebaseInitializer_FirebaseReady()
        {
            _authService = new FirebaseAuthService();
            _dbService = new FirebaseDatabaseService();
        }
    }
}