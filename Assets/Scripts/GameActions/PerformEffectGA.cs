using System.Collections.Generic;

// Bu s�n�f, oyun s�ras�nda bir etkiyi bir veya
// birden fazla hedefe uygulamak gerekti�inde kullan�l�r.
// �rne�in, bir kart oynand���nda veya bir yetenek kullan�ld���nda,
// ilgili etki ve hedefler belirlenir
// ve bu s�n�f ile bir aksiyon olarak i�lenir.
public class PerformEffectGA : GameAction
{
    public Effect Effect { get; set; }
    public List<CombatantView> Targets { get; set; }
    public PerformEffectGA(Effect effect, List<CombatantView> targets)
    {
        Effect = effect;
        // targets parametresi null ise Targets'� da null yapar;
        // de�ilse, gelen listenin kopyas�n� olu�turarak d��ar�dan
        // yap�lan de�i�ikliklerin bu s�n�f� etkilemesini �nler.
        Targets = targets == null ? null : new List<CombatantView>(targets);
    }
}