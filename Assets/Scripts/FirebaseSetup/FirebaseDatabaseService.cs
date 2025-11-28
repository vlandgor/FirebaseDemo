using System;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

namespace FirebaseSetup
{
    public class FirebaseDatabaseService
    {
        private DatabaseReference _root;

        public FirebaseDatabaseService()
        {
            _root = FirebaseDatabase.GetInstance(FirebaseInitializer.App).RootReference;
        }

        public void WriteTestValue(string userId)
        {
            string path = $"test/{userId}/timestamp";

            _root.Child(path).SetValueAsync(DateTime.UtcNow.ToString())
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.LogError("Database write failed: " + task.Exception);
                    }
                    else
                    {
                        Debug.Log("Database write successful → " + path);
                    }
                });
        }
    }
}