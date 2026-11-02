using Godot;

namespace CoopDefense.Entities
{
    public static class SmoothMovement
    {
        public static Vector2 GetVelocity(
            Vector2 currentPos,
            Vector2 targetPos,
            float targetSpeed,
            float acceleration,
            float currentSpeed,
            float delta)
        {
            float speed = Mathf.MoveToward(currentSpeed, targetSpeed, acceleration * delta);
            return (targetPos - currentPos).Normalized() * speed;
        }
    }
}