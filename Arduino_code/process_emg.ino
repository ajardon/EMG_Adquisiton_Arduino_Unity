const int sensorPin = A0;  // Pin donde está conectado el sensor
const int numSamples = 10; // Número de muestras para la media móvil
int rawValues[numSamples]; // Array para almacenar las muestras
int currentIndex = 0;      // Índice actual en el array
float smoothedValue = 0;   // Valor suavizado
float threshold = 0;       // Umbral calculado

unsigned long startTime = 0;
const unsigned long ignoreTime = 5000;  // Ignorar los primeros 5 segundos
const unsigned long captureTime = 60000; // Tiempo para capturar el umbral (1 minuto)

// Variables para el cálculo de media y desviación estándar
float sum = 0;
float sumSq = 0;
int sampleCount = 0;

void setup() {
  Serial.begin(9600);Serial.println("Señal_Raw\tSeñal_Smooth\tThreshold");
  startTime = millis();
}

void loop() {
  // Leer el valor bruto del sensor
  int rawValue = analogRead(sensorPin);

  // Añadir la nueva muestra y quitar la más antigua
  rawValues[currentIndex] = rawValue;
  currentIndex = (currentIndex + 1) % numSamples;

  // Calcular la media móvil
  float total = 0;
  for (int i = 0; i < numSamples; i++) {
    total += rawValues[i];
  }
  smoothedValue = total / numSamples;

  // Solo capturar datos después de ignorar los primeros 5 segundos
  if (millis() - startTime > ignoreTime && millis() - startTime <= captureTime) {
    sum += smoothedValue;
    sumSq += smoothedValue * smoothedValue;
    sampleCount++;
  }

  // Fijar el umbral después de 1 minuto (excluyendo los primeros 5 segundos)
  if (millis() - startTime > captureTime + ignoreTime && !threshold) {
    float mean = sum / sampleCount;
    float variance = (sumSq / sampleCount) - (mean * mean);
    float stddev = sqrt(variance);

    // Establecer el umbral como la media más 2 desviaciones estándar
    threshold = mean + 2 * stddev;
  }

  // Comparar con el umbral para detectar contracción
  bool isContracted = smoothedValue > threshold;

  // Enviar los valores al Monitor Serial con etiquetas claras
  Serial.print(rawValue);
  Serial.print(" ");
  Serial.print(smoothedValue);
  Serial.print(" ");
  Serial.println(threshold);


  // Espera 100 milisegundos antes de la siguiente lectura
  delay(100);
}
