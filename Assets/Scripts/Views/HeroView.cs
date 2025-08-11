// Hero(biz) için görsel bileþeni temsil eder.
// Temel savaþçý iþlevselliðini yeniden kullanmak için CombatantView'dan devralýr.
public class HeroView : CombatantView
{
    // Kahraman görünümünü varsayýlan canla ve sprite olmadan baþlatýr.
    // Bu yöntem yeni eklendi ve kahraman için temel deðerleri ayarlar.
    public void Setup(HeroData heroData)
    {
        SetupBase(heroData.Health, heroData.Image);
    }
}
