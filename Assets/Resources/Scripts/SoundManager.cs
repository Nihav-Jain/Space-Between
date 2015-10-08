using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    private const string breatheSoundLocation = "Audio/Dialogue/Scene02_JessNormalBreathing";   //Loading locations for audio
    private const string warningSoundLocation = "Audio/warningAlarm";
    private const string beepSoundLocation = "Audio/Quindar_UnMute";
    private const string proximityBeepSoundLocation = "Audio/beep1";
    private const string debrisAlarmSoundLocation = "Audio/resourceAlarm";
    private const string shortAirBurstLocation = "Audio/airBurstShort";
    private const string longAirBurstLocation = "Audio/airBurstLong";
    private const string thudLocation = "Audio/thud";
    private const string earRingingLocation = "Audio/earRinging";

    private static string[] jessCollisionSoundLocations = { "Audio/Dialogue/Scene02_JessCollision1",
    "Audio/Dialogue/Scene02_JessCollision2", "Audio/Dialogue/Scene02_JessCollision3"};

    private static string[] nickCollisionSoundLocations = { "Audio/Dialogue/Scene02_NickCollision1",
    "Audio/Dialogue/Scene02_NickCollision2", "Audio/Dialogue/Scene02_NickCollision3"};

    public static float breatheSoundVolume = 0.4f;                      //Volume settings for audio
    public static float warningSoundVolume = 0.05f;
    public static float beepSoundVolume = 0.05f;
    public static float proximityBeepSoundVolume = 0.05f;
    public static float debrisAlarmSoundVolume = 0.05f;
    public static float airBurstSoundVolume = 0.05f;
    public static float thudSoundVolume = 9.0f;
    public static float earRingingVolume = 0.1f;

    private AudioSource breatheSoundSource;                             //Sources for each separate audio to play
    private AudioSource warningSoundSource;
    private AudioSource beepSoundSource;
    private AudioSource proximityBeepSoundSource;
    private AudioSource debrisAlarmSoundSource;
    private AudioSource shortAirBurst;
    private AudioSource longAirBurst;
    private AudioSource thudSound;
    private AudioSource earRingingSound;
    private AudioSource jessCollisionSoundSource;
    private AudioSource nickCollisionSoundSource;

    private System.DateTime beepTimer; //How long before each beep?
    private float beepTimeElapsed;      //Saves when the game is paused
    public static float beepTimeMax = 5.0f;
    public static float breatheTimeMax = 8.0f;
    private bool waitingForBeep = false;

    private bool isJessSpeaking;

    void Start()
    {
        this.isJessSpeaking = false;
        this.playBreatheSound(Camera.main.transform.position);
    }

    public void playJessCollision(Vector3 location, float volume = 1.0f)
    {
        if (isJessSpeaking) return;
        if (this.jessCollisionSoundSource == null)
        {
            this.jessCollisionSoundSource = this.gameObject.AddComponent<AudioSource>();      
        }
        this.jessCollisionSoundSource.clip = Resources.Load(jessCollisionSoundLocations[Random.Range(0,
                jessCollisionSoundLocations.Length)]) as AudioClip;
        this.jessCollisionSoundSource.volume = volume;
        this.jessCollisionSoundSource.Play();
    }

    public void playNickCollision(Vector3 location, float volume = 1.0f)
    {
        if (this.nickCollisionSoundSource == null)
        {
            this.nickCollisionSoundSource = this.gameObject.AddComponent<AudioSource>();  
        }
        this.nickCollisionSoundSource.clip = Resources.Load(nickCollisionSoundLocations[Random.Range(0,
                nickCollisionSoundLocations.Length)]) as AudioClip;
        this.nickCollisionSoundSource.volume = volume;
        this.nickCollisionSoundSource.Play();
    }

    public void playBreatheSound(Vector3 location, float volume = 1.0f)
    {
        if(isJessSpeaking == false)
        {
            if (this.breatheSoundSource == null)
            {
                this.breatheSoundSource = this.gameObject.AddComponent<AudioSource>();
                this.breatheSoundSource.clip = Resources.Load(breatheSoundLocation) as AudioClip;
            }
            this.breatheSoundSource.volume = breatheSoundVolume;
            this.breatheSoundSource.loop = true;
            this.breatheSoundSource.Play();
        }
        
    }

    public void stopBreatheSound()
    {
        if (this.breatheSoundSource == null) breatheSoundSource.Stop();
    }

    public void playOxygenWarningSound(Vector3 location, float volume = 1.0f)
    {
        if (this.warningSoundSource == null)
        {
            this.warningSoundSource = this.gameObject.AddComponent<AudioSource>();
            this.warningSoundSource.clip = Resources.Load(warningSoundLocation) as AudioClip;
        }
        this.warningSoundSource.volume = warningSoundVolume;
        this.warningSoundSource.loop = true;
        this.warningSoundSource.Play();
    }

    public void playBeepSound(Vector3 location, float volume = 1.0f)
    {
        if (this.beepSoundSource == null)
        {
            this.beepSoundSource = this.gameObject.AddComponent<AudioSource>();
            this.beepSoundSource.clip = Resources.Load(beepSoundLocation) as AudioClip;
        }
        this.beepSoundSource.volume = beepSoundVolume;
        this.beepSoundSource.Play();
    }

    public void playProximityBeepSound(Vector3 location, float volume = 1.0f)
    {
        if (this.proximityBeepSoundSource == null)
        {
            this.proximityBeepSoundSource = this.gameObject.AddComponent<AudioSource>();
            this.proximityBeepSoundSource.clip = Resources.Load(proximityBeepSoundLocation) as AudioClip;
        }
        this.proximityBeepSoundSource.volume = proximityBeepSoundVolume;
        this.proximityBeepSoundSource.Play();
    }

    public void playDebrisAlarmSound(Vector3 location, float volume = 1.0f)
    {
        if (this.debrisAlarmSoundSource == null)
        {
            this.debrisAlarmSoundSource = this.gameObject.AddComponent<AudioSource>();
            this.debrisAlarmSoundSource.clip = Resources.Load(debrisAlarmSoundLocation) as AudioClip;
        }
        this.debrisAlarmSoundSource.volume = debrisAlarmSoundVolume;
        this.debrisAlarmSoundSource.loop = true;
        this.debrisAlarmSoundSource.Play();
    }

    public void playShortAirBurst(Vector3 location, float volume = 1.0f)
    {
        if (this.shortAirBurst == null)
        {
            this.shortAirBurst = this.gameObject.AddComponent<AudioSource>();
            this.shortAirBurst.clip = Resources.Load(shortAirBurstLocation) as AudioClip;
        }
        this.shortAirBurst.volume = airBurstSoundVolume;
        this.shortAirBurst.Play();
    }

    public void playLongAirBurst(Vector3 location,  float volume = 1.0f)
    {
        if (this.longAirBurst == null)
        {
            this.longAirBurst = this.gameObject.AddComponent<AudioSource>();
            this.longAirBurst.clip = Resources.Load(longAirBurstLocation) as AudioClip;
        }
        this.longAirBurst.volume = airBurstSoundVolume;
        this.longAirBurst.Play();
    }

    public void playThud(Vector3 location, float volume = 1.0f)
    {
        if (this.thudSound == null)
        {
            this.thudSound = this.gameObject.AddComponent<AudioSource>();
            this.thudSound.clip = Resources.Load(thudLocation) as AudioClip;
        }
        this.thudSound.volume = thudSoundVolume;
        this.thudSound.Play();
    }
    public void playEarRinging(Vector3 location, float volume = 1.0f)
    {
        if (this.earRingingSound == null)
        {
            this.earRingingSound = this.gameObject.AddComponent<AudioSource>();
            this.earRingingSound.clip = Resources.Load(earRingingLocation) as AudioClip;
        }
        this.earRingingSound.volume = earRingingVolume;
        this.earRingingSound.Play();
    }


    void updateBeep()
    {
        if (!this.waitingForBeep)
        {
            this.beepTimer = System.DateTime.Now; //Start timer
            this.waitingForBeep = true;
        }
        float timeRemaining = (float)(System.DateTime.Now - this.beepTimer).TotalSeconds - beepTimeElapsed;
        if (timeRemaining > beepTimeMax) //Test how long things pass in seconds
        {
            beepTimeElapsed = 0;
            this.playBeepSound(Camera.main.transform.position);
            this.waitingForBeep = false;
        }
    }

    //void updateBreathe()
    //{
    //    if (!this.waitingForBreathe)
    //    {
    //        this.breatheTimer = System.DateTime.Now; //Start timer
    //        this.waitingForBreathe = true;
    //    }
    //    float timeRemaining = (float)(System.DateTime.Now - this.breatheTimer).TotalSeconds - breatheTimeElapsed;
    //    if (timeRemaining > breatheTimeMax) //Test how long things pass in seconds
    //    {
    //        breatheTimeElapsed = 0;
    //        this.playBreatheSound(Camera.main.transform.position);
    //        this.waitingForBreathe = false;
    //    }
    //}

    public void stopSounds()
    {
        if (breatheSoundSource) breatheSoundSource.Stop();
        if (warningSoundSource) warningSoundSource.Stop();
        if (beepSoundSource) beepSoundSource.Stop();
        if (proximityBeepSoundSource) proximityBeepSoundSource.Stop();
        if (debrisAlarmSoundSource) debrisAlarmSoundSource.Stop();
        if (longAirBurst) longAirBurst.Stop();
        if (shortAirBurst) shortAirBurst.Stop();
    }

    public void playAirBurstOnce(Vector3 location)
    {

    }

    public void Pause()
    {
        foreach (AudioSource source in this.gameObject.GetComponents<AudioSource>())
        {
            source.Pause();
        }
        beepTimeElapsed = (float)(System.DateTime.Now - this.beepTimer).TotalSeconds % beepTimeMax;
    }

    public void UnPause()
    {
        foreach (AudioSource source in this.gameObject.GetComponents<AudioSource>())
        {
            source.UnPause();
        }
    }

    void Update()
    {
        if (GameObject.Find("MainGame").GetComponent<MainScript>().isPaused()) return;
        this.updateBeep();
    }

    public void setJessSpeaking(bool isJessSpeaking)
    {
        this.isJessSpeaking = isJessSpeaking;
    }
}
