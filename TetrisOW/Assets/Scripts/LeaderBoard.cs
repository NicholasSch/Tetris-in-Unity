using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using Dan.Main;

public class LeaderBoard : MonoBehaviour
{
    public static LeaderBoard Instance { get; private set; }

    [Header("Leaderboard Data Display")]
    [SerializeField] private List<TextMeshProUGUI> leaderboardRanks;
    [SerializeField] private List<TextMeshProUGUI> leaderboardNames;
    [SerializeField] private List<TextMeshProUGUI> leaderboardScores;

    [Header("Score Submission Panel")]
    [SerializeField] private TMP_InputField inputname;
    [SerializeField] private TextMeshProUGUI inputScore; 
    [SerializeField] private TextMeshProUGUI Scoretext;

    [Header("Language Toggles")]
    public bool English = true; 

    [SerializeField] private TextMeshProUGUI nametitle;
    [SerializeField] private TextMeshProUGUI ranktitle;
    [SerializeField] private TextMeshProUGUI scoretitle;

    [Header("Buttons")]
    [SerializeField] private Button submitButton;

    public UnityEvent Onback;
    
    private int finalScore = 0; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        submitButton.onClick.AddListener(() => UploadEntry(inputname.text, finalScore)); 

        ApplyLanguage();
        
        GetLeaderboard();

        inputname.interactable = false;
        submitButton.interactable = false;
    }

    public void SetScore(int score)
    {
        finalScore = score;
        inputScore.text = score.ToString();
        
        inputname.interactable = true;
        submitButton.interactable = true;
    }

    public void ApplyLanguage()
    {
        if (English)
        {
            inputname.placeholder.GetComponent<TextMeshProUGUI>().text = "Input name";
            Scoretext.text = "Current score";
            nametitle.text = "Name";
            ranktitle.text = "Rank";
            scoretitle.text = "Score";
        }
        else 
        {
            inputname.placeholder.GetComponent<TextMeshProUGUI>().text = "Inserir nick"; 
            Scoretext.text = "Pontuação atual";
            nametitle.text = "Nick";
            ranktitle.text = "Rank";
            scoretitle.text = "Score";
        }
    }

    private void GetLeaderboard()
    {
        foreach (var text in leaderboardNames) text.text = string.Empty;
        foreach (var text in leaderboardRanks) text.text = string.Empty;
        foreach (var text in leaderboardScores) text.text = string.Empty;
        
        Leaderboards.TetrisOWLeaderboard.GetEntries(msg =>
        {
            int looplength = Mathf.Min(msg.Length, leaderboardNames.Count);
            for (int i = 0; i < looplength; i++)
            {
                leaderboardRanks[i].text = msg[i].Rank.ToString();
                leaderboardNames[i].text = msg[i].Username;
                leaderboardScores[i].text = msg[i].Score.ToString();
            }
        });
    }

    private void UploadEntry(string name, int score)
    {
        Debug.Log("jorgeeee");
        if (string.IsNullOrWhiteSpace(name) || name == "Input name" || name == "Inserir nick")
        {
            Debug.LogWarning("Please enter a valid name before submitting.");
            return;
        }

        inputname.interactable = false;
        submitButton.interactable = false;
        
        Leaderboards.TetrisOWLeaderboard.UploadNewEntry(name, score, isSuccessful =>
        {
            if (isSuccessful)
            {
                Leaderboards.TetrisOWLeaderboard.ResetPlayer();

                GetLeaderboard();
                Debug.Log("Score successfully uploaded!");

                inputname.text = string.Empty; 
            }
            else
            {
                Debug.LogError("Failed to upload score. Enabling submission again.");
                inputname.interactable = true;
                submitButton.interactable = true;
            }
        });
        
    }
    
    

    public void OnPTButtonClick()
    {   
        English = false;
        ApplyLanguage();
    }

    public void OnENButtonClick()
    {
        English = true;
        ApplyLanguage();
    }
}