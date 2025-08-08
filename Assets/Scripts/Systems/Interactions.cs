public class Interactions : Singleton<Interactions>
{
    public bool PlayerIsDragging { get; set; } = false;

    public bool PlayerCanInteract ()
    {
        if (ActionSystem.Instance.State != ActionState.Performing)
        {
            return true;
        }
        else
        {
             return false;
        }
    }

    public bool PlayerCanHover()
    {
        if (PlayerIsDragging)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
