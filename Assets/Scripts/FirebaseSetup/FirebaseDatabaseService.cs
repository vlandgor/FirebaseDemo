using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

namespace FirebaseSetup
{
    public class FirebaseDatabaseService
    {
        private readonly DatabaseReference _root;

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

        public void UpdateUserProfile(
            string userId,
            string ageGroup,
            bool requiresParentalConsent,
            bool parentApproved,
            string onboardingStep,
            string parentEmail = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                Debug.LogError("UpdateUserProfile: userId is null or empty.");
                return;
            }

            var updates = new Dictionary<string, object>
            {
                { "ageGroup", ageGroup },
                { "requiresParentalConsent", requiresParentalConsent },
                { "parentApproved", parentApproved },
                { "onboardingStep", onboardingStep }
            };

            if (!string.IsNullOrEmpty(parentEmail))
            {
                updates["parentEmail"] = parentEmail;
            }

            string path = $"users/{userId}/profile";

            _root.Child(path).UpdateChildrenAsync(updates)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.LogError("UpdateUserProfile failed: " + task.Exception);
                    }
                    else
                    {
                        Debug.Log($"UpdateUserProfile successful → {path}");
                    }
                });
        }
    }
}
