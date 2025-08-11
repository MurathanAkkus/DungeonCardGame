using UnityEngine;

public class HeroSystem : Singleton<HeroSystem>
{
    // HeroSystem, oyundaki kahramanla ilgili
    // kahraman se�imi, kahraman istatistikleri
    // ve kahraman yetenekleri gibi kahramanla
    // ilgili i�levleri y�netmek i�in geni�letilebilir.
    [field: SerializeField] public HeroView HeroView { get; private set; }

    public void Setup(HeroData heroData)
    {
        HeroView.Setup(heroData);
    }
}