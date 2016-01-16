using UnityEngine;
using System.Collections;


// for the autocomplete!

public class Tags{
    public const string player = "Player";
    public const string people = "People";
    public const string person = "Person";
    public const string untagged = "Untagged";

    public class Scenes
    {
        public const string root = "RootScene";
#if UNITY_EDITOR
        public const string derek = "Derek";
#endif
        public const string select = "PlayerRegistration";
    }

    //public class Axis
    //{
    //    //public const string horizontal = "Horizontal";
    //    //public const string vertical = "Vertical";
    //}

    public class Layers
    {
    }

    public class AnimatorParams
    {
    }

    public class PlayerPrefKeys
    {
    }

    public class Options
    {
        public const string SoundLevel = "SoundLevel";
        public const string MusicLevel = "MusicLevel";
    }

    public class ShaderParams
    {
        public const string cutoff = "Cutoff";
    }
}