using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class Projectile
    {
        private TGCVector3 initialPosition;
        private TGCVector3 initialSpeedDirection;
        private float timeSinceShot = 0;

        public Projectile(TGCVector3 initialPos, TGCVector3 initialSpeedDir)
        {
            initialPosition = initialPos;
            initialSpeedDirection = initialSpeedDir;
        }

        public void updateTimeSinceShot(float time)
        {
            timeSinceShot += time;
        }

        public TGCVector3 getInitialPosition()
        {
            return initialPosition;
        }

        public TGCVector3 getInitialSpeedDirection()
        {
            return initialSpeedDirection;
        }

        public float getTimeSinceShot()
        {
            return timeSinceShot;
        }

    }
}
