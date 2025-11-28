using System;
using Firebase;
using Firebase.Extensions;
using UnityEngine;

namespace FirebaseSetup
{
    public class FirebaseInitializer : MonoBehaviour
    {
        public event Action FirebaseReady;
        
        [SerializeField] private FirebaseSettings settings;

        public static FirebaseApp App { get; private set; }

        private void Awake()
        {
            InitializeFirebase();
        }

        private void InitializeFirebase()
        {
            Debug.Log("Initializing Firebase...");

            var options = new AppOptions
            {
                ApiKey = settings.apiKey,
                AppId = settings.appId,
                ProjectId = settings.projectId,
                DatabaseUrl = new System.Uri(settings.databaseUrl)
            };

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    App = FirebaseApp.Create(options);
                    Debug.Log("Firebase initialized successfully.");
                    
                    FirebaseReady?.Invoke();
                }
                else
                {
                    Debug.LogError("Could not resolve Firebase dependencies: " + task.Result);
                }
            });
        }
    }
}
