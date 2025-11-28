using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirebaseSetup
{
    public class FirebaseDemoUI : MonoBehaviour
    {
        [SerializeField] private FirebaseInitializer _firebaseInitializer;

        [SerializeField] private GameObject _loginPanel;
        [SerializeField] private GameObject _userPanel;
        
        [Header("Login UI")]
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private Button loginButton;
        [SerializeField] private TextMeshProUGUI _loginDebugText;

        [Header("Database Test")]
        [SerializeField] private Button writeDbButton;

        [Header("Profile Simulation UI")]
        [SerializeField] private TMP_InputField parentEmailInput;
        [SerializeField] private Button setAdultButton;
        [SerializeField] private Button setMinorButton;
        [SerializeField] private Button approveParentButton;
        [SerializeField] private TextMeshProUGUI _userDebugText;
        
        [Space]
        [SerializeField] private GameObject _parentalBlockPanel;

        private FirebaseAuthService _authService;
        private FirebaseDatabaseService _dbService;
        private string _userId;

        // -----------------------------
        // LIFECYCLE
        // -----------------------------
        private void Start()
        {
            if (_firebaseInitializer == null)
            {
                Debug.LogError("FirebaseInitializer reference not assigned in inspector!");
                return;
            }

            _firebaseInitializer.FirebaseReady += OnFirebaseReady;

            loginButton.onClick.AddListener(OnLogin);
            writeDbButton.onClick.AddListener(OnWriteDatabase);

            setAdultButton.onClick.AddListener(OnSetAdultProfile);
            setMinorButton.onClick.AddListener(OnSetMinorProfile);
            approveParentButton.onClick.AddListener(OnApproveParent);
        }

        private void OnDestroy()
        {
            if (_firebaseInitializer != null)
                _firebaseInitializer.FirebaseReady -= OnFirebaseReady;
        }

        // -----------------------------
        // FIREBASE INITIALIZED
        // -----------------------------
        private void OnFirebaseReady()
        {
            Debug.Log("Firebase is ready — creating services.");

            _authService = new FirebaseAuthService();
            _dbService = new FirebaseDatabaseService();
        }

        // -----------------------------
        // LOGIN
        // -----------------------------
        private void OnLogin()
        {
            if (_authService == null)
            {
                _loginDebugText.color = Color.red;
                _loginDebugText.text = "Auth service not ready yet. Wait for Firebase to initialize.";
                
                Debug.LogError("Auth service not ready yet. Wait for Firebase to initialize.");
                return;
            }

            _authService.SignIn(emailInput.text, passwordInput.text,
                onSuccess: id =>
                {
                    _userId = id;
                    
                    _userPanel.SetActive(true);
                    _loginPanel.SetActive(false);
                    
                    Debug.Log($"Login successful, userId: {id}");
                },
                onError: err =>
                {
                    _loginDebugText.color = Color.red;
                    _loginDebugText.text = $"Login failed: {err}";
                    
                    Debug.LogError("Login failed: " + err);
                });
        }

        // -----------------------------
        // TEST WRITE
        // -----------------------------
        private void OnWriteDatabase()
        {
            if (!EnsureUser()) return;

            _dbService.WriteTestValue(_userId);
        }

        // -----------------------------
        // PROFILE FLOWS
        // -----------------------------
        private void OnSetAdultProfile()
        {
            _parentalBlockPanel.SetActive(false);
            
            if (!EnsureUser()) return;

            _dbService.UpdateUserProfile(
                userId: _userId,
                ageGroup: "18+",
                requiresParentalConsent: false,
                parentApproved: true,
                onboardingStep: "tutorial");

            _loginDebugText.color = Color.black;
            _userDebugText.text = "Set profile -> Adult (no consent required).";
            Debug.Log("Set profile -> Adult (no consent required).");
        }

        private void OnSetMinorProfile()
        {
            _parentalBlockPanel.SetActive(true);
            
            if (!EnsureUser()) return;

            string parentEmail = parentEmailInput != null ? parentEmailInput.text : "";

            _dbService.UpdateUserProfile(
                userId: _userId,
                ageGroup: "13-17",
                requiresParentalConsent: true,
                parentApproved: false,
                onboardingStep: "waiting_for_parent",
                parentEmail: parentEmail);

            _loginDebugText.color = Color.black;
            _userDebugText.text = "Set profile -> Minor (waiting for parental consent).";
            Debug.Log("Set profile -> Minor (waiting for parental consent).");
        }

        private void OnApproveParent()
        {
            if (!EnsureUser()) return;

            _dbService.UpdateUserProfile(
                userId: _userId,
                ageGroup: "13-17",
                requiresParentalConsent: true,
                parentApproved: true,
                onboardingStep: "tutorial");

            _loginDebugText.color = Color.black;
            _userDebugText.text = "Parent approved -> onboardingStep = tutorial.";
            Debug.Log("Parent approved -> onboardingStep = tutorial.");
            
            _parentalBlockPanel.SetActive(false);
        }

        // -----------------------------
        // HELPERS
        // -----------------------------
        private bool EnsureUser()
        {
            if (string.IsNullOrEmpty(_userId))
            {
                _loginDebugText.color = Color.red;
                _userDebugText.text = "You must log in first.";
                Debug.LogError("You must log in first.");
                return false;
            }

            if (_dbService == null)
            {
                _loginDebugText.color = Color.red;
                _userDebugText.text = "Database service not ready yet.";
                Debug.LogError("Database service not ready yet.");
                return false;
            }

            return true;
        }
    }
}
