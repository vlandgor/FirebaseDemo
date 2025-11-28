using UnityEngine;

namespace FirebaseSetup
{
    [CreateAssetMenu(fileName = "FirebaseSettings", menuName = "Firebase/Settings")]
    public class FirebaseSettings : ScriptableObject
    {
        public string apiKey = "AIzaSyCKNq9gkeYs2XKHYdml2QRQ3fYlRLjRg3o";
        public string authDomain = "fir-demo-70b8e.firebaseapp.com";
        public string projectId = "fir-demo-70b8e";
        public string storageBucket = "fir-demo-70b8e.firebasestorage.com";
        public string messagingSenderId = "422578668752";
        public string appId = "1:422578668752:web:bf6f60c01dca840eddcad5";
        public string databaseUrl = "https://fir-demo-70b8e-default-rtdb.firebaseio.com";

    }
}