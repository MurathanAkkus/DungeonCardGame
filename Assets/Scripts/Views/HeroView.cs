
        if (heroData.StartingStrength != 0)
        {
            AddStatusEffect(StatusEffectType.STRENGTH, heroData.StartingStrength);
        }
}
// Temel savaşçı işlevselliğini yeniden kullanmak için CombatantView'dan devralır.
public class HeroView : CombatantView
{
    // Kahraman görünümünü varsayılan canla ve sprite olmadan başlatır.
    // Bu yöntem yeni eklendi ve kahraman için temel değerleri ayarlar.
    public void Setup(HeroData heroData)
    {
        SetupBase(heroData.Health, heroData.Image, heroData.StartingArmor);
    }
}