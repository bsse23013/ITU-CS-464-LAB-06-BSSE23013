using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Objective { CollectAll, ReachGoal, Survive, CollectThenGoal }
public enum GameState { Playing, LevelWon, GameWon, Lost }

// One GameManager sits in every level scene. The numbers for the level (coins needed,
// time limit, lives) are set in the Inspector, so each scene can have its own rules.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level settings")]
    [SerializeField] string levelName = "Level";
    [SerializeField] string brief = "";
    [SerializeField] Objective objective;
    [SerializeField] int targetScore;
    [SerializeField] float timeLimit;          // 0 = no timer
    [SerializeField] int startLives = 3;

    [Header("Player")]
    [SerializeField] Health playerHealth;
    [SerializeField] PlayerController playerController;

    [SerializeField] float respawnDelay = 0.6f;
    [SerializeField] float spawnProtection = 1.5f;

    public GameState State { get; private set; } = GameState.Playing;
    public int Score { get; private set; }
    public int Lives { get; private set; }
    public float TimeLeft { get; private set; }
    public bool Respawning { get; private set; }

    public Transform PlayerTransform => playerHealth.transform;

    public bool GoalUnlocked
    {
        get
        {
            if (objective == Objective.ReachGoal) return true;
            if (objective == Objective.CollectThenGoal) return Score >= targetScore;
            return false;
        }
    }

    Rigidbody playerBody;
    Vector3 spawnPoint;
    string lostReason;
    string toast;
    float toastUntil;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        playerBody = playerHealth.GetComponent<Rigidbody>();
        spawnPoint = playerHealth.transform.position;
        Lives = startLives;
        TimeLeft = timeLimit;
    }

    void Start()
    {
        playerHealth.Restore(spawnProtection);
        ShowToast(brief, 5f);
    }

    void Update()
    {
        if (GameInput.MenuPressed()) { SceneManager.LoadScene(0); return; }
        if (GameInput.RestartPressed()) { Restart(); return; }
        if (State != GameState.Playing || timeLimit <= 0f) return;

        TimeLeft -= Time.deltaTime;
        if (TimeLeft <= 0f)
        {
            TimeLeft = 0f;
            if (objective == Objective.Survive) Win();
            else Lose("Time ran out!");
        }
    }

    // ---------- rules ----------

    public void AddScore(int amount, float timeBonus = 0f)
    {
        if (State != GameState.Playing) return;
        Score += amount;

        if (timeBonus > 0f && timeLimit > 0f)
        {
            TimeLeft += timeBonus;
            ShowToast("+" + timeBonus + " seconds!");
        }

        if (objective == Objective.CollectAll && Score >= targetScore) Win();
        else if (objective == Objective.CollectThenGoal && Score == targetScore)
            ShowToast("Goal unlocked! Reach the green pad.", 3f);
    }

    public void ReachGoal()
    {
        if (State == GameState.Playing && GoalUnlocked) Win();
    }

    public void PlayerDied()
    {
        if (State != GameState.Playing || Respawning) return;
        Lives--;
        if (Lives <= 0) Lose("Out of lives!");
        else StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        Respawning = true;
        yield return new WaitForSeconds(respawnDelay);
        playerBody.position = spawnPoint;
        playerBody.linearVelocity = Vector3.zero;
        playerBody.angularVelocity = Vector3.zero;
        playerHealth.Restore(spawnProtection);
        Respawning = false;
    }

    void Win()
    {
        bool lastLevel = SceneManager.GetActiveScene().buildIndex >= SceneManager.sceneCountInBuildSettings - 1;
        State = lastLevel ? GameState.GameWon : GameState.LevelWon;
        Time.timeScale = 0f;
    }

    void Lose(string reason)
    {
        lostReason = reason;
        State = GameState.Lost;
        Time.timeScale = 0f;
    }

    void Restart() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    void NextLevel() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); }
    void ToMenu() { SceneManager.LoadScene(0); }

    public void ShowToast(string msg, float seconds = 2f)
    {
        toast = msg;
        toastUntil = Time.unscaledTime + seconds;
    }

    // ---------- HUD (OnGUI, so no Canvas needed) ----------

    GUIStyle big, label, toastStyle, boxStyle;

    void MakeStyles()
    {
        if (big != null) return;
        big = new GUIStyle(GUI.skin.label) { fontSize = 44, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        label = new GUIStyle(GUI.skin.label) { fontSize = 18, wordWrap = false, clipping = TextClipping.Clip };
        toastStyle = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, wordWrap = true };
        boxStyle = new GUIStyle(GUI.skin.box);
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.75f));
        tex.Apply();
        boxStyle.normal.background = tex;
    }

    string ObjectiveText()
    {
        switch (objective)
        {
            case Objective.CollectAll: return "Collect all the coins";
            case Objective.ReachGoal: return "Reach the green goal before time runs out";
            case Objective.Survive: return "Survive until the timer hits zero";
            default: return Score < targetScore ? "Collect every key, then reach the goal" : "Goal unlocked! Reach the green pad";
        }
    }

    void OnGUI()
    {
        MakeStyles();
        // everything is laid out for a 1280x720 screen and scaled to fit
        float s = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
        GUI.matrix = Matrix4x4.Scale(new Vector3(s, s, 1f));
        float w = Screen.width / s;
        float h = Screen.height / s;

        GUI.Box(new Rect(0, 0, w, 84), GUIContent.none, boxStyle);
        GUI.Label(new Rect(16, 6, w - 200, 26), levelName, label);
        GUI.Label(new Rect(16, 30, w - 200, 26), ObjectiveText(), label);

        string stats = "Hearts: " + Mathf.Max(playerHealth.Current, 0) + "/" + playerHealth.Max + "    Lives: " + Lives;
        if (targetScore > 0) stats += "    Coins: " + Score + "/" + targetScore;
        if (playerHealth.shielded) stats += "    Shield";
        if (playerController.BoostLeft > 0f) stats += "    Speed " + playerController.BoostLeft.ToString("0.0") + "s";
        GUI.Label(new Rect(16, 54, w - 200, 26), stats, label);

        if (timeLimit > 0f)
        {
            GUIStyle t = new GUIStyle(big) { fontSize = 30, alignment = TextAnchor.MiddleRight };
            GUI.Label(new Rect(w - 180, 20, 164, 44), TimeLeft.ToString("0.0") + "s", t);
        }

        if (Time.unscaledTime < toastUntil && State == GameState.Playing)
            GUI.Label(new Rect(w / 2 - 400, h - 80, 800, 64), toast, toastStyle);

        float cx = w / 2;
        if (State == GameState.LevelWon) EndPanel(cx, h, "LEVEL COMPLETE", "Well done.", "Next level", NextLevel, "Replay", Restart);
        else if (State == GameState.GameWon) EndPanel(cx, h, "YOU WIN!", "You finished all 5 levels.", "Play again", () => SceneManager.LoadScene(1), "Main menu", ToMenu);
        else if (State == GameState.Lost) EndPanel(cx, h, "LEVEL FAILED", lostReason, "Retry", Restart, "Main menu", ToMenu);
    }

    void EndPanel(float cx, float h, string head, string line, string b1, System.Action a1, string b2, System.Action a2)
    {
        float top = h / 2 - 110;
        GUI.Box(new Rect(cx - 300, top - 20, 600, 260), GUIContent.none, boxStyle);
        GUI.Label(new Rect(cx - 300, top, 600, 60), head, big);
        GUI.Label(new Rect(cx - 300, top + 70, 600, 36), line, toastStyle);
        if (GUI.Button(new Rect(cx - 240, top + 140, 220, 50), b1)) a1();
        if (GUI.Button(new Rect(cx + 20, top + 140, 220, 50), b2)) a2();
    }
}
