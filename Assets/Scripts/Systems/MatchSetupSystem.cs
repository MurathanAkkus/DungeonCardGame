using System.Collections.Generic;
using UnityEngine;

// Oyun baþlatýldýðýnda destedeki kartlarý hazýrlayan ve oyuncunun eline baþlangýç kartlarýný daðýtan sistemi temsil eder.
// CardSystem ve ActionSystem ile etkileþime girerek, oyun baþlangýcýnda gerekli kurulum iþlemlerini otomatik olarak gerçekleþtirir.
public class MatchSetupSystem : MonoBehaviour
{
    // Oyuncunun ve rakibin destesinde bulunacak kart verileri
    [SerializeField] private HeroData heroData;
    [SerializeField] private List<EnemyData> enemyDatas;

    private void Start()
    {   
        HeroSystem.Instance.Setup(heroData);
        EnemySystem.Instance.Setup(enemyDatas);
        // CardSystem'i baþlatýr ve destedeki kart verilerini sisteme aktarýr
        // Böylece oyun baþýnda hangi kartlarýn kullanýlacaðý belirlenir
        CardSystem.Instance.Setup(heroData.Deck);

        // 5 kart çekmek için bir DrawCardsGA (Game Action) nesnesi oluþturulur
        // Bu, oyuncunun eline baþlangýçta 5 kart gelmesini saðlar
        DrawCardsGA drawCardsGA = new(5);

        // ActionSystem üzerinden kart çekme iþlemi gerçekleþtirilir
        // Böylece oyun baþladýðýnda oyuncunun eline kartlar daðýtýlýr
        ActionSystem.Instance.Perform(drawCardsGA);
    }
}
