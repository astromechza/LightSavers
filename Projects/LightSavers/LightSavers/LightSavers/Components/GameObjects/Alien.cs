using LightPrePassRenderer;
using LightPrePassRenderer.partitioning;
using LightSavers.Utils;
using Microsoft.Xna.Framework;
using SkinnedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.GameObjects
{
    public class Alien : GameObject
    {
        private Vector3 VERTICAL_OFFSET = new Vector3(0, 0.8f, 0);
        private Matrix SCALE = Matrix.CreateScale(0.6f);

        private SkinnedMesh mesh;
        private MeshSceneGraphReceipt modelReceipt;
        private DurationBasedAnimator aplayer;
        private RealGame game;
        private float rotation;
        private float targetRotation;

        private Vector3 targetPosition;
        private Vector3 delta;

        public Alien(RealGame game, Vector3 spawnPosition)
        {
            this.game = game;
            this.position = spawnPosition;
            this.rotation = (float)Globals.random.NextDouble() * MathHelper.TwoPi;

            this.mesh = new SkinnedMesh();
            this.mesh.Model = AssetLoader.mdl_alien1;

            this.aplayer = new DurationBasedAnimator(mesh.SkinningData, mesh.SkinningData.AnimationClips["Take 001"]);

            aplayer.AddAnimationPackage = AssetLoader.ani_alien1;
            aplayer.StartClip("moving");

            UpdateAnimation(0);
            UpdateMajorTransforms(0);

            this.game.sceneGraph.Setup(mesh);
            this.modelReceipt = game.sceneGraph.Add(mesh);

            this.targetPosition = new Vector3();
            AssignRandomTarget();


        }

        private void UpdateAnimation(float ms)
        {
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int)ms);
            aplayer.Update(ts, true, Matrix.Identity);
            mesh.BoneMatrixes = aplayer.GetSkinTransforms();
        }

        private void UpdateMajorTransforms(float ms)
        {
            mesh.Transform = SCALE * Matrix.CreateRotationY(rotation + (float)Math.PI) * Matrix.CreateTranslation(position + VERTICAL_OFFSET);                       
        }

        public override void Update(float ms)
        {
            float deltarotation = targetRotation - rotation;

            // sanitise
            if (deltarotation > Math.PI) deltarotation -= MathHelper.TwoPi;
            if (deltarotation < -Math.PI) deltarotation += MathHelper.TwoPi;

            // add difference
            rotation += (ms / 200) * deltarotation;

            
            float distance = (targetPosition - position).LengthSquared();
            if (distance < 0.4f)
            {
                AssignRandomTarget();
            }

            if (deltarotation < 0.15f)
            {
                position += delta * 0.03f;
            }


            UpdateAnimation(ms*1.5f);
            UpdateMajorTransforms(ms);
        }

        public void AssignRandomTarget()
        {
            this.targetPosition.X = (float)Globals.random.NextDouble() * 10+1;
            this.targetPosition.Z = (float)Globals.random.NextDouble() * 10+1;
            this.delta = targetPosition - position;
            this.delta.Normalize();
            this.targetRotation = (float)Math.Atan2(-delta.Z, delta.X) + MathHelper.PiOver2;
        }

        public override RectangleF GetBoundRect()
        {
            return new RectangleF();
        }
    }
}
