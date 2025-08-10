using TMPro;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;
public class CardView : MonoBehaviour
{
    [Header("CardView Data Settings")]
    [SerializeField] private TMP_Text title;            // Kart ba�l���n� g�stermek i�in TMP_Text referans�
    [SerializeField] private TMP_Text description;      // Kart a��klamas�n� g�stermek i�in TMP_Text referans�
    [SerializeField] private TMP_Text mana;             // Kart�n mana de�erini g�stermek i�in TMP_Text referans�
    [Header("CardView Visual Settings")]
    [SerializeField] private SpriteRenderer imageSR;    // Kart g�rselini g�stermek i�in SpriteRenderer referans�
    [SerializeField] private GameObject wrapper;        // Kart�n etraf�ndaki g�rsel sarmalay�c� (�rne�in �er�eve)
    [SerializeField] private LayerMask dropLayer;       // Kart�n b�rak�labilece�i katman

    public Card Card { get; private set; }              // Bu CardView'da g�sterilen kart nesnesi

    private Vector3 dragStartPosition;                  // S�r�kleme ba�lad���nda kart�n pozisyonunu saklar
    private Quaternion dragStartRotation;               // S�r�kleme ba�lad���nda kart�n rotasyonunu saklar

    public void Setup(Card card)
    {   // Kart� ve g�rsel elemanlar�n� verilen Card nesnesiyle doldurur
        Card = card;
        title.text = card.Title;
        description.text = card.Description;
        mana.text = card.Mana.ToString();
        imageSR.sprite = card.Image;
        //wrapper.SetActive(true);
    }
    void OnMouseEnter()
    {
        if (!Interactions.Instance.PlayerCanHover())
        {   // Oyuncu kart�n �zerine gelebiliyor mu kontrol edilir
            return;
        }
        wrapper.SetActive(false);                               // Kart�n �er�evesi gizlenir
        Vector3 position = new (transform.position.x, -2, 0);   // Kart hover sisteminde kart b�y�k g�sterilir
        // B�ylece oyuncu fareyle kart�n �zerine geldi�inde, kart�n detaylar�n� daha b�y�k ve okunabilir �ekilde g�rebilir.
        CardViewHoverSystem.Instance.Show(Card, position);      
    }
    void OnMouseExit()
    {
        if (!Interactions.Instance.PlayerCanHover())
        {
            // Oyuncunun kart�n �zerine gelip detaylar�n� g�rebilmesi i�in gerekli �artlar�n sa�lan�p sa�lanmad���n� kontrol eder.
            return; // E�er hover(�zerine gelme) m�mk�n de�ilse, fonksiyonun devam� �al��maz ve kart b�y�t�lerek g�sterilmez.
        }
        // Hover g�r�n�m� kapat�l�r ve �er�eve tekrar g�sterilir
        CardViewHoverSystem.Instance.Hide();
        wrapper.SetActive(true);
    }

    // Kart�n �zerine t�kland���nda �al���r (s�r�kleme ba�lat�l�r)
    void OnMouseDown()
    {   
        if (!Interactions.Instance.PlayerCanInteract())
        {   // Oyuncunun kart� t�klayarak s�r�kleyip b�rakma gibi i�lemleri yap�p yapamayaca��n� kontrol eder. 
            return; // E�er etkile�im m�mk�n de�ilse, fonksiyonun devam� �al��maz ve i�lem iptal edilir.
        }
        // S�r�kleme ba�lat�l�r
        Interactions.Instance.PlayerIsDragging = true;                      
        wrapper.SetActive(true);                                            
        CardViewHoverSystem.Instance.Hide();
        // S�r�kleme ba�lang�� pozisyonu ve rotasyonu kaydedilir
        dragStartPosition = transform.position;
        dragStartRotation = transform.rotation;
        // Kart d�z hale getirilir ve mouse pozisyonuna ta��n�r
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = MouseUtil.GetMousePositionInWorldSpace(-1);
    }

    // Kart s�r�klenirken s�rekli �al���r
    void OnMouseDrag()
    {
        if (!Interactions.Instance.PlayerCanInteract())
        {   // Bu kontroller, oyun ak���nda yanl�� zamanda veya istenmeyen durumlarda kartlarla etkile�imi ve hover efektini engellemek i�in kullan�l�r.
            return; // B�ylece sadece uygun zamanlarda oyuncunun kartlarla etkile�ime ge�mesi sa�lan�r.
        }
        // Kart mouse'un oldu�u pozisyona ta��n�r
        transform.position = MouseUtil.GetMousePositionInWorldSpace(-1);
    }

    // S�r�kleme b�rak�ld���nda �al���r
    void OnMouseUp()
    {
        if (!Interactions.Instance.PlayerCanInteract())
        {
            return;
        }

        // Kart�n b�rak�ld��� yerde bir hedef var m� kontrol edilir
        if (ManaSystem.Instance.HasEnoughMana(Card.Mana) 
            && Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hit, 10f, dropLayer))
        {
            PlayCardGA playCardGA = new PlayCardGA(Card); // Kart oynama aksiyonu olu�turulur
            ActionSystem.Instance.Perform(playCardGA);    // Aksiyon sisteme eklenir 
        }
        else
        {
            // Ge�erli bir hedef yoksa kart eski pozisyonuna ve rotasyonuna d�ner
            transform.position = dragStartPosition;
            transform.rotation = dragStartRotation;
        }
        // S�r�kleme i�lemi sona erer
        Interactions.Instance.PlayerIsDragging = false;
    }
}
