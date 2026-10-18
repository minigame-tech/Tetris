using Raylib_cs;
using Tetris.Core;

namespace Tetris.Audio;

/// <summary>
/// Controls audio device lifecycle, sound effects playback, and procedural background music.
/// Gracefully degrades if host audio devices are unavailable.
/// </summary>
public sealed class AudioManager : IDisposable
{
    private readonly GameSettings _settings;
    private readonly Dictionary<SoundEffectType, Sound> _sounds = new();
    private Sound? _bgMusic;
    private bool _isAudioAvailable = false;
    private bool _isDisposed = false;

    public AudioManager(GameSettings settings)
    {
        _settings = settings;
        InitializeAudio();
    }

    private void InitializeAudio()
    {
        try
        {
            Raylib.InitAudioDevice();
            _isAudioAvailable = Raylib.IsAudioDeviceReady();
        }
        catch
        {
            _isAudioAvailable = false;
        }

        if (!_isAudioAvailable) return;

        // Generate procedural sounds
        foreach (SoundEffectType type in Enum.GetValues<SoundEffectType>())
        {
            try
            {
                var wavData = SoundGenerator.GenerateSound(type);
                var wave = Raylib.LoadWaveFromMemory(".wav", wavData);
                if (Raylib.IsWaveValid(wave))
                {
                    var sound = Raylib.LoadSoundFromWave(wave);
                    Raylib.UnloadWave(wave);
                    if (Raylib.IsSoundValid(sound))
                    {
                        _sounds[type] = sound;
                    }
                }
            }
            catch
            {
                // Silently skip any specific sound generation error
            }
        }

        // Generate looping background music
        try
        {
            var musicWav = SoundGenerator.GenerateBackgroundMusicLoop();
            var musicWave = Raylib.LoadWaveFromMemory(".wav", musicWav);
            if (Raylib.IsWaveValid(musicWave))
            {
                var musicSound = Raylib.LoadSoundFromWave(musicWave);
                Raylib.UnloadWave(musicWave);
                if (Raylib.IsSoundValid(musicSound))
                {
                    _bgMusic = musicSound;
                }
            }
        }
        catch
        {
            // Silently recover if music cannot be generated
        }

        ApplyVolumes();
    }

    public void Update()
    {
        if (!_isAudioAvailable || _settings.IsMuted || !_bgMusic.HasValue) return;

        // Keep background music looping smoothly
        if (_settings.MusicVolume > 0.01f && !Raylib.IsSoundPlaying(_bgMusic.Value))
        {
            Raylib.PlaySound(_bgMusic.Value);
        }
    }

    public void PlaySound(SoundEffectType type)
    {
        if (!_isAudioAvailable || _settings.IsMuted || _settings.SfxVolume <= 0.01f) return;

        if (_sounds.TryGetValue(type, out var sound))
        {
            float volume = _settings.MasterVolume * _settings.SfxVolume;
            Raylib.SetSoundVolume(sound, volume);
            Raylib.PlaySound(sound);
        }
    }

    public void ApplyVolumes()
    {
        if (!_isAudioAvailable) return;

        float effectiveSfxVol = _settings.IsMuted ? 0f : _settings.MasterVolume * _settings.SfxVolume;
        float effectiveMusicVol = _settings.IsMuted ? 0f : _settings.MasterVolume * _settings.MusicVolume;

        foreach (var sound in _sounds.Values)
        {
            Raylib.SetSoundVolume(sound, effectiveSfxVol);
        }

        if (_bgMusic.HasValue)
        {
            Raylib.SetSoundVolume(_bgMusic.Value, effectiveMusicVol);
            if (effectiveMusicVol <= 0.01f && Raylib.IsSoundPlaying(_bgMusic.Value))
            {
                Raylib.StopSound(_bgMusic.Value);
            }
        }
    }

    public void ToggleMute()
    {
        _settings.IsMuted = !_settings.IsMuted;
        ApplyVolumes();
        _settings.Save();
    }

    public void StopMusic()
    {
        if (_isAudioAvailable && _bgMusic.HasValue && Raylib.IsSoundPlaying(_bgMusic.Value))
        {
            Raylib.StopSound(_bgMusic.Value);
        }
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        if (_isAudioAvailable)
        {
            StopMusic();

            foreach (var sound in _sounds.Values)
            {
                Raylib.UnloadSound(sound);
            }
            _sounds.Clear();

            if (_bgMusic.HasValue)
            {
                Raylib.UnloadSound(_bgMusic.Value);
                _bgMusic = null;
            }

            Raylib.CloseAudioDevice();
        }
    }
}

