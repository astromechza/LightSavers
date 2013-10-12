using LightPrePassRenderer;
using LightPrePassRenderer.partitioning;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.Guns
{
    public class BaseGun
    {
        public Mesh mesh;
        public MeshSceneGraphReceipt receipt;
        public Matrix holdTransform;
        public Vector3 emmitterPosition;
        public Vector3 emmitterVector;

        public DateTime lastShot;
        public TimeSpan coolDown;
        public TimeSpan animationCoolDown;

        public BaseGun()
        {
            mesh = null;
            receipt = null;
            holdTransform = Matrix.Identity;
            emmitterVector = Vector3.Zero;
            emmitterPosition = Vector3.Zero;
            lastShot = DateTime.Now;
            coolDown = TimeSpan.Zero;
            animationCoolDown = TimeSpan.Zero;
        }

        public void SetModel(Model m)
        {
            mesh = new Mesh();
            mesh.Model = m;
        }

        public void SetEmmitterVector(Vector3 v)
        {
            emmitterVector = v;
        }

        public void SetTransform(Matrix handTransform, Matrix playerTransform)
        {
            mesh.Transform = holdTransform * handTransform * playerTransform;
            emmitterPosition = Vector3.Transform(emmitterVector, mesh.Transform);
        }

        public void SetHoldTransform(Matrix t)
        {
            holdTransform = t;
        }

        public bool CanFire()
        {
            return (DateTime.Now-lastShot) > coolDown;
        }

        public virtual void Fire(float rotation)
        {
            PostFire();
        }

        public void PostFire()
        {
            lastShot = DateTime.Now;
        }

        public bool AnimationComplete()
        {
            return (DateTime.Now - lastShot) > animationCoolDown;
        }

    }
}
