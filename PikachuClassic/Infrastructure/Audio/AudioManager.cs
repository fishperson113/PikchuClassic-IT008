using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WMPLib;

namespace PikachuClassic
{
    public class AudioManager
    {
        #region Singleton
        private static AudioManager instance;
        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AudioManager();
                }
                return instance;
            }
        }
        #endregion

        private Dictionary<string, string> soundEffects;  // Lưu đường dẫn tệp âm thanh
        private WindowsMediaPlayer player;  // Dùng WindowsMediaPlayer cho MP3
        private Thread backgroundMusicThread;
        private bool isPlayingBackgroundMusic;
        private string currentBackgroundMusic;

        private AudioManager()
        {
            soundEffects = new Dictionary<string, string>();
            player = new WindowsMediaPlayer();  // Khởi tạo WindowsMediaPlayer
            isPlayingBackgroundMusic = false;
        }

        // Nạp âm thanh (Lưu đư��ng dẫn tệp âm thanh vào Dictionary)
        public void LoadSound(string soundName, string filePath)
        {
            if (!soundEffects.ContainsKey(soundName))
            {
                try
                {
                    soundEffects[soundName] = filePath;  // Lưu đường dẫn tệp âm thanh vào dictionary
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Không thể tải âm thanh {soundName}: {ex.Message}");
                }
            }
        }

        // Phát âm thanh (Hỗ trợ WAV với SoundPlayer, MP3 với WindowsMediaPlayer)
        public async Task PlaySoundAsync(string soundName, int durationInSeconds)
        {
            if (!soundEffects.ContainsKey(soundName))
            {
                Console.WriteLine($"Âm thanh {soundName} chưa được tải.");
                return;
            }

            try
            {
                string filePath = soundEffects[soundName];
                string fullPath = System.IO.Path.GetFullPath(filePath);

                if (fullPath.EndsWith(".mp3"))
                {
                    await PlayMP3Async(fullPath, durationInSeconds);
                }
                else
                {
                    await PlayWAVAsync(fullPath, durationInSeconds);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi phát âm thanh {soundName}: {ex.Message}");
            }
        }

        private async Task PlayMP3Async(string fullPath, int durationInSeconds)
        {
            player.URL = fullPath;
            player.controls.play();
            await Task.Delay(durationInSeconds * 1000);
            player.controls.stop();
        }

        private async Task PlayWAVAsync(string fullPath, int durationInSeconds)
        {
            using (var soundPlayer = new System.Media.SoundPlayer(fullPath))
            {
                soundPlayer.Play();
                await Task.Delay(durationInSeconds * 1000);
            }
        }

        // Xóa âm thanh khỏi bộ nhớ
        public void UnloadSound(string soundName)
        {
            if (soundEffects.ContainsKey(soundName))
            {
                soundEffects.Remove(soundName);
            }
        }

        // Phát nhạc nền sử dụng soundName
        public void StartBackgroundMusic(string soundName)
        {
            if (!isPlayingBackgroundMusic && soundEffects.ContainsKey(soundName))
            {
                try
                {
                    Console.WriteLine($"Đang phát nhạc nền: {soundName}");
                    currentBackgroundMusic = soundName;
                    isPlayingBackgroundMusic = true;
                    backgroundMusicThread = new Thread(() => PlayBackgroundMusic(soundName));
                    backgroundMusicThread.IsBackground = true;
                    backgroundMusicThread.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi phát nhạc nền: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Nhạc nền đã đang phát hoặc âm thanh không tồn tại.");
            }
        }

        // Dừng nhạc nền
        public void ToggleBackgroundMusic()
        {
            try
            {
                if (isPlayingBackgroundMusic)  // Nếu nhạc đang phát, dừng nhạc
                {
                    player.controls.stop();
                    isPlayingBackgroundMusic = false;
                }
                else  // Nếu nhạc không phát, phát nhạc
                {
                    player.controls.play();
                    isPlayingBackgroundMusic = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi điều khiển nhạc nền: {ex.Message}");
            }
        }

        // Phát nhạc nền theo vòng lặp (MP3)
        private void PlayBackgroundMusic(string soundName)
        {
            try
            {
                string filePath = soundEffects[soundName];  // Lấy đường dẫn từ tên âm thanh
                string fullPath = System.IO.Path.GetFullPath(filePath); 
                Console.WriteLine(filePath);
                Console.WriteLine($"Đường dẫn đầy đủ: {fullPath}");
                    
                player.URL = fullPath;
                player.controls.play();  // Phát nhạc nền (MP3)
                player.settings.setMode("loop", true);  // Bật chế độ lặp lại
                while (isPlayingBackgroundMusic)
                {
                    Thread.Sleep(100);  // Đảm bảo thread không chiếm quá nhiều tài nguyên CPU
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi phát nhạc nền: {ex.Message}");
            }
        }
    }
}
