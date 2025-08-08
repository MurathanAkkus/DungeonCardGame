using UnityEngine;

// Mouse ile ilgili yard�mc� fonksiyonlar� bar�nd�ran statik s�n�f
public static class MouseUtil
{
    // Ana kamera referans�. Mouse pozisyonunu d�nya koordinatlar�na �evirmek i�in kullan�l�r.
    private static Camera camera = Camera.main;

    /// <summary>
    /// Mouse'un ekrandaki pozisyonunu, d�nya (world) koordinatlar�na �evirir.
    /// </summary>
    /// <param name="zValue">
    /// D�n��t�r�lecek d�zlemin Z eksenindeki konumu.
    /// Varsay�lan olarak 0'd�r. Kart gibi nesneleri belirli bir derinlikte ta��mak i�in kullan�l�r.
    /// </param>
    /// <returns>
    /// Mouse'un d�nya koordinatlar�ndaki pozisyonu. E�er hesaplanamazsa Vector3.zero d�ner.
    /// </returns>
    public static Vector3 GetMousePositionInWorldSpace(float zValue = 0f)
    {
        // Mouse'un d�nya koordinat�na �evrilece�i d�zlem olu�turulur.
        // D�zlemin y�n� kameran�n bakt��� y�nle ayn�, konumu ise zValue parametresiyle belirlenir.
        Plane dragePlane = new(camera.transform.forward, new Vector3(0, 0, zValue));

        // Mouse'un ekrandaki pozisyonundan bir ���n (ray) olu�turulur.
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        // Olu�turulan ���n�n, belirlenen d�zlemle kesi�ip kesi�medi�i kontrol edilir.
        if (dragePlane.Raycast(ray, out float distance))
        {   // E�er kesi�iyorsa, ���n�n d�zlemle kesi�ti�i noktadaki d�nya koordinat� d�nd�r�l�r.
            return ray.GetPoint(distance);
        }
        // E�er kesi�me yoksa, (�rne�in kamera veya d�zlem hatal�ysa) Vector3.zero d�nd�r�l�r.
        return Vector3.zero;
    }
}