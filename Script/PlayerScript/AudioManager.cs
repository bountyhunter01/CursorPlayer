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
    [Tooltip("���� ������ ����")]
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
        // ����� �÷��̾� �ʱ�ȭ
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;
        bgmHighPassFilter = Camera.main.GetComponent<AudioHighPassFilter>();

        // ȿ���� �÷��̾� �ʱ�ȭ
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayer = new AudioSource[channels];

        for (int index = 0; index < sfxPlayer.Length; index++)
        {
            sfxPlayer[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayer[index].playOnAwake = false;
            sfxPlayer[index].bypassListenerEffects = true; // ����ī�޶� �������� ����Ʈ ����
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
        // ������� ���� ����� ����
        for (int index = 0; index < sfxPlayer.Length; index++)
        {
            int loopindex = (index + channelIndex) % sfxPlayer.Length;
            if (sfxPlayer[loopindex].isPlaying)
                // ���� �ִ� ����� ������ ������ �Ѿ
                continue;
            channelIndex = loopindex;
            sfxPlayer[loopindex].clip = sfxClip[(int)sfx];
            sfxPlayer[loopindex].Play();
            // for���� ������ ��
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
