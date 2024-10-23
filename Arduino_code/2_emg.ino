void setup() {
  Serial.begin(9600);  // Inicia la comunicación serie a 9600 baudios
}

void loop() {
  int sensorValue1 = analogRead(A0);  // Lee el valor del pin A0
  int sensorValue2 = analogRead(A1);  // Lee el valor del pin A1

  // Envia los valores leídos separados por una coma
  Serial.print(sensorValue1);
  Serial.print(",");
  Serial.println(sensorValue2);

  delay(10);  // Espera un poco antes de la siguiente lectura
}
