using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//imports taken from Tank class
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LightSavers.Components
{
    class Player
    {

        //***INITIAILISNG ATTRIBUTES OF PLAYER OBJECT***//

        //MODEL AS A WHOLE
        Model playerModel;
        //Animation_recs.AnimatedModel playerAnimatedModel;
        //MODEL BONES
        //here we need to figure out exactly which bones we have. 
        //fingers??
        ModelBone neckBone;

        //left size
        ModelBone leftShoulderBone;
        ModelBone leftElbowBone;
        ModelBone leftWristBone;

        ModelBone leftHipBone;
        ModelBone leftKneeBone;
        ModelBone leftAnkleBone;

        //right size
        ModelBone rightShoulderBone;
        ModelBone rightElbowBone;
        ModelBone rightWristBone;

        ModelBone rightHipBone;
        ModelBone rightKneeBone;
        ModelBone rightAnkleBone;

        //MATRICES TO STORE ORIGINAL TRANSFORMS
        Matrix neckTransform;

        //left size
        Matrix leftShoulderTransform;
        Matrix leftElbowTransform;
        Matrix leftWristTransform;

        Matrix leftHipTransform;
        Matrix leftKneeTransform;
        Matrix leftAnkleTransform;

        //right size
        Matrix rightShoulderTransform;
        Matrix rightElbowTransform;
        Matrix rightWristTransform;

        Matrix rightHipTransform;
        Matrix rightKneeTransform;
        Matrix rightAnkleTransform;

        //ARRAY OF TRANSFORMATIONS TO STORE ALL TRANSFORMATIONS
        Matrix[] boneTransforms;

        //DIRECTIONS AND POSITIONS
        public Vector3 Position;
        public Vector3 Direction;
        public Vector3 Up;
        public Vector3 right;


        //LOAD METHOD
        public void Load(ContentManager content)
        {

            //load all the bones
            neckBone = playerModel.Bones[""];

            leftShoulderBone = playerModel.Bones[""];
            leftElbowBone = playerModel.Bones[""];
            leftWristBone = playerModel.Bones[""];
            leftHipBone = playerModel.Bones[""];
            leftKneeBone = playerModel.Bones[""];
            leftAnkleBone = playerModel.Bones[""];

            rightShoulderBone = playerModel.Bones[""];
            rightElbowBone = playerModel.Bones[""];
            rightWristBone = playerModel.Bones[""];
            rightHipBone = playerModel.Bones[""];
            rightKneeBone = playerModel.Bones[""];
            rightAnkleBone = playerModel.Bones[""];

            //load the transforms
            neckTransform = neckBone.Transform;

            leftShoulderTransform = leftShoulderBone.Transform;
            leftElbowTransform = leftElbowBone.Transform;
            leftWristTransform = leftWristBone.Transform;
            leftHipTransform = leftHipBone.Transform;
            leftKneeTransform = leftKneeBone.Transform;
            leftAnkleTransform = leftAnkleBone.Transform;

            rightShoulderTransform = rightShoulderBone.Transform;
            rightElbowTransform = rightElbowBone.Transform;
            rightWristTransform = rightWristBone.Transform;
            rightHipTransform = rightHipBone.Transform;
            rightKneeTransform = rightKneeBone.Transform;
            rightAnkleTransform = rightAnkleBone.Transform;

            //initialisng the transform matrix array
            boneTransforms = new Matrix[playerModel.Bones.Count];

            //setting directions
            Direction = new Vector3(1, 0, 0);
            Up = Vector3.Up;
        }

        //GETTING USER INPUT TO MOVE THE PLAYER AROUND
        public void getUserInput(GamePadState currentGamePadState, KeyboardState currentKeyboardState, GameTime gameTime)
        {

        }

        public void Draw(Matrix view, Matrix projection)
        {

        }

    }
}
