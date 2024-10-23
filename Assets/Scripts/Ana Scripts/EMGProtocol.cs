using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO; // Necesario para la funcionalidad de archivos
using TMPro;

public class EMGProtocol : MonoBehaviour
{
    public InputField patientNameInput;
    public InputField sessionNumberInput;
    public InputField amputationLevelInput;
    public TMP_Dropdown amputationTypeDropdown;
    public Button saveButton; // Añadimos una referencia al botón de guardar
    public Button selectionButton; // Añadimos una referencia al botón de guardar
    public Button nextButton; // Añadimos una referencia al botón de guardar



    private string patientName;
    private string sessionNumber;
    private string amputationLevel;
    private string amputationType;

    private string nextSceneName;
    public static string sessionFolderPath;

    void Start()
    {
        // Add listener for the dropdown value change
        amputationTypeDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(amputationTypeDropdown);
        });

        // Add listener for the save button click
        saveButton.onClick.AddListener(SaveData);  

        selectionButton.onClick.AddListener(SelectAmputationType);   

        nextButton.onClick.AddListener(GoToNextScene);   

        DontDestroyOnLoad(this.gameObject);

        
    }

    public void SaveData()
    {

        if (string.IsNullOrEmpty(amputationType))
        {
            Debug.LogError("No se ha seleccionado un tipo de amputación");
            return;
        }

        patientName = patientNameInput.text;
        sessionNumber = sessionNumberInput.text;
        amputationLevel = amputationLevelInput.text;
        amputationType = amputationTypeDropdown.options[amputationTypeDropdown.value].text;


        // Crear el contenido para el archivo de texto
        string data = "Patient Name: " + patientName + "\n" +
                      "Session Number: " + sessionNumber + "\n" +
                      "Amputation Level: " + amputationLevel + "\n" +
                      "Amputation Type: " + amputationType + "\n";


        string baseFolderPath = @"C:\Users\almgp\OneDrive\Documentos\UC3M\TFM";

        // Crear la carpeta del paciente
        string patientFolderPath = Path.Combine(baseFolderPath, patientName);
        if (!Directory.Exists(patientFolderPath))
        {
            Directory.CreateDirectory(patientFolderPath);
        }

        // Guardar los datos del paciente en su carpeta
        string filePath = Path.Combine(patientFolderPath, "PatientData.txt");
        File.WriteAllText(filePath, data);

        // Crear la carpeta de la sesión para almacenar datos en las siguientes escenas
        sessionFolderPath = Path.Combine(patientFolderPath, "Session_" + sessionNumber);
        if (!Directory.Exists(sessionFolderPath))
        {
            Directory.CreateDirectory(sessionFolderPath);
        }

        Debug.Log("Data saved: " + data);
        Debug.Log("Patient file saved at: " + filePath);
        Debug.Log("Session folder created at: " + sessionFolderPath);
    }


    public void SelectAmputationType()
    {
        amputationType = amputationTypeDropdown.options[amputationTypeDropdown.value].text;

        // Determine the next scene based on the type of amputation
        switch (amputationType)
        {
            case "Muñeca":
                nextSceneName = "Otros";
                break;
            case "Transradial":
                nextSceneName = "Transradial";
                break;
            case "Codo":
                nextSceneName = "Otros";
                break;
            case "Transhumeral":
                nextSceneName = "Otros";
                break;
            default:
                Debug.LogError("Tipo de amputación no reconocido");
                break;
        }

        Debug.Log("Selected amputation type: " + amputationType + ", Next scene: " + nextSceneName);
    }

    public void GoToNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("No se ha seleccionado una escena válida");
        }
    }


    void DropdownValueChanged(TMP_Dropdown change)
    {
        amputationType = change.options[change.value].text;
        Debug.Log("Dropdown value changed: " + amputationType);
    }
}

