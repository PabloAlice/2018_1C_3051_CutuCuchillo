using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class RigidBodies
    {
        public static RigidBody CreateVehicleRigidBody(float mass, float size, TGCVector3 origin, float yaw = 0, float pitch = 0, float roll = 0)
        {
            CollisionShape chassisShape = new BoxShape(size);

            CompoundShape compound = new CompoundShape();

            //localTrans effectively shifts the center of mass with respect to the chassis
            var unitY = new TGCVector3(0f,1f,0f);
            TGCMatrix localTrans = TGCMatrix.Translation(unitY);
            compound.AddChildShape(localTrans.ToBsMatrix, chassisShape);
            chassisShape.UserObject = "Chassis";
            var boxTransform = TGCMatrix.RotationYawPitchRoll(yaw, pitch, roll).ToBsMatrix;
            boxTransform.Origin = origin.ToBsVector;
            DefaultMotionState boxMotionState = new DefaultMotionState(boxTransform);
            //Es importante calcular la inercia caso contrario el objeto no rotara.
            var boxLocalInertia = chassisShape.CalculateLocalInertia(mass);
            var boxInfo = new RigidBodyConstructionInfo(mass, boxMotionState, chassisShape, boxLocalInertia);
            var boxBody = new RigidBody(boxInfo);
            boxBody.LinearFactor = TGCVector3.One.ToBsVector;
            //boxBody.SetDamping(0.7f, 0.9f);
            //boxBody.Restitution = 1f;
            return boxBody;
        }
        public static RigidBody CreateWall(float dx, float dy, float dz, float x, float y, float z, float yaw, float pitch, float roll)
        {
            var boxShape = new BoxShape(dx, dy, dz);
            var boxTransform = TGCMatrix.RotationYawPitchRoll(yaw, pitch, roll).ToBsMatrix;
            boxTransform.Origin = new TGCVector3(x, y, z).ToBsVector;
            DefaultMotionState boxMotionState = new DefaultMotionState(boxTransform);
            //Es importante calcular la inercia caso contrario el objeto no rotara.
            var boxInfo = new RigidBodyConstructionInfo(0, boxMotionState, boxShape);
            var boxBody = new RigidBody(boxInfo);
            boxBody.LinearFactor = TGCVector3.One.ToBsVector;
            //boxBody.SetDamping(0.7f, 0.9f);
            //boxBody.Restitution = 1f;
            return boxBody;
        }
    }
}
