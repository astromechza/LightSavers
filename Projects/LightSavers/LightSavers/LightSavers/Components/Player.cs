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

        //MODEL BONES
        //here we need to figure out exactly which bones we have. 
        //fingers??
        ModelBone neck;

        //left size
        ModelBone leftShoulder;
        ModelBone leftElbow;
        ModelBone leftWrist;

        ModelBone leftHip;
        ModelBone leftKnee;
        ModelBone leftAnkle;

        //right size
        ModelBone rightShoulder;
        ModelBone rightElbow;
        ModelBone rightWrist;

        ModelBone rightHip;
        ModelBone rightKnee;
        ModelBone rightAnkle;
    }
}
