
using UnityEngine;

namespace BronobiMod
{
    internal static class Config
    {
        public static bool addLife = true;
        public static bool refillAmmos = true;
        public static bool givePockettedAmmo = true;
        public static bool giveFlexPower = true;

        public static bool respawnFriend = true;
        public static bool useProcGen = false;
        public static bool spawnDrone = false;
        public static bool spawnPig = false;

        public static SoundHolder CopySoundHolder(SoundHolder source)
        {
            SoundHolder result = new SoundHolder();
            result.alienBurst = source.alienBurst;
            result.alternateMeleeHitSound = source.alternateMeleeHitSound;
            result.alternateMeleeHitSound2 = source.alternateMeleeHitSound2;
            result.attack2Sounds = source.attack2Sounds;
            result.attack3Sounds = source.attack3Sounds;
            result.attack4Sounds = source.attack4Sounds;
            result.attackSounds = source.attackSounds;
            result.attractSounds = source.attractSounds;
            result.bassDrop = source.bassDrop;
            result.bleedSounds = source.bleedSounds;
            result.burnSounds = source.burnSounds;
            result.chokeSounds = source.chokeSounds;
            result.confused = source.confused;
            result.dashSounds = source.dashSounds;
            result.deathGargleSounds = source.deathGargleSounds;
            result.defendSounds = source.deathGargleSounds;
            result.hitSounds = source.hitSounds;
            result.effortSounds = source.effortSounds ;
            result.specialSounds = source.specialSounds;
            result.special2Sounds = source.special2Sounds;
            result.special3Sounds = source.special3Sounds;
            result.special4Sounds = source.special4Sounds;
            result.specialAttackSounds = source.specialAttackSounds;
            result.powerUp = source.powerUp;
            result.panic = source.panic;
            result.greeting = source.greeting;
            result.missSounds = source.missSounds;
            result.meleeHitSound = source.meleeHitSound;
            result.fallHitSound = source.fallHitSound;
            result.knockedSounds = source.knockedSounds;
            result.fizzleSounds = source.fizzleSounds;
            result.zappedSounds = source.zappedSounds;
            result.throwSounds = source.throwSounds;
            result.dismemberSounds = source.dismemberSounds;
            result.deathGargleSounds = source.deathGargleSounds;
            result.dizzieSounds = source.dizzieSounds;
            result.muffledScreams = source.muffledScreams;
            result.resurrectSounds = source.resurrectSounds;
            result.laughSounds = source.laughSounds;
            result.fallSounds = source.fallSounds;
            result.hurtSounds = source.hurtSounds;
            result.meleeHitTerrainSound = source.meleeHitTerrainSound;
            result.splashGunkSound = source.splashGunkSound;
            result.jetpackSound = source.jetpackSound;
            result.frozenGibSounds = source.frozenGibSounds;
            result.freezeScreamSounds = source.freezeScreamSounds;
            result.fartSounds = source.fartSounds;
            result.fartHugeSounds = source.fartHugeSounds;

            return result;
        }
        public static void UI()
        {
            GUILayout.Label("Bronobi Ghost Gifts:");
            GUILayout.Label("-- Some options may need additionals conditions --");
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            addLife = GUILayout.Toggle(addLife, new GUIContent("Add Life"));
            refillAmmos = GUILayout.Toggle(refillAmmos, new GUIContent("Refill Special"));
            givePockettedAmmo = GUILayout.Toggle(givePockettedAmmo, new GUIContent("Give Pocketted Ammo"));
            giveFlexPower = GUILayout.Toggle(giveFlexPower, new GUIContent("Give Flex Power"));
            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            //respawnFriend = GUILayout.Toggle(spawnPig, new GUIContent("Respawn Patriotic Friend"));
            spawnPig = GUILayout.Toggle(spawnPig, new GUIContent("Spawn Pig"));
            useProcGen = GUILayout.Toggle(useProcGen, new GUIContent("Proc Gen Items"));
           // spawnDrone = GUILayout.Toggle(spawnDrone, new GUIContent("Spawn Drone"));
            GUILayout.EndHorizontal();

        }
    }
}
