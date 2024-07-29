using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("#Bgm")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmHighPassFilter;

    [Header("#SFX")]
    public AudioClip[] sfxClip;
    public float sfxVolume;
    [Tooltip("여러 사운드의 개수")]
    public int channels;
     AudioSource[] sfxPlayer;
    int channelIndex;

    public enum Sfx
    {
        Laser, FirExplosion, IceBall, Wind, Heal, ClickButton
    }

    private void Awake()
    {
        Instance = this;
        Init();
    }

    void Init()
    {
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;
        bgmHighPassFilter = Camera.main.GetComponent<AudioHighPassFilter>();

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayer = new AudioSource[channels];

        for (int index = 0; index < sfxPlayer.Length; index++)
        {
            sfxPlayer[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayer[index].playOnAwake = false;
            sfxPlayer[index].bypassListenerEffects = true; // 메인카메라 리스너의 이펙트 조절
            sfxPlayer[index].volume = sfxVolume;
        }
    }

    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }

    public void EffectBgm(bool isPlay)
    {
        bgmHighPassFilter.enabled = isPlay;
    }

    public void PlaySfx(Sfx sfx)
    {
        // 재생되지 않은 오디오 쓰기
        for (int index = 0; index < sfxPlayer.Length; index++)
        {
            int loopindex = (index + channelIndex) % sfxPlayer.Length;
            if (sfxPlayer[loopindex].isPlaying)
                // 쉬고 있는 오디오 변수를 만나면 넘어감
                continue;
            channelIndex = loopindex;
            sfxPlayer[loopindex].clip = sfxClip[(int)sfx];
            sfxPlayer[loopindex].Play();
            // for문을 끝내야 함
            break;
        }
    }

    public void StopAllSfx()
    {
        for (int index = 0; index < sfxPlayer.Length; index++)
        {
            sfxPlayer[index].volume =0f;
        }
    }
}
