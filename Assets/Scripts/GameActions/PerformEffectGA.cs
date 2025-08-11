using System.Collections.Generic;

// Bu sýnýf, oyun sýrasýnda bir etkiyi bir veya
// birden fazla hedefe uygulamak gerektiðinde kullanýlýr.
// Örneðin, bir kart oynandýðýnda veya bir yetenek kullanýldýðýnda,
// ilgili etki ve hedefler belirlenir
// ve bu sýnýf ile bir aksiyon olarak iþlenir.
public class PerformEffectGA : GameAction
{
    public Effect Effect { get; set; }
    public List<CombatantView> Targets { get; set; }
    public PerformEffectGA(Effect effect, List<CombatantView> targets)
    {
        Effect = effect;
        // targets parametresi null ise Targets'ý da null yapar;
        // deðilse, gelen listenin kopyasýný oluþturarak dýþarýdan
        // yapýlan deðiþikliklerin bu sýnýfý etkilemesini önler.
        Targets = targets == null ? null : new List<CombatantView>(targets);
    }
}