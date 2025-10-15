using UnityEngine;
using UnityEngine.UI;

public class SoundVolumeController : MonoBehaviour
{
    [Header("Slider điều chỉnh âm lượng BG")]
    public Slider volumeSlider;
    public Slider volumeSlider2;

    [Header("Ảnh hiển thị trạng thái âm thanh")]
    public Image soundIcon;          // Ảnh sẽ đổi giữa on/off
    public Image soundIcon2;          // Ảnh sẽ đổi giữa on/off

    [Header("Sprites cho icon âm thanh")]
    public Sprite soundOnSprite;     // Hình icon khi có âm thanh
    public Sprite soundOnSprite2;     // Hình icon khi có âm thanh
    public Sprite soundOffSprite;    // Hình icon khi tắt âm thanh
    public Sprite soundOffSprite2;    // Hình icon khi tắt âm thanh

    private void Start()
    {
        // Đặt âm lượng mặc định là 1 khi bắt đầu game
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBGVolume(1f);
        }

        // Khởi tạo giá trị slider và icon
        volumeSlider.value = 1f;
        UpdateSoundIcon(1f);

        // Lắng nghe khi người chơi thay đổi âm lượng
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        volumeSlider2.onValueChanged.AddListener(OnVolumeChanged2);
    }

    private void OnVolumeChanged(float value)
    {
        // Cập nhật âm lượng nhạc nền
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBGVolume(value);
        }

        // Cập nhật icon
        UpdateSoundIcon(value);
    }
    private void OnVolumeChanged2(float value)
    {
 
        // Cập nhật icon
        UpdateSoundIcon2(value);
    }

    private void UpdateSoundIcon(float value)
    {
        if (soundIcon == null) return;

        // Nếu value == 0 thì đổi sang icon off, ngược lại là on
        soundIcon.sprite = (value <= 0.001f) ? soundOffSprite : soundOnSprite;
    }

    private void UpdateSoundIcon2(float value)
    {
        if (soundIcon2 == null) return;

        // Nếu value == 0 thì đổi sang icon off, ngược lại là on
        soundIcon2.sprite = (value <= 0.001f) ? soundOffSprite2 : soundOnSprite2;
    }
}
