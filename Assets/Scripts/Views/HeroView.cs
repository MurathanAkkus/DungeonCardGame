// Hero(biz) i�in g�rsel bile�eni temsil eder.
// Temel sava��� i�levselli�ini yeniden kullanmak i�in CombatantView'dan devral�r.
public class HeroView : CombatantView
{
    // Kahraman g�r�n�m�n� varsay�lan canla ve sprite olmadan ba�lat�r.
    // Bu y�ntem yeni eklendi ve kahraman i�in temel de�erleri ayarlar.
    public void Setup(HeroData heroData)
    {
        SetupBase(heroData.Health, heroData.Image);
    }
}
