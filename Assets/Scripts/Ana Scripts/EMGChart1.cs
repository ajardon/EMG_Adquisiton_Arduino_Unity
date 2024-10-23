using System.Collections;
using UnityEngine;
using XCharts.Runtime;

namespace XCharts.Example
{
    [DisallowMultipleComponent]
    public class EMGChart1 : MonoBehaviour
    {
        private LineChart chart;
        private Serie serie1;
        private Serie serie2;
        private int timeElapsed = 0;

        private void OnEnable()
        {
            StartCoroutine(PieDemo());
        }

        IEnumerator PieDemo()
        {
            chart = gameObject.GetComponent<LineChart>();
            if (chart == null)
            {
                chart = gameObject.AddComponent<LineChart>();
                chart.Init();
            }

            RectTransform rectTransform = chart.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(400, 150);

            var title = chart.GetChartComponent<Title>();
            chart.GetChartComponent<Title>().text = "EMG Adquisition - Prehensión";

            var yAxis = chart.GetChartComponent<YAxis>();
            yAxis.minMaxType = Axis.AxisMinMaxType.Custom;
            yAxis.min = 0;
            yAxis.max = 400;

            var xAxis = chart.GetChartComponent<XAxis>();
            xAxis.type = Axis.AxisType.Value;

            chart.RemoveData();
            serie1 = chart.AddSerie<Line>("Line 1");
            serie1.symbol.show = false;

            while (!ArduinoReaderUI.isArduinoConnected)
            {
                yield return null;  // Espera a que el Arduino se conecte
            }

            Debug.Log("Arduino detectado. Comenzando a graficar.");

            while (timeElapsed <= 60005)
            {
                AddDataPoints();
                timeElapsed += 100;
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitUntil(() => ArduinoReaderUI.threshold1 != 0);

            // Una vez que threshold1 se ha calculado correctamente, añadir la línea horizontal
            AddHorizontalLine(ArduinoReaderUI.threshold1, 60000);
        }

        void AddDataPoints()
        {
            chart.AddXAxisData(timeElapsed.ToString() + "ms" );
            chart.AddData(serie1.index, ArduinoReaderUI.smoothedValues1[ArduinoReaderUI.currentIndex1]);
            Debug.Log("Graficando valor suavizado 1 : " + ArduinoReaderUI.smoothedValues1[ArduinoReaderUI.currentIndex1]);
        }

        void AddHorizontalLine(int value, int endTime)
        {
            serie2 = chart.AddSerie<Line>("Line 2");
            serie2.symbol.show = false;
            for (int i = 0; i <= endTime; i += 100)
            {
                chart.AddXAxisData(i.ToString());
                chart.AddData(serie2.index, value);
            }
            Debug.Log("Umbral 1 añadido al gráfico: " + value);
        }
    }
}




