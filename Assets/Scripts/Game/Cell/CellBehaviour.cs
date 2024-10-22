public abstract class CellBehaviour
{
    protected BehaviourCellContext Context;

    public void Init(BehaviourCellContext behaviourCellContext)
    {
        Context = behaviourCellContext;
    }
    public abstract void Enter();
    public abstract void Exit();
    public virtual void Update() {}
}
public class BehaviourCellContext
{
    public Cell Cell { get; set; }
}