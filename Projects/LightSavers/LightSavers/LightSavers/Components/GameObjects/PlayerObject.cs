using LightPrePassRenderer;
using LightPrePassRenderer.partitioning;
using LightSavers.Components.Guns;
using LightSavers.Components.Projectiles;
using LightSavers.Utils;
using LightSavers.Utils.Geometry;
using Microsoft.Xna.Framework;
using SkinnedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.GameObjects
{
    public class PlayerObject : GameObject
    {
        #region CONSTANTS
        // Player orientation
        const float PLAYER_YORIGIN = 1f;
        const float PLAYER_SCALE = 0.75f;

        // Light stuff
        const float TORCH_HEIGHT = 2.5f;
        const float HALO_HEIGHT = 6.0f;

        Matrix mPlayerScale = Matrix.CreateScale(PLAYER_SCALE);
        Matrix mHaloPitch = Matrix.CreateRotationX(-90);
        Matrix mTorchPitch = Matrix.CreateRotationX(-0.4f);
        #endregion

        private PlayerIndex playerIndex;
        private Color color;

        private SkinnedMesh mesh;

        private RectangleF collisionRectangle;

        private float rotation;

        // Lighting variables
        private Light torchlight;
        private Light halolight;
        private Light haloemitlight;
        

        private DurationBasedAnimator upPlayer;
        private DurationBasedAnimator lowPlayer;

        private RealGame game;
        
        // Scenegraphstuff
        private MeshSceneGraphReceipt modelReceipt;
        private LightSceneGraphReceipt light1receipt;
        private LightSceneGraphReceipt light2receipt;
        private LightSceneGraphReceipt light3receipt;

        private BaseGun[] weapons;
        private int currentWeapon;
        private int currentAnimation;
        private int currentFiringAnimation;
        int moving=0, weapon=Animation_States.pistol, shooting=0;

        public PlayerObject(RealGame game, PlayerIndex playerIndex, Color color, Vector3 pos, float initialYRot)
        {
            this.game = game;
            this.playerIndex = playerIndex;
            this.color = color;

            // initial transform
            this._position = pos;
            rotation = initialYRot;

            SetupLights();

            mesh = new SkinnedMesh();
            mesh.Model = AssetLoader.mdl_character;

            //Create a new Animation Player that will take the bone dictionaries as arguments allowing individual animation with upper and lower body
            upPlayer = new DurationBasedAnimator(mesh.SkinningData, mesh.SkinningData.AnimationClips["Take 001"],  Animation_States.upperCharacterBones);
            lowPlayer = new DurationBasedAnimator(mesh.SkinningData, mesh.SkinningData.AnimationClips["Take 001"], Animation_States.lowerCharacterBonesandRoot);
            //Load the animations from the asset loader (these are in an Animation Package)
            upPlayer.AddAnimationPackage = AssetLoader.ani_character;
            upPlayer.StartClip(moving + shooting + weapon);
            lowPlayer.AddAnimationPackage = AssetLoader.ani_character;
            lowPlayer.StartClip(moving+weapon);
            

            UpdateAnimation(0);
            UpdateMajorTransforms(0);

            game.sceneGraph.Setup(mesh);
            modelReceipt = game.sceneGraph.Add(mesh);
            light1receipt = game.sceneGraph.Add(torchlight);
            light2receipt = game.sceneGraph.Add(haloemitlight);
            light3receipt = game.sceneGraph.Add(halolight);

            SetupWeapons();
            SwitchWeapon(0);

            collisionRectangle = new RectangleF(0, 0, 0.98f, 0.98f);

        }

        private void UpdateAnimation(float ms)
        {
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int) ms);
            lowPlayer.Update(ts, true, Matrix.Identity, Matrix.Identity);
            upPlayer.Update(ts, true, Matrix.Identity, lowPlayer.GetWorldTransforms()[0]);
            
            mesh.BoneMatrixes = lowPlayer.MergeTransforms(upPlayer.GetSkinTransforms());
        }

        private void UpdateMajorTransforms(float ms)
        {
            mesh.Transform = mPlayerScale * Matrix.CreateRotationY(rotation + (float)Math.PI) * Matrix.CreateTranslation(_position + new Vector3(0, PLAYER_YORIGIN, 0));

            halolight.Transform = mHaloPitch * Matrix.CreateTranslation(_position + new Vector3(0, HALO_HEIGHT, 0));
            haloemitlight.Transform = mHaloPitch * Matrix.CreateTranslation(_position + new Vector3(0, 2, 0));
            torchlight.Transform = mTorchPitch * Matrix.CreateTranslation(0, 0, 0.3f) * Matrix.CreateRotationY(rotation) * Matrix.CreateTranslation(_position + new Vector3(0, TORCH_HEIGHT, 0));
        }

        public override void Update(float ms)
        {

            Vector3 newposition = new Vector3(_position.X, _position.Y, _position.Z);

            // 1. == Update Movement
            // movement is done via analog sticks
            Vector2 vleft = Globals.inputController.getAnalogVector(AnalogStick.Left, playerIndex);
            Vector2 vright = Globals.inputController.getAnalogVector(AnalogStick.Right, playerIndex);
            // 1.1 = Update player rotation based on RIGHT analog stick
            if (vright.LengthSquared() > 0.01f)
            {
                // get target angle
                float targetrotation = (float)Math.Atan2(vright.Y, vright.X) - MathHelper.PiOver2;

                // get difference
                float deltarotation = targetrotation - rotation;

                // sanitise
                if (deltarotation > Math.PI)
                    deltarotation -= MathHelper.TwoPi;
                if (deltarotation < -Math.PI)
                    deltarotation += MathHelper.TwoPi;

                // add difference
                rotation += (ms / 300) * deltarotation;

                // sanitise rotation
                if (rotation > MathHelper.TwoPi) rotation -= MathHelper.TwoPi;
                if (rotation < -MathHelper.TwoPi) rotation += MathHelper.TwoPi;
            }

            // 1.2 = Update player movement based on LEFT analog stick
            if (vleft.LengthSquared() > 0.1f)
            {
                Vector3 pdelta = new Vector3(vleft.X, 0, -vleft.Y);
                pdelta.Normalize();
                // modifies the horizantal direction
                newposition += pdelta * ms / 300;

                // 1.3 = If no rotation was changed, pull player angle toward forward vector
                if (vright.LengthSquared() < 0.01f)
                {
                    // get target angle
                    float targetrotation = (float)Math.Atan2(vleft.Y, vleft.X) - MathHelper.PiOver2;

                    // get difference
                    float deltarotation = targetrotation - rotation;

                    // sanitise
                    if (deltarotation > Math.PI)
                        deltarotation -= MathHelper.TwoPi;
                    if (deltarotation < -Math.PI)
                        deltarotation += MathHelper.TwoPi;

                    // add difference
                    rotation += (ms / 300) * deltarotation;

                    // sanitise rotation
                    if (rotation > MathHelper.TwoPi) rotation -= MathHelper.TwoPi;
                    if (rotation < -MathHelper.TwoPi) rotation += MathHelper.TwoPi;
                }

                moving = Animation_States.run;
            }
            else
            {
                moving = Animation_States.idle;
            }

            if (Globals.inputController.isTriggerDown(Triggers.Right, playerIndex))
            {
                if (currentWeapon > -1)
                {
                    float r = (float)Globals.random.NextDouble() * 0.1f - 0.05f;

                    StandardBullet b = game.projectileManager.standardBulletPool.Provide();
                    b.Construct(game, weapons[currentWeapon].emmitterPosition, rotation + MathHelper.PiOver2 + r);
                }

                shooting = Animation_States.shoot;
            }
            else
            {
                shooting = 0; // not shooting
            }

            if(Globals.inputController.isButtonPressed(Microsoft.Xna.Framework.Input.Buttons.Y, playerIndex))
            {
                int nw = (currentWeapon + 1) % 5;
                //Switch weapon animations

                if (nw == 1 || nw == 3)
                    weapon = Animation_States.snipshot;
                else if (nw == 0)
                    weapon = Animation_States.pistol;
                else if (nw == 2)
                    weapon = Animation_States.assault;
                else if (nw == 4)
                    weapon = Animation_States.sword;
                
                SwitchWeapon(nw);
            }


            // collision stuff
            if (_position != newposition)
            {
                // First test X collision
                collisionRectangle.Left = newposition.X - 0.49f;
                collisionRectangle.Top  = _position.Z - 0.49f;
                if (game.cellCollider.RectangleCollides(collisionRectangle))
                {
                    // if it does collide, pull it back
                    newposition.X = _position.X;                    
                }

                // Then test Z collision
                collisionRectangle.Left = _position.X - 0.49f;
                collisionRectangle.Top  = newposition.Z - 0.49f;
                if (game.cellCollider.RectangleCollides(collisionRectangle))
                {
                    // if it does collide, pull it back
                    newposition.Z = _position.Z;
                }

                // if there is still a new position
                if (_position != newposition)
                {
                    _position = newposition;

                    modelReceipt.graph.Renew(modelReceipt);
                    light1receipt.graph.Renew(light1receipt);
                    light2receipt.graph.Renew(light2receipt);
                    light3receipt.graph.Renew(light3receipt);
                }
            }

            UpdateAnimation(ms);
            UpdateMajorTransforms(ms);

            if (currentWeapon > -1)
            {
                weapons[currentWeapon].SetTransform(upPlayer.GetWorldTransforms()[31], mesh.Transform);
                weapons[currentWeapon].receipt.graph.Renew(weapons[currentWeapon].receipt);
            }

            // Update Top half of body
            if (currentFiringAnimation != moving + shooting + weapon)
            {
                currentFiringAnimation = moving + shooting + weapon;
                upPlayer.StartClip(currentFiringAnimation);
            }

            //Update Bottom half of body
            if (currentAnimation != moving + weapon)
            {
                currentAnimation = moving + weapon;
                lowPlayer.StartClip(currentAnimation);
            }
            
        }

        public void SetupWeapons()
        {
            weapons = new BaseGun[5];
            weapons[0] = new Pistol();
            game.sceneGraph.Setup(weapons[0].mesh);
            weapons[1] = new Shotgun();
            game.sceneGraph.Setup(weapons[1].mesh);
            weapons[2] = new AssaultRifle();
            game.sceneGraph.Setup(weapons[2].mesh);
            weapons[3] = new SniperRifle();
            game.sceneGraph.Setup(weapons[3].mesh);
            weapons[4] = new Sword();
            game.sceneGraph.Setup(weapons[4].mesh);

            currentWeapon = -1;
            
        }

        public void SwitchWeapon(int to)
        {
            //disable current weapon
            if (currentWeapon > -1)
            {
                game.sceneGraph.Remove(weapons[currentWeapon].receipt);
            }

            currentWeapon = to;
            BaseGun g = weapons[to];
            g.SetTransform(upPlayer.GetWorldTransforms()[31], mesh.Transform);

            if (g.receipt != null) game.sceneGraph.Remove(g.receipt);
            g.receipt = game.sceneGraph.Add(g.mesh);
        }

        public void SetupLights()
        {

            torchlight = new Light();
            torchlight.LightType = Light.Type.Spot;
            torchlight.ShadowDepthBias = 0.002f;
            torchlight.Radius = 15;
            torchlight.SpotAngle = 25;
            torchlight.Intensity = 1.0f;
            torchlight.SpotExponent = 6;
            torchlight.Color = color*1.1f;
            torchlight.CastShadows = true;
            torchlight.Transform = Matrix.Identity;

            halolight = new Light();
            halolight.LightType = Light.Type.Spot;
            halolight.ShadowDepthBias = 0.001f;
            halolight.Radius = HALO_HEIGHT+1;
            halolight.SpotAngle = 35;
            halolight.SpotExponent = 0.6f;
            halolight.Intensity = 0.8f;
            halolight.Color = color;
            halolight.CastShadows = true;
            halolight.Transform = Matrix.Identity;

            haloemitlight = new Light();
            haloemitlight.LightType = Light.Type.Point;
            haloemitlight.Intensity = 0.4f;
            haloemitlight.Radius = 3;
            haloemitlight.Color = color * 1.4f;
            haloemitlight.Transform = Matrix.Identity;
            haloemitlight.SpecularIntensity = 3;

        }


        public Light[] GetLights()
        {
            return new Light[] { 
                torchlight, 
                halolight, 
                haloemitlight 
            };
        }
        public Mesh GetMesh()
        {
            return mesh;
        }

        public void AddCriticalPoints(List<Vector2> outputPoints)
        {
            outputPoints.Add(new Vector2(_position.X, _position.Z));

            Vector3 v = new Vector3(0, 0, -7);
            Matrix m = Matrix.CreateRotationY(rotation) * Matrix.CreateTranslation(_position);
            Vector3 t = Vector3.Transform(v, m);
            outputPoints.Add(new Vector2(t.X,t.Z));
        }
    }
}
