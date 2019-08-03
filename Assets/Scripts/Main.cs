using System.IO;
using UnityEngine;


/// <summary>
/// Программа для создания и сохранения в файл текстуры для ColorPicker-а в формате PNG.
/// Текстура представляет из себя бублик, радиусом Radius, толщиной Thick и закрашенный смесью цветов RGB.
/// </summary>
public class Main : MonoBehaviour
{
    /// <summary>
    /// Радиус цветного круга в пикселях.
    /// </summary>
    [Tooltip ("Радиус цветного круга в пикселях.")]
    public int  Radius = 512;

    /// <summary>
    /// Толщина цветного круга в пикселях. 
    /// </summary>
    [Tooltip("Толщина цветного круга в пикселях.")]
    public int Thick = 100;

    /// <summary>
    /// Имя файла в который будет сохранена текстура.
    /// </summary>
    [Tooltip ("Имя файла в который будет сохранена текстура.")]
    public string FileName = "ColorPicker.png";

   /// <summary>
    /// Массив с координатами точек R, G и B.
    /// </summary>
    private Vector2[] RGBpoints = new Vector2[3];

    /// <summary>
    /// Ссылка на массив под угловые расстояния заданной точки до точек R, G и B соответственно.
    /// </summary>
    private float[] Angles;


    /// <summary>
    /// Рассчет цвета пикселя в зависимости от координат его центра. 
    /// </summary>
    /// <param name="pixelPos"> Координаты центра пикселя относительно центра закрашиваемого круга. </param>
    /// <returns> Рассчитанный цвет пикселя. </returns>
    private Color CalcColor(Vector2 pixelPos)
    {
        Color color = Color.black;

        // Рассчитать угловые расстояния от центра пикселя до точек R, G и B соответственно
        for (int n = 0; n < Angles.Length; n++)
        {
            Angles[n] = Vector2.Angle(pixelPos, RGBpoints[n]);
        }

        // Найти минимальное, максмальное и среднее угловые расстояния
        float angleMin = Mathf.Min(Angles);
        float angleMax = Mathf.Max(Angles);
        float angleMed = 0;
        for (int n = 0; n < Angles.Length; n++)
        {
            if ((Angles[n] != angleMin) && (Angles[n] != angleMax))
            {
                angleMed = Angles[n];
            }
        }

        // В зависимости от углвых расстояни рассчитать цвет пикселя
        for (int n = 0; n < Angles.Length; n++)
        {
            if (Angles[n] == angleMin)
            {
                color[n] = 1.0f;
            }
            else if (Angles[n] == angleMax)
            {
                color[n] = 0.0f;
            }
            else
            {
                color[n] = 2 * angleMin / (angleMin + angleMed);
            }
        }

        return color;
    }


    // Start is called before the first frame update
    void Start()
    {
        // Кватернион поворота на 120 градусов по часовой стрелке вокруг оси Z
        Quaternion rotateQuaternion = Quaternion.Euler(0, 0, -120);

        // Инициализировать массив с координатами точек R, G и B 
        RGBpoints[0] = new Vector2(0, Radius);
        for (int p = 1; p < RGBpoints.Length; p++)
        {
            RGBpoints[p] = rotateQuaternion * RGBpoints[p - 1];
        }

        // Создать массив под угловые расстояния заданной точки до точек R, G и B
        Angles = new float[RGBpoints.Length];

        // Создать текстуру
        Texture2D texture = new Texture2D(2 * Radius, 2 * Radius);

        // Заполнить текстуру прозрачным цветом
        Color transparentColor = new Color(0, 0, 0, 0);
        texture.Fill(transparentColor);

        // Нарисовать цветной круг
        texture.Circle(Radius, Radius, Radius, CalcColor);

        // Нарисовать прозрачный круг внутри цветного круга
        texture.Circle(Radius, Radius, Radius - Thick, (pixelPos) =>
        {
            return transparentColor;
        });

        // Применить отрисванные пиксели к текстуре
        texture.Apply();

        // Преобразовать текстуру в фал PNG и уничтожить текстуру, она нам больше не понадобится
        byte[] bytes = texture.EncodeToPNG();
        Destroy(texture);

        // Сохранить ткстуру в файл в папку с текщим проектом
        File.WriteAllBytes(Application.dataPath + "/../" + FileName, bytes);

        // Вывести в отладку сообщение о том что текстура успешно создана и сохранена в файл 
        Debug.Log("Текстура успешно создана и сохранена в файл!");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
