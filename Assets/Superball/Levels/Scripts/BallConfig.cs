using FriendsGamesTools.DebugTools;

namespace Superball
{
    public class BallConfig: BalanceSettings<BallConfig>
    {
        public float controlSensitity = 100f;
        public float samePipeSpeedMultiplier = 2f;
        public bool showJoystick = false;
        public int joystickMaxDistance = 300;
        public float strafeVelocity = 30;
        public bool resetJoystickInPipe = false;
   }
}