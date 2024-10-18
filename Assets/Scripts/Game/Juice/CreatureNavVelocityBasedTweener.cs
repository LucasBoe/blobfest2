using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Simple.SoundSystem.Core;
using NaughtyAttributes;

public class CreatureNavVelocityBasedTweener : MonoBehaviour
{
    [SerializeField] MonoBehaviour velocityProviderComponent;
    [SerializeField] Sound optionalStepSound;
    [SerializeField] bool haveRandomTimeOffset;
    private IVelocityProvider velocityProvider;

    public const float WALK_POS_OFFSET = -.2f;
    public const float WALK_SQUASH_STRENGTH = 0.75f;
    public const float IDLE_SQUASH_STRENGTH = 0.2f;
    public const float IDLE_ANIMATION_SPEED = 3f;
    public const float IDLE_PEAK_SHARPNESS = 0.4f;

    public float WalkAnimationSpeed = 1.5f;
    public float WalkRotationStrength = 20f;

    private float timeOffset = 0f;
    private Vector2 velocity = Vector2.zero; // Assumes you have a direction vector somewhere.
    private float xOrientation = 1.0f; // Assumes a starting orientation.
    [SerializeField, ReadOnly] private float lastPlayTime = -1f;

    private void Awake()
    {
        if (haveRandomTimeOffset)
            timeOffset = Random.Range(0, Mathf.PI);

        velocityProvider = velocityProviderComponent as IVelocityProvider;
    }

    private void Update()
    {
        velocity = velocityProvider.Velocity;

        if (velocity.x != 0)
            xOrientation = Mathf.Sign(velocity.x);
        
        TweenTargetData target = ApplyTweens();

        float lerpFactor = Time.deltaTime / .2f;
        float lerpFactorRotation = lerpFactor * 4f;
        float lerpFactorScale = Mathf.Sign(transform.localScale.x) != Mathf.Sign(target.Scale.x) ? 1f : lerpFactor;

        // Interpolating position, rotation, and scale
        transform.localPosition = Vector2.Lerp(transform.localPosition, target.Position, lerpFactor);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, target.Rotation), lerpFactorRotation);
        transform.localScale = Vector2.Lerp(transform.localScale, target.Scale, lerpFactorScale);
    }
    private TweenTargetData ApplyTweens()
    {
        float timeInSeconds = Time.time + timeOffset;

        if (velocity.magnitude > 1)
            return WalkTween(timeInSeconds);
        
        return IdleTween(timeInSeconds);
    }
    private TweenTargetData WalkTween(float timeInSeconds)
    {
        float t = timeInSeconds * WalkAnimationSpeed;
        float sin = Mathf.Sin(t);
        float sign = Mathf.Sign(sin);

        if (Mathf.Abs(sin) >= .9f)
        {
            if (lastPlayTime < Time.time - .2f)
            {
                lastPlayTime = Time.time;
                optionalStepSound?.Play();
            }
        }

        float raw = Mathf.Pow(Mathf.Abs(sin), 0.75f) * sign;

        float dirToRotation = new Vector2(1, velocity.normalized.y * 0.2f).normalized.magnitude * xOrientation;
        float rotationTarget = dirToRotation + raw * WalkRotationStrength;

        float rawS = Mathf.Pow(Mathf.Abs(Mathf.Sin(t)), 0.4f) * Mathf.Sign(Mathf.Sin(t));
        float scaleBase = Mathf.Abs(rawS) * WALK_SQUASH_STRENGTH;

        float x = (.5f + scaleBase) * xOrientation;
        float y = .8f + Mathf.Abs(scaleBase - WALK_SQUASH_STRENGTH);

        Vector2 pos = new Vector2(0, WALK_POS_OFFSET / 2 - Mathf.Abs(rawS) * WALK_POS_OFFSET);

        return new TweenTargetData(pos, rotationTarget, new Vector2(x, y));
    }

    private TweenTargetData IdleTween(float timeInSeconds)
    {
        float scaleOffset = Mathf.Sin(timeInSeconds * IDLE_ANIMATION_SPEED);
        float scaleBase = Mathf.Pow(Mathf.Abs(scaleOffset), IDLE_PEAK_SHARPNESS) * IDLE_SQUASH_STRENGTH;
        float x = (1.1f + Mathf.Abs(scaleBase - IDLE_SQUASH_STRENGTH)) * xOrientation;
        float y = 1f - IDLE_SQUASH_STRENGTH / 2f + scaleBase;

        return new TweenTargetData(Vector2.zero, 0, new Vector2(x, y));
    }

    // Equivalent of Godot's custom class to store position, rotation, and scale
    public class TweenTargetData
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Scale { get; set; }

        public TweenTargetData(Vector2 position, float rotation, Vector2 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }
    }
}