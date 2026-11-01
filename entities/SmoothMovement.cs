using Godot;

namespace CoopDefense.Entities
{
    public static class SmoothMovement
    {
        // 核心物理方法：朝目标点移动
        public static Vector2 GetVelocity(Vector2 currentPos, Vector2 targetPos, float maxSpeed, float acceleration, float currentSpeed)
        {
            Vector2 desiredVelocity = (targetPos - currentPos).Normalized() * maxSpeed;
            
            // 使用 Godot 的向量插值（相当于物理引擎中的加速/减速）
            // 将当前速度平滑推向目标速度
            Vector2 newVelocity = currentPos.MoveToward(targetPos, currentSpeed * (float)0.016); 
            
            // 这里为了好理解，用最基础的 Arrive 转向力：
            Vector2 direction = (targetPos - currentPos).Normalized();
            float distance = currentPos.DistanceTo(targetPos);
            
            // 当距离很近时，减慢速度，防止在目标点附近鬼畜回弹
            float targetSpeed = distance < 10f ? maxSpeed * (distance / 10f) : maxSpeed;
            
            return direction * targetSpeed;
        }
    }
}