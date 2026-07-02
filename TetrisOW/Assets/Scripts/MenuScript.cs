using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameMenuManager : MonoBehaviour
{
    public enum Difficulty { Easy, Medium, Hard }
    public enum MenuState { MainMenu, DifficultySelection } 
    
    public static Difficulty selectedDifficulty = Difficulty.Medium;
    
    [Header("Buttons (Assign 3 identical buttons)")]
    public Button button1;
    public Button button2;
    public Button button3;

    private TextMeshProUGUI button1Text;
    private TextMeshProUGUI button2Text;
    private TextMeshProUGUI button3Text;

    [Header("Scene Names")]
    public string gameSceneName = "Game";
    public string leaderboardSceneName = "LeaderboardScene";
    public string settingsSceneName = "SettingsScene";

    private MenuState currentMenuState = MenuState.MainMenu;

    private void Awake()
    {
        button1Text = button1.GetComponentInChildren<TextMeshProUGUI>();
        button2Text = button2.GetComponentInChildren<TextMeshProUGUI>();
        button3Text = button3.GetComponentInChildren<TextMeshProUGUI>();

        selectedDifficulty = Difficulty.Medium;
    }

    private void Start()
    {
        ShowMainMenu();
    }

    private void ShowMainMenu()
    {
        currentMenuState = MenuState.MainMenu;
        
        button1.onClick.RemoveAllListeners();
        button2.onClick.RemoveAllListeners();
        button3.onClick.RemoveAllListeners();
        
        if (button1Text != null) button1Text.text = "START GAME";
        if (button2Text != null) button2Text.text = "LEADERBOARD";
        if (button3Text != null) button3Text.text = "SETTINGS";

        button1.onClick.AddListener(ShowDifficultyMenu);
        button2.onClick.AddListener(LoadLeaderboardScene);
        button3.onClick.AddListener(LoadSettingsScene);
    }

    private void ShowDifficultyMenu()
    {
        currentMenuState = MenuState.DifficultySelection;
        
        button1.onClick.RemoveAllListeners();
        button2.onClick.RemoveAllListeners();
        button3.onClick.RemoveAllListeners();
        
        if (button1Text != null) button1Text.text = "EASY";
        if (button2Text != null) button2Text.text = "MEDIUM";
        if (button3Text != null) button3Text.text = "HARD";

        button1.onClick.AddListener(() => StartGame(Difficulty.Easy));
        button2.onClick.AddListener(() => StartGame(Difficulty.Medium));
        button3.onClick.AddListener(() => StartGame(Difficulty.Hard));
    }

    private void StartGame(Difficulty difficulty)
    {
        selectedDifficulty = difficulty;
        Debug.Log("Starting game with difficulty: " + selectedDifficulty);
        SceneManager.LoadScene(gameSceneName); 
    }
    
    private void LoadLeaderboardScene()
    {
        Debug.Log("Loading Leaderboard scene...");
        SceneManager.LoadScene(leaderboardSceneName);
    }
    
    private void LoadSettingsScene()
    {
        Debug.Log("Loading Settings scene...");
        SceneManager.LoadScene(settingsSceneName);
    }
}