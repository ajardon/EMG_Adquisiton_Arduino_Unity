# Creación de un dispositivo que permita la capturar señales EMG (Virtu-Limb) -- AMPUTACIONES TRANSRADIALES.
1. Estudio del mercado de sensores de EMGs.
     Los electrodos deberan ser posicionados de la froma descrita debajo para poder adquirir las señales EMG, depende del paciente sera las complejo o mas sencillo puede dificultar la adquisicón.
     ![image](https://github.com/anlmgp/TFM-Robotica/assets/46930450/f38c17aa-17d6-4ad2-8a8f-05ece52e272b)
     **- Sensor muscular MyoWare 2.0**
       Este sensor puede ser una buena opción debido a su capacidad para detectar señales de varios músculos al mismo tiempo. Esto es posible gracias a que el sensor cuenta con dos canales, lo que permite medir 
       la  actividad de dos músculos independientes al mismo tiempo. Además, el MyoWare 2.0 EMG sensor es compatible con Arduino y se puede programar para adaptarse a diferentes necesidades.
       https://solectroshop.com/es/blog/explorando-las-posibilidades-del-sensor-emg-myoware-20-de-sparkfun-n119
       https://solectroshop.com/es/sensores-medicos/36364-myoware-20-muscle-sensor-development-kit-kit-completo.html
       ![image](https://github.com/anlmgp/TFM-Robotica/assets/46930450/447acd4a-5d76-4bf3-9133-2228ff083a57)
      ** PERMITE EL PROCESADO DE LAS SEÑALES FISIOLOGICAS QUE HAN SIDO DEFINIDAS EN EL SUMMARY, CONEXIÓN CON ARDUINO FACIL IMPLEMENTACION ** 
     -  MyoWare Muscle Sensor de SparkFun
        Bastante similiar a el anterior, es la versión anterior es mejor la nueva ya que contienen un modulo que permite conectar varios sesores a la vez lo que nos permitiria analizar los dos musculos.
        https://www.electronicaembajadores.com/Datos/pdf1/ss/ssbi/ssbimu4.pdf
     - Walfront Sensor EMG Sensor Electromiográfico Sensor de Señal Muscular con Línea de Conexión EMG (Sensor EMG)
       ![image](https://github.com/anlmgp/TFM-Robotica/assets/46930450/132891cc-a3bb-4971-8439-781fa7506cea)
        No he encontrado mucha información a el respecto.

     **DE MOMENTO LA MEJOR OPCIÓN ES SENSOR MUSCULAR MYOWARE**
     
2. Establecer el numero de sensores en base a las necesidades de la detección.
   Para el control de una  prótesis mioeléctrica transradial, es necesario controlar los movimientos de aprehensión/prensión de la mano asi como el movimiento de giro de la muñeca. Por ello usando electrodos de     superficie ubicados en el musculo braquiorradial se controlan los movimientos de aprehensión y prensión, y con el músculo supinador se controla el movimiento de giro.
   5 Movimientos a necesarios que deben de ser realizados durante la etapa de entrenamiento:
   - **Posicion anatomica** : mano extendida con los dedos extendidos.
     ![image](https://github.com/anlmgp/TFM-Robotica/assets/46930450/86394000-b43b-4bfa-b5b6-bccfdfa77e74)
   - **Prenshion de la mano** (Agarre): El agarre describe la capacidad de los dedos y el pulgar para agarrar, a menudo para sostener, sujetar y recoger objetos grandes en los que se requiere una gran fuerza          manual.Esta actividad requiere grandes fuerzas para flexionar las falanges, y para flexionar y abducción del pulgar. La muñeca debe estar parcialmente extendida para estabilizar el movimiento. La
     forma de los objetos sujetados tiende a ser esférica o cilíndrica Se mide principalmente en el *musculo braquiradial*.
     
   - **Pinza de la mano**: 
4. Diseño y creación del brazalete con sensores EMG
   
   Se puede crear un dispositivo similar a el que se encuentra en la siguiente imagen en el cual se usen distintos sensores (Myoware EMG Sensor).

   Los sensores pueden ir protegidos por un diseño realizado con imprension 3D y la union realizarla con un materal mas maleable que permita ajustarse a el muñon de cada paciente.
   MATERIAL
  ![image](https://github.com/anlmgp/TFM-Robotica/assets/46930450/44bc0fb1-af6d-4684-a56d-2941bc7bf66b)

 ** EL NUMERO DE SENSORES DEL DISEÑO PUEDE VERSE LIMITADO POR EL DIAMETRO Y LAS DIMENSIONES DE ESTE MISMO ESTO PUEDE SUPONER UN PROBLEMA PARA EL NUMERO DE CANALES.**

