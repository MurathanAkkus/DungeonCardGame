using UnityEngine;

public class HeroSystem : Singleton<HeroSystem>
{
    // HeroSystem, oyundaki kahramanla ilgili
    // kahraman seçimi, kahraman istatistikleri
    // ve kahraman yetenekleri gibi kahramanla
    // ilgili iþlevleri yönetmek için geniþletilebilir.
    [field: SerializeField] public HeroView HeroView { get; private set; }

    public void Setup(HeroData heroData)
    {
        HeroView.Setup(heroData);
    }
}