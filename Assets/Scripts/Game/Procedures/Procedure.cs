using System;
using Engine;
using UnityEngine;

public abstract class ProcedureBase
{
    public bool IsRunning { get; protected set; } = true;
    public Cell AssociatedCell { get; private set; }
    public Card Input { get; private set; }

    private Token rewardToken;
    private Card rewardCard;
    private Action rewardAction;

    private bool needsNPC = false;
    private NPCBehaviour associatedNPC;

    public float Progression { get; private set; } 

    public Event<float> OnProcedureProgressionChangedEvent = new();

    public abstract void Update(float time);
    protected void SetNewProgression(float progression)
    {
        if (progression < 1)
            IsRunning = true;

        Progression = progression;

        OnProcedureProgressionChangedEvent?.Invoke(Progression);

        if (Progression >= 1f)
        {
            IsRunning = false;
            FinishProcedure();
        }
    }
    internal void OnStartProcedure()
    {
        if (!needsNPC)
            return;

        if (NPCHandler.Instance.TryRequestFreedNPC(out var npc))
        {
            associatedNPC = npc;
            associatedNPC.Link(AssociatedCell);
        }
    }
    internal void OnStopProcedure(bool regular = true)
    {
        if (!needsNPC || associatedNPC == null)
            return;

        GameObject.Destroy(associatedNPC.gameObject);
    }
    protected void FinishProcedure()
    {
        if (rewardToken != null)
            CollectibleSpawner.Instance.SpawnAt(rewardToken, AssociatedCell.Center);

        if (rewardCard != null)
            CollectibleSpawner.Instance.SpawnAt(rewardCard, AssociatedCell.Center);

        if (Input != null)
            CollectibleSpawner.Instance.SpawnAt(Input, AssociatedCell.Center);

        rewardAction?.Invoke();
    }
    internal ProcedureBase At(Cell cell)
    {
        AssociatedCell = cell;
        return this;
    }
    internal ProcedureBase WithInput(Card input)
    {
        Input = input;
        return this;
    }
    internal ProcedureBase WithReward(CardID id)
    {
        rewardCard = id.ToCard();
        return this;
    }
    internal ProcedureBase WithReward(TokenID id)
    {
        rewardToken = id.ToToken();
        return this;
    }
    internal ProcedureBase WithCallback(Action callback)
    {
        rewardAction = callback;
        return this;
    }

    internal ProcedureBase WithNPC()
    {
        needsNPC = true;
        return this;
    }
}

public class StaticTimeProcedure : ProcedureBase
{
    public float StartTime, FinishTime = -1f;
    private float duration;

    public StaticTimeProcedure(float duration)
    {
        this.duration = duration;
        StartTime = Time.time;
        FinishTime = StartTime + duration;
    }

    public override void Update(float time)
    {
        float progression = (time - StartTime) / duration;
        SetNewProgression(progression);
    }
}


public class DynamicTimeProcecure : ProcedureBase
{
    public interface IProgressProvider
    {
        public float ProgressMultiplier { get; }
    }

    private float lastTime;
    private IProgressProvider progressProvider;

    public DynamicTimeProcecure(IProgressProvider progressProvider)
    {
        lastTime = Time.time;
        this.progressProvider = progressProvider;
    }

    public override void Update(float time)
    {
        float deltaTime = time - lastTime;
        SetNewProgression(Progression + progressProvider.ProgressMultiplier * deltaTime);
        lastTime = time;
    }
}
