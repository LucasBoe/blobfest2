using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class CreatureBase : MonoBehaviour, IVelocityProvider, IPositionProvider
{
    [SerializeField] protected CreatureMovement Movement;
    [SerializeField] Transform zOffsetTransform;

    public Vector2 Velocity => Movement.Velocity;

    [ShowNativeProperty] public Vector2 Position => transform.position;

    private void Start()
    {
        Movement.Init(this);
    }
    [Button]
    private void MoveRandom()
    {
        Movement.SetInput(new Vector2(Random.Range(-14, 14), Random.Range(-7, 7)));
    }
    private void FixedUpdate()
    {
        Movement.OnFixedUpdate();
    }
    internal void SetPosition(Vector2 position)
    {
        transform.position = position;
        zOffsetTransform.localPosition = new Vector3(0, 0,  ZOffsetUtil.OffsetFromY(position.y));
    }
}
public class ZOffsetUtil
{
    internal static float OffsetFromY(float y) => .1f * y;
}
public interface IPositionProvider
{
    public Vector2 Position { get; }
}
internal interface IVelocityProvider
{
    public Vector2 Velocity { get; }
}

[System.Serializable]
public class CreatureMovement : IVelocityProvider
{
    [SerializeField] float moveSpeed;
    [SerializeField] float movementSmoothingTime = .25f;

    CreatureBase creature;

    private Vector2 direction;
    Vector2 targetVelocity;

    public Vector2 Velocity { get; private set; }
    public static readonly Vector2 MOVE_SPEED_BY_AXIS = new Vector2(1, .66f);

    internal void Init(CreatureBase creature)
    {
        this.creature = creature;
    }

    internal void OnFixedUpdate()
    {
        if (direction.magnitude <= 0)
        {
            targetVelocity = Vector2.zero;
        }
        else
        {
            targetVelocity = direction.normalized * MOVE_SPEED_BY_AXIS * moveSpeed;
            direction = Vector2.zero;
        }


        Velocity = Vector2.Lerp(Velocity, targetVelocity, Time.fixedDeltaTime / movementSmoothingTime);
        creature.SetPosition(creature.Position + Velocity * Time.fixedDeltaTime);
    }

    internal void SetInput(Vector2 direction)
    {
     this.   direction = direction;
    }
}