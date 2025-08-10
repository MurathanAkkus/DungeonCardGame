using TMPro;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;
public class CardView : MonoBehaviour
{
    [Header("CardView Data Settings")]
    [SerializeField] private TMP_Text title;            // Kart baþlýðýný göstermek için TMP_Text referansý
    [SerializeField] private TMP_Text description;      // Kart açýklamasýný göstermek için TMP_Text referansý
    [SerializeField] private TMP_Text mana;             // Kartýn mana deðerini göstermek için TMP_Text referansý
    [Header("CardView Visual Settings")]
    [SerializeField] private SpriteRenderer imageSR;    // Kart görselini göstermek için SpriteRenderer referansý
    [SerializeField] private GameObject wrapper;        // Kartýn etrafýndaki görsel sarmalayýcý (örneðin çerçeve)
    [SerializeField] private LayerMask dropLayer;       // Kartýn býrakýlabileceði katman

    public Card Card { get; private set; }              // Bu CardView'da gösterilen kart nesnesi

    private Vector3 dragStartPosition;                  // Sürükleme baþladýðýnda kartýn pozisyonunu saklar
    private Quaternion dragStartRotation;               // Sürükleme baþladýðýnda kartýn rotasyonunu saklar

    public void Setup(Card card)
    {   // Kartý ve görsel elemanlarýný verilen Card nesnesiyle doldurur
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
        {   // Oyuncu kartýn üzerine gelebiliyor mu kontrol edilir
            return;
        }
        wrapper.SetActive(false);                               // Kartýn çerçevesi gizlenir
        Vector3 position = new (transform.position.x, -2, 0);   // Kart hover sisteminde kart büyük gösterilir
        // Böylece oyuncu fareyle kartýn üzerine geldiðinde, kartýn detaylarýný daha büyük ve okunabilir þekilde görebilir.
        CardViewHoverSystem.Instance.Show(Card, position);      
    }
    void OnMouseExit()
    {
        if (!Interactions.Instance.PlayerCanHover())
        {
            // Oyuncunun kartýn üzerine gelip detaylarýný görebilmesi için gerekli þartlarýn saðlanýp saðlanmadýðýný kontrol eder.
            return; // Eðer hover(üzerine gelme) mümkün deðilse, fonksiyonun devamý çalýþmaz ve kart büyütülerek gösterilmez.
        }
        // Hover görünümü kapatýlýr ve çerçeve tekrar gösterilir
        CardViewHoverSystem.Instance.Hide();
        wrapper.SetActive(true);
    }

    // Kartýn üzerine týklandýðýnda çalýþýr (sürükleme baþlatýlýr)
    void OnMouseDown()
    {   
        if (!Interactions.Instance.PlayerCanInteract())
        {   // Oyuncunun kartý týklayarak sürükleyip býrakma gibi iþlemleri yapýp yapamayacaðýný kontrol eder. 
            return; // Eðer etkileþim mümkün deðilse, fonksiyonun devamý çalýþmaz ve iþlem iptal edilir.
        }
        // Sürükleme baþlatýlýr
        Interactions.Instance.PlayerIsDragging = true;                      
        wrapper.SetActive(true);                                            
        CardViewHoverSystem.Instance.Hide();
        // Sürükleme baþlangýç pozisyonu ve rotasyonu kaydedilir
        dragStartPosition = transform.position;
        dragStartRotation = transform.rotation;
        // Kart düz hale getirilir ve mouse pozisyonuna taþýnýr
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = MouseUtil.GetMousePositionInWorldSpace(-1);
    }

    // Kart sürüklenirken sürekli çalýþýr
    void OnMouseDrag()
    {
        if (!Interactions.Instance.PlayerCanInteract())
        {   // Bu kontroller, oyun akýþýnda yanlýþ zamanda veya istenmeyen durumlarda kartlarla etkileþimi ve hover efektini engellemek için kullanýlýr.
            return; // Böylece sadece uygun zamanlarda oyuncunun kartlarla etkileþime geçmesi saðlanýr.
        }
        // Kart mouse'un olduðu pozisyona taþýnýr
        transform.position = MouseUtil.GetMousePositionInWorldSpace(-1);
    }

    // Sürükleme býrakýldýðýnda çalýþýr
    void OnMouseUp()
    {
        if (!Interactions.Instance.PlayerCanInteract())
        {
            return;
        }

        // Kartýn býrakýldýðý yerde bir hedef var mý kontrol edilir
        if (ManaSystem.Instance.HasEnoughMana(Card.Mana) 
            && Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hit, 10f, dropLayer))
        {
            PlayCardGA playCardGA = new PlayCardGA(Card); // Kart oynama aksiyonu oluþturulur
            ActionSystem.Instance.Perform(playCardGA);    // Aksiyon sisteme eklenir 
        }
        else
        {
            // Geçerli bir hedef yoksa kart eski pozisyonuna ve rotasyonuna döner
            transform.position = dragStartPosition;
            transform.rotation = dragStartRotation;
        }
        // Sürükleme iþlemi sona erer
        Interactions.Instance.PlayerIsDragging = false;
    }
}
