using System;
using Engine;
using UnityEngine;

internal class Procedure
{
    public bool IsRunning { get; private set; } = true;
    public Cell AssociatedCell { get; private set; }
    public Card Input { get; private set; }

    public float StartTime, FinishTime = -1f;

    private float duration;

    private Token rewardToken;
    private Card rewardCard;
    private Action rewardAction;

    public Event<float> OnProcedureProgressionChangedEvent = new();

    public Procedure(float duration)
    {
        this.duration = duration;
        StartTime = Time.time;
        FinishTime = StartTime + duration;
    }

    internal void Update(float time)
    {
        float progression = (time - StartTime) / duration;
        OnProcedureProgressionChangedEvent?.Invoke(progression);

        if (time > FinishTime)
        {
            IsRunning = false;
            FinishProcedure();
        }
    }
    private void FinishProcedure()
    {
        if (rewardToken != null)
            CollectibleSpawner.Instance.SpawnAt(rewardToken, AssociatedCell.Center);

        if (rewardCard != null)
            CollectibleSpawner.Instance.SpawnAt(rewardCard, AssociatedCell.Center);

        if (Input != null)
            CollectibleSpawner.Instance.SpawnAt(Input, AssociatedCell.Center);

        rewardAction?.Invoke();
    }

    internal Procedure At(Cell cell)
    {
        AssociatedCell = cell;
        return this;
    }
    internal Procedure WithInput(Card input)
    {
        Input = input;
        return this;
    }
    internal Procedure WithReward(CardID id)
    {
        rewardCard = id.ToCard();
        return this;
    }
    internal Procedure WithReward(TokenID id)
    {
        rewardToken = id.ToToken();
        return this;
    }
    internal Procedure WithCallback(Action callback)
    {
        rewardAction = callback;
        return this;
    }
}
