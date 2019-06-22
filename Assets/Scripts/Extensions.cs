using UnityEngine;


/// <summary>
/// Класс с расширениями.
/// </summary>


public static class Extensions
{
    /// <summary>
    /// Заполнить текстуру заданным цветом.
    /// </summary>
    /// <param name="texture"> Ссылка на текстуру. </param>
    /// <param name="color"> Цвет заполнениия. </param>
    public static void Fill(this Texture2D texture, Color color)
    {
        Color[] colors = new Color[texture.width * texture.height];
        for (int n = 0; n < colors.Length; n++)
        {
            colors[n] = color;
        }

        texture.SetPixels(colors);
    }


    /// <summary>
    /// Описание делегата для расчета цвета пикселя, в зависимости от координат центра пикселя.
    /// </summary>
    /// <param name="pixelPos"> Координаты центра пикселя относительно центра закрашиваемого круга. </param>
    /// <returns></returns>
    public delegate Color CalcColorDelegate(Vector2 pixelPos);

    /// <summary>
    /// Нарисовать круг на текстуре.
    /// </summary>
    /// <param name="texture"> Ссылка на текстуру. </param>
    /// <param name="centerX"> Координата X центра круга. </param>
    /// <param name="centerY"> Координата Y центра круга. </param>
    /// <param name="radius"> Радиус круга. </param>
    /// <param name="calcColor"> Функция для расчета цвета для каждого пикселя. </param>
    public static void Circle(this Texture2D texture, int centerX, int centerY, int radius, CalcColorDelegate calcColor)
    {
        Color[] colors = texture.GetPixels(centerX - radius, centerY - radius, 2 * radius, 2 * radius);

        for (int y = 0; y < radius; y++)
        {
            float pixelPosY = y + 0.5f;

            int py = centerY + y;
            int ny = centerY - y - 1;

            for (int x = 0; x < radius; x++)
            {
                float pixelPosX = x + 0.5f;

                Vector2 pixelPos = new Vector2(pixelPosX, pixelPosY);
                if (pixelPos.sqrMagnitude < radius * radius)
                {
                    int px = centerX + x;
                    int nx = centerX - x - 1;

                    colors[2 * radius * py + px] = calcColor(pixelPos);
                    colors[2 * radius * py + nx] = calcColor(new Vector2(-pixelPosX, pixelPosY));
                    colors[2 * radius * ny + px] = calcColor(new Vector2(pixelPosX, -pixelPos.y));
                    colors[2 * radius * ny + nx] = calcColor(new Vector2(-pixelPos.x, -pixelPos.y));
                }
            }
        }
        texture.SetPixels(centerX - radius, centerY - radius, 2 * radius, 2 * radius, colors);
    }
}
