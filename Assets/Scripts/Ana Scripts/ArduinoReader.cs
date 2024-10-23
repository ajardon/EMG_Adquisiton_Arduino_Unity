using System.Collections.Generic;  // Necesario para List<T>
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class ArduinoReaderUI : MonoBehaviour
{
    public string portName = "COM4";
    public int baudRate = 9600;
    private SerialPort serialPort;
    public static bool isArduinoConnected = false;

    public static int numSamples = 10;

    // Señal 1
    public static int[] smoothedValues1;
    public static List<int> allSmoothedValues1 = new List<int>();  // Para almacenar todos los datos durante la calibración
    public static int currentIndex1 = 0;
    public static int threshold1 = 0;
    public static bool thresholdCalculated1 = false;
    public static int timeElapsed1 = 0;
    public static int smoothedValue1 = 0;

    // Señal 2
    public static int[] smoothedValues2 = null;
    public static List<int> allSmoothedValues2 = new List<int>();  // Para almacenar todos los datos durante la calibración
    public static int currentIndex2;
    public static int threshold2;
    public static bool thresholdCalculated2 = false;
    public static int timeElapsed2 = 0;
    public static bool isSecondSignalActive = false;
    public static int smoothedValue2 = 0;

    // Variables para la detección del máximo local con ventana deslizante
    public static Queue<int> windowSignal1 = new Queue<int>();
    public static Queue<int> windowSignal2 = new Queue<int>();
    public static int windowSize = 30 ; // Tamaño de la ventana para la detección de picos
    public static int maxSignalValue1 = 0;
    public static int maxSignalValue2 = 0;
    public static bool isContracted1 ;
    public static bool isContracted2 ;

     private float lastContracted1Time = -1f;

    public Image circleRed1, circleRed2, circleGreen1, circleGreen2, circleYellow1, circleYellow2;
    public TMP_Text connectionStatusText;

    private float contractionDuration = 0.5f; // Duración para mantener la contracción (en segundos)
    private float contractionTimer = 0f;

    public Button nextSceneButton; // Botón para cambiar de escena

    void Start()
    {
        TryOpenPort();
        connectionStatusText.text = "Conectando...";
        smoothedValues1 = new int[numSamples];
        smoothedValues2 = new int[numSamples];

        nextSceneButton.onClick.AddListener(LoadNextScene);

        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string dataString = serialPort.ReadLine();
                string[] dataValues = dataString.Split(','); // Asumiendo que ambas señales vienen separadas por una coma

                if (dataValues.Length >= 1)
                {
                    // Procesamiento de la primera señal
                    int rawValue1 = int.Parse(dataValues[0]);
                    smoothedValue1 = CalcularMediaMovil(rawValue1, smoothedValues1, ref currentIndex1);
                    allSmoothedValues1.Add(smoothedValue1);  // Almacenar el valor suavizado
                    Debug.Log("Valor suavizado 1 (int): " + smoothedValue1);

                    if (!thresholdCalculated1 && timeElapsed1 >= 60000)
                    {
                        threshold1 = CalcularUmbral(allSmoothedValues1.ToArray());  // Calcular umbral usando todos los datos
                        thresholdCalculated1 = true;
                        Debug.Log("Threshold 1 Calculated: " + threshold1);
                    }

                    // Añadir el valor suavizado a la ventana de la señal 1
                    windowSignal1.Enqueue(smoothedValue1);
                    if (windowSignal1.Count > windowSize)
                    {
                        windowSignal1.Dequeue();
                    }

                    maxSignalValue1 = EsPico(windowSignal1, maxSignalValue1);

                    timeElapsed1 += (int)(Time.deltaTime * 1000);
                }

                // Procesamiento de la segunda señal (si la primera señal ya ha sido calibrada)
                if (thresholdCalculated1 && dataValues.Length >= 2)
                {
                    if (!isSecondSignalActive)
                    {
                        // Inicializamos la señal 2 solo cuando comenzamos a recibir datos para ella
                        smoothedValues2 = new int[numSamples];
                        allSmoothedValues2.Clear();  // Limpiar la lista por si acaso
                        currentIndex2 = 0;
                        threshold2 = 0;
                        timeElapsed2 = 0;
                        isSecondSignalActive = true;
                    }

                    int rawValue2 = int.Parse(dataValues[1]);
                    smoothedValue2 = CalcularMediaMovil(rawValue2, smoothedValues2, ref currentIndex2);
                    allSmoothedValues2.Add(smoothedValue2);  // Almacenar el valor suavizado
                    Debug.Log("Valor suavizado 2 (int): " + smoothedValue2);

                    if (!thresholdCalculated2 && timeElapsed2 >= 60000)
                    {
                        threshold2 = CalcularUmbral(allSmoothedValues2.ToArray());  // Calcular umbral usando todos los datos
                        thresholdCalculated2 = true;
                        Debug.Log("Threshold 2 Calculated: " + threshold2);
                        SaveThresholds();
                    }

                    // Añadir el valor suavizado a la ventana de la señal 2
                    windowSignal2.Enqueue(smoothedValue2);
                    if (windowSignal2.Count > windowSize)
                    {
                        windowSignal2.Dequeue();
                    }

                    maxSignalValue2 = EsPico(windowSignal2, maxSignalValue2);

                    timeElapsed2 += (int)(Time.deltaTime * 1000);
                }

                // Comparar los picos detectados después de haber analizado las ventanas
                if (windowSignal1.Count == windowSize && windowSignal2.Count == windowSize)
                {

                    bool wasContracted1InLastSecond = (Time.time - lastContracted1Time) <= 1.0f;

                    if (maxSignalValue1 > threshold1 && !wasContracted1InLastSecond)
                    {
                        isContracted1 = true;
                        isContracted2 = false;
                        lastContracted1Time = Time.time;
                        contractionTimer = contractionDuration;
                        Debug.Log("Contracción atribuida a la Señal 1.");
                    }
                    if (maxSignalValue1 < threshold1 && maxSignalValue2 > threshold2 && maxSignalValue2 < 35 && HasDescendingValuesAfterPeak(windowSignal2, maxSignalValue2) && !wasContracted1InLastSecond)
                    {
                        isContracted1 = false;
                        isContracted2 = true;
                        contractionTimer = contractionDuration;
                        Debug.Log("Contracción atribuida a la Señal 2.");
                    }
                    

                    // Reiniciar los valores máximos para la próxima detección
                    maxSignalValue1 = 0;
                    maxSignalValue2 = 0;
                
                }
                if (contractionTimer > 0)
                {
                    contractionTimer -= Time.deltaTime;
                    if (contractionTimer <= 0)
                    {
                        isContracted1 = false;
                        isContracted2 = false;
                        Debug.Log("Contracción finalizada.");
                    }
                }

                if (!isArduinoConnected)
                {
                    isArduinoConnected = true;
                    Debug.Log("Arduino conectado y detectado!");
                    connectionStatusText.text = "Muscle Sensor V3 Conectado";
                    ChangeCircleColors(isArduinoConnected);
                }
                

            }
            catch (System.Exception ex)
            {
                if (isArduinoConnected)
                {
                    isArduinoConnected = false;
                    Debug.LogWarning("Arduino desconectado. Error: " + ex.Message);
                    connectionStatusText.text = "Muscle Sensor V3 Desconectado";
                    ChangeCircleColors(isArduinoConnected);
                }
            }
        }
        else
        {
            TryOpenPort();
        }
    }

    int EsPico(Queue<int> window, int currentMax)
    {
        // Encontrar el valor máximo en la ventana
        int maxInWindow = Mathf.Max(window.ToArray());
        // Devolver el máximo entre el valor máximo actual y el encontrado en la ventana
        return Mathf.Max(currentMax, maxInWindow);
    }

    bool HasDescendingValuesAfterPeak(Queue<int> signal, int peak)
    {
        bool peakReached = false;
        int descendingCount = 0;
        int lastValue = peak;

        foreach (var value in signal)
        {
            if (value == peak)
            {
                peakReached = true;
                continue;
            }

            if (peakReached)
            {
                if (value < lastValue)
                {
                    descendingCount++;
                    if (descendingCount >= 5)
                    {
                        return true; // Hay al menos 5 valores que descienden
                    }
                }
                else
                {
                    descendingCount = 0; // Resetear si un valor no es descendente
                }

                lastValue = value;
            }
        }

        return false; // No se encontraron 5 valores descendentes
    }

    void TryOpenPort()
    {
        try
        {
            if (serialPort == null || !serialPort.IsOpen)
            {
                serialPort = new SerialPort(portName, baudRate);
                serialPort.Open();
                Debug.Log("Intentando conectar al Arduino...");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("No se pudo abrir el puerto serial: " + ex.Message);
            connectionStatusText.text = "Conectando...";
            Invoke("TryOpenPort", 1);
        }
    }

    int CalcularMediaMovil(int rawValue, int[] smoothedValues, ref int currentIndex)
    {
        smoothedValues[currentIndex] = rawValue;
        currentIndex = (currentIndex + 1) % numSamples;

        int total = 0;
        for (int i = 0; i < numSamples; i++)
        {
            total += smoothedValues[i];
        }
        return total / numSamples;
    }

    int CalcularUmbral(int[] smoothedValues)
    {
        int sum = 0;
        int sumSq = 0;
        for (int i = 0; i < smoothedValues.Length; i++)
        {
            sum += smoothedValues[i];
            sumSq += smoothedValues[i] * smoothedValues[i];
        }
        int mean = sum / smoothedValues.Length;
        int variance = (sumSq / smoothedValues.Length) - (mean * mean);
        return mean + 2 * (int)Mathf.Sqrt(variance);  // Cambiado a int
    }

    void SaveThresholds()
    {
        if (string.IsNullOrEmpty(EMGProtocol.sessionFolderPath))
        {
            Debug.LogError("El path de la sesión no está definido.");
            return;
        }

        string filePath = Path.Combine(EMGProtocol.sessionFolderPath, "Thresholds.txt");
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Umbral Prehesion=" + threshold1);
            writer.WriteLine("Umbral Pinza=" + threshold2);
        }

        Debug.Log("Umbrales guardados en: " + filePath);
    }

    void ChangeCircleColors(bool isConnected)
    {
        float normalizedValue = isConnected ? 1f : 0f;

        Color newRedColor = Color.Lerp(new Color(1, 0, 0, 1), new Color(0.5f, 0, 0, 1), normalizedValue);
        circleRed1.color = newRedColor;
        circleRed2.color = newRedColor;

        Color newGreenColor = Color.Lerp(new Color(0, 1, 0, 1), new Color(0, 0.5f, 0, 1), normalizedValue);
        circleGreen1.color = newGreenColor;
        circleGreen2.color = newGreenColor;

        Color newYellowColor = Color.Lerp(new Color(1, 1, 0, 1), new Color(0.5f, 0.5f, 0, 1), normalizedValue);
        circleYellow1.color = newYellowColor;
        circleYellow2.color = newYellowColor;
    }

    public void LoadNextScene()
    {
        // Cambia el nombre "NextSceneName" por el nombre de la siguiente escena
        SceneManager.LoadScene("Game");
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}









