using System.Collections.Generic;
using UnityEngine;

// Oyun ba�lat�ld���nda destedeki kartlar� haz�rlayan ve oyuncunun eline ba�lang�� kartlar�n� da��tan sistemi temsil eder.
// CardSystem ve ActionSystem ile etkile�ime girerek, oyun ba�lang�c�nda gerekli kurulum i�lemlerini otomatik olarak ger�ekle�tirir.
public class MatchSetupSystem : MonoBehaviour
{
    // Oyuncunun ve rakibin destesinde bulunacak kart verileri
    [SerializeField] private HeroData heroData;
    [SerializeField] private List<EnemyData> enemyDatas;

    private void Start()
    {   
        HeroSystem.Instance.Setup(heroData);
        EnemySystem.Instance.Setup(enemyDatas);
        // CardSystem'i ba�lat�r ve destedeki kart verilerini sisteme aktar�r
        // B�ylece oyun ba��nda hangi kartlar�n kullan�laca�� belirlenir
        CardSystem.Instance.Setup(heroData.Deck);

        // 5 kart �ekmek i�in bir DrawCardsGA (Game Action) nesnesi olu�turulur
        // Bu, oyuncunun eline ba�lang��ta 5 kart gelmesini sa�lar
        DrawCardsGA drawCardsGA = new(5);

        // ActionSystem �zerinden kart �ekme i�lemi ger�ekle�tirilir
        // B�ylece oyun ba�lad���nda oyuncunun eline kartlar da��t�l�r
        ActionSystem.Instance.Perform(drawCardsGA);
    }
}
