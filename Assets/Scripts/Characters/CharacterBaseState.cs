/// <summary>
/// Base State Pattern for Characters, which all states are derived from
/// </summary>
public abstract class CharacterBaseState
{
    public abstract void Start(CharacterStateManager StateManager);

    public abstract void Update(CharacterStateManager StateManager);
}
