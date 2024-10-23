using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float initialGameSpeed = 5f;
    public float gameSpeedIncrease = 0.1f;
    public float gameSpeed { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hiscoreText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button exitButton;

    private Player player;
    private Spawner spawner;

    private float score;
    private float elapsedTime;
    public float Score => score;

    private int jumpCount;
    private int crouchCount;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        spawner = FindObjectOfType<Spawner>();

        NewGame();
    }

    public void NewGame()
    {
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();

        foreach (var obstacle in obstacles) {
            Destroy(obstacle.gameObject);
        }

        score = 0f;
        gameSpeed = initialGameSpeed;
        elapsedTime = 0f;
        jumpCount = 0;
        crouchCount = 0;
        enabled = true;

        player.ResetCounts();
        player.gameObject.SetActive(true);
        spawner.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);

        string timeFormatted = "00:00";

        UpdateScoreText(timeFormatted);
        UpdateHiscore();
    }

    public void GameOver()
    {
        gameSpeed = 0f;
        enabled = false;

        player.gameObject.SetActive(false);
        spawner.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);

        UpdateHiscore();
        SaveGameOverData();
    }

    private void Update()
    {
        gameSpeed += gameSpeedIncrease * Time.deltaTime;
        score += gameSpeed * Time.deltaTime;

        elapsedTime += Time.deltaTime; // Incrementar el tiempo transcurrido
        int minutes = Mathf.FloorToInt(elapsedTime / 60); // Calcular minutos
        int seconds = Mathf.FloorToInt(elapsedTime % 60); // Calcular segundos
        string timeFormatted = $"{minutes:00}:{seconds:00}"; // Formatear como MM:SS

        jumpCount = player.JumpCount; // Supone que has implementado estas propiedades en el script Player
        crouchCount = player.CrouchCount;

        UpdateScoreText(timeFormatted);
    }

    private void UpdateScoreText(string timeFormatted)
    {
        // Actualizar el texto del puntaje con los valores actuales de saltos y agachadas
        scoreText.text = $"{Mathf.FloorToInt(score):D5}\n{timeFormatted}\n{jumpCount:D2}\n{crouchCount:D2}";
    }

    private void UpdateHiscore()
    {
        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);
        int hiscoreJumps = PlayerPrefs.GetInt("hiscoreJumps", 0);
        int hiscoreCrouches = PlayerPrefs.GetInt("hiscoreCrouches", 0);
        float hiscoreTime = PlayerPrefs.GetFloat("hiscoreTime", 0);

        if (score > hiscore)
        {
            hiscore = score;
            hiscoreTime = elapsedTime; // Guardar el mejor tiempo si se bate el récord
            PlayerPrefs.SetFloat("hiscore", hiscore);
            PlayerPrefs.SetInt("hiscoreJumps", jumpCount);
            PlayerPrefs.SetInt("hiscoreCrouches", crouchCount);
            PlayerPrefs.SetFloat("hiscoreTime", hiscoreTime); // Guardar el mejor tiempo
        }

        int minutes = Mathf.FloorToInt(hiscoreTime / 60);
        int seconds = Mathf.FloorToInt(hiscoreTime % 60);
        string hiscoreTimeFormatted = $"{minutes:00}:{seconds:00}";

        hiscoreText.text = $"{Mathf.FloorToInt(hiscore):D5}\n{hiscoreTimeFormatted}\n{hiscoreJumps:D2}\n{hiscoreCrouches:D2}";
    }

    public void OnExitButtonClicked()
    {
        SceneManager.LoadScene("Datos"); // Cargar la escena llamada "Datos"
    }

    private void SaveGameOverData()
    {
    // Verificar si sessionFolderPath está inicializado
    string sessionFolderPath = null;

    if (!string.IsNullOrEmpty(EMGProtocol.sessionFolderPath))
    {
        sessionFolderPath = EMGProtocol.sessionFolderPath;
    }

    // Obtener el número de archivos existentes en el directorio para generar el siguiente nombre de archivo
    int fileIndex = Directory.GetFiles(sessionFolderPath, "GameOverData_*.txt").Length + 1;
    string filename = $"GameOverData_{fileIndex}.txt";
    
    // Obtener la ruta completa al archivo
    string path = Path.Combine(sessionFolderPath, filename);

    // Formatear el tiempo transcurrido
    int minutes = Mathf.FloorToInt(elapsedTime / 60);
    int seconds = Mathf.FloorToInt(elapsedTime % 60);
    string timeFormatted = $"{minutes:00}:{seconds:00}";

    // Preparar los datos para escribir en el archivo
    string data = $"Score: {Mathf.FloorToInt(score):D5}\n" +
                  $"Time: {timeFormatted}\n" +
                  $"Jumps: {jumpCount:D2}\n" +
                  $"Crouches: {crouchCount:D2}\n" +
                  $"Threshold1: {ArduinoReaderUI.threshold1}\n" +
                  $"Threshold2: {ArduinoReaderUI.threshold2}\n";

    // Escribir los datos en el archivo
    File.WriteAllText(path, data); // Guardar los datos en un archivo único

    Debug.Log("Datos de GameOver guardados en: " + path);
    }
}




