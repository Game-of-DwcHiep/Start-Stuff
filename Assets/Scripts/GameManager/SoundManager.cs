using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{

    [Header("Audio Sources")]
    public AudioSource bgmSource;    // Dành cho nhạc nền (loop)
    public AudioSource sfxSource;    // Dành cho nhạc thắng hoặc hiệu ứng

    [Header("Audio Clips")]
    public AudioClip bgmClip;        // Nhạc nền
    public AudioClip winClip;        // Nhạc khi thắng level

    private void Start()
    {
        PlayBGM();
    }

    // ------------------------------------------------------------
    // 🎵 Phát nhạc nền (loop)
    // ------------------------------------------------------------
    public void PlayBGM()
    {
        if (bgmSource == null || bgmClip == null) return;

        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.volume = 0.7f;
        bgmSource.Play();
    }

    public void SetBGVolume(float value)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = Mathf.Clamp01(value);
        }
    }

    // ------------------------------------------------------------
    // 🏁 Phát nhạc khi thắng level (chỉ 1 lần)
    // ------------------------------------------------------------
    public void PlayWinMusic()
    {
        if (sfxSource == null || winClip == null) return;

        sfxSource.loop = false;
        sfxSource.volume = 1f;
        sfxSource.PlayOneShot(winClip);
    }

    // ------------------------------------------------------------
    // ⏹ Dừng nhạc nền (nếu cần)
    // ------------------------------------------------------------
    public void StopBGM()
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();
    }

    // ------------------------------------------------------------
    // 🔊 Dừng toàn bộ âm thanh
    // ------------------------------------------------------------
    public void StopAllSounds()
    {
        bgmSource.Stop();
        sfxSource.Stop();
    }
}
