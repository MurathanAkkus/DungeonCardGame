using UnityEngine;

// Mouse ile ilgili yardýmcý fonksiyonlarý barýndýran statik sýnýf
public static class MouseUtil
{
    // Ana kamera referansý. Mouse pozisyonunu dünya koordinatlarýna çevirmek için kullanýlýr.
    private static Camera camera = Camera.main;

    /// <summary>
    /// Mouse'un ekrandaki pozisyonunu, dünya (world) koordinatlarýna çevirir.
    /// </summary>
    /// <param name="zValue">
    /// Dönüþtürülecek düzlemin Z eksenindeki konumu.
    /// Varsayýlan olarak 0'dýr. Kart gibi nesneleri belirli bir derinlikte taþýmak için kullanýlýr.
    /// </param>
    /// <returns>
    /// Mouse'un dünya koordinatlarýndaki pozisyonu. Eðer hesaplanamazsa Vector3.zero döner.
    /// </returns>
    public static Vector3 GetMousePositionInWorldSpace(float zValue = 0f)
    {
        // Mouse'un dünya koordinatýna çevrileceði düzlem oluþturulur.
        // Düzlemin yönü kameranýn baktýðý yönle ayný, konumu ise zValue parametresiyle belirlenir.
        Plane dragePlane = new(camera.transform.forward, new Vector3(0, 0, zValue));

        // Mouse'un ekrandaki pozisyonundan bir ýþýn (ray) oluþturulur.
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        // Oluþturulan ýþýnýn, belirlenen düzlemle kesiþip kesiþmediði kontrol edilir.
        if (dragePlane.Raycast(ray, out float distance))
        {   // Eðer kesiþiyorsa, ýþýnýn düzlemle kesiþtiði noktadaki dünya koordinatý döndürülür.
            return ray.GetPoint(distance);
        }
        // Eðer kesiþme yoksa, (örneðin kamera veya düzlem hatalýysa) Vector3.zero döndürülür.
        return Vector3.zero;
    }
}