using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;

namespace FirebaseSetup
{
    public class FirebaseAuthService
    {
        private FirebaseAuth _auth;

        public FirebaseAuthService()
        {
            _auth = FirebaseAuth.GetAuth(FirebaseInitializer.App);
        }

        public void SignIn(string email, string password, Action<string> onSuccess, Action<string> onError)
        {
            _auth.SignInWithEmailAndPasswordAsync(email, password)
                .ContinueWithOnMainThread((Task<AuthResult> task) =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        onError?.Invoke(task.Exception?.GetBaseException().Message);
                        return;
                    }

                    var result = task.Result;
                    var user = result.User;

                    if (user != null)
                    {
                        onSuccess?.Invoke(user.UserId);
                    }
                    else
                    {
                        onError?.Invoke("Login result returned null user.");
                    }
                });
        }
    }
}