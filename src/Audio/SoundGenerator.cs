namespace Tetris.Audio;

/// <summary>
/// Synthesizes high-fidelity 16-bit 44.1kHz PCM sound effects and retro chiptune
/// music tracks in memory, eliminating any external audio asset dependencies.
/// </summary>
public static class SoundGenerator
{
    private const int SampleRate = 44100;

    public static byte[] GenerateSound(SoundEffectType type)
    {
        return type switch
        {
            SoundEffectType.Move => GenerateTone(440, 0.04f, WaveType.Square, 0.4f, 0.005f, 0.035f),
            SoundEffectType.Rotate => GenerateFrequencySweep(520, 780, 0.06f, WaveType.Triangle, 0.5f),
            SoundEffectType.SoftDrop => GenerateTone(220, 0.03f, WaveType.Sine, 0.3f, 0.005f, 0.025f),
            SoundEffectType.HardDrop => GenerateHardDropSound(),
            SoundEffectType.Lock => GenerateTone(320, 0.05f, WaveType.Square, 0.5f, 0.005f, 0.045f),
            SoundEffectType.Hold => GenerateChord(new[] { 587.33f, 880f }, 0.10f, WaveType.Sine, 0.5f),
            SoundEffectType.LineClear1 => GenerateChord(new[] { 523.25f, 659.25f }, 0.18f, WaveType.Triangle, 0.6f),
            SoundEffectType.LineClear2 => GenerateChord(new[] { 523.25f, 659.25f, 783.99f }, 0.25f, WaveType.Triangle, 0.6f),
            SoundEffectType.LineClear3 => GenerateChord(new[] { 523.25f, 659.25f, 783.99f, 1046.50f }, 0.35f, WaveType.Triangle, 0.7f),
            SoundEffectType.TetrisClear => GenerateTetrisFanfare(),
            SoundEffectType.TSpin => GenerateFrequencySweep(350, 950, 0.25f, WaveType.Sine, 0.7f),
            SoundEffectType.LevelUp => GenerateLevelUpSound(),
            SoundEffectType.GameOver => GenerateGameOverSound(),
            SoundEffectType.ButtonHover => GenerateTone(880, 0.02f, WaveType.Sine, 0.25f, 0.002f, 0.018f),
            SoundEffectType.ButtonClick => GenerateTone(659.25f, 0.04f, WaveType.Triangle, 0.4f, 0.003f, 0.037f),
            SoundEffectType.ModeComplete => GenerateVictoryFanfare(),
            _ => GenerateTone(440, 0.05f, WaveType.Sine, 0.3f, 0.01f, 0.04f)
        };
    }

    /// <summary>
    /// Synthesizes an 8-bit / synthwave melodic theme loop inspired by classical themes.
    /// </summary>
    public static byte[] GenerateBackgroundMusicLoop()
    {
        // 16-step iconic bass & melody arpeggio in A minor / E major
        // Notes: E5, B4, C5, D5, C5, B4, A4, A4, C5, E5, D5, C5, B4, C5, D5, E5
        float[] melodyNotes = {
            659.25f, 493.88f, 523.25f, 587.33f, 523.25f, 493.88f, 440.00f, 440.00f,
            523.25f, 659.25f, 587.33f, 523.25f, 493.88f, 523.25f, 587.33f, 659.25f,
            523.25f, 440.00f, 440.00f, 0f,      587.33f, 698.46f, 880.00f, 783.99f,
            698.46f, 659.25f, 523.25f, 659.25f, 587.33f, 523.25f, 493.88f, 493.88f
        };

        float[] bassNotes = {
            164.81f, 164.81f, 220.00f, 220.00f, 164.81f, 164.81f, 110.00f, 110.00f,
            220.00f, 220.00f, 146.83f, 146.83f, 164.81f, 164.81f, 164.81f, 164.81f,
            220.00f, 220.00f, 110.00f, 110.00f, 146.83f, 146.83f, 220.00f, 220.00f,
            174.61f, 174.61f, 164.81f, 164.81f, 146.83f, 146.83f, 164.81f, 164.81f
        };

        float noteDuration = 0.22f; // ~136 BPM
        int totalNotes = melodyNotes.Length;
        int totalSamples = (int)(SampleRate * noteDuration * totalNotes);

        short[] pcm = new short[totalSamples];

        for (int noteIdx = 0; noteIdx < totalNotes; noteIdx++)
        {
            float mFreq = melodyNotes[noteIdx];
            float bFreq = bassNotes[noteIdx];
            int startSample = (int)(noteIdx * noteDuration * SampleRate);
            int noteSamples = (int)(noteDuration * SampleRate);

            for (int i = 0; i < noteSamples; i++)
            {
                int sampleIdx = startSample + i;
                if (sampleIdx >= totalSamples) break;

                double t = (double)i / SampleRate;
                double frac = (double)i / noteSamples;
                double env = Math.Exp(-frac * 3.5); // Fast natural pluck decay

                double melVal = 0;
                if (mFreq > 20)
                {
                    // Square wave with slight duty cycle + triangle overtone
                    double phase = (t * mFreq) % 1.0;
                    melVal = (phase < 0.4 ? 1.0 : -1.0) * 0.45 + (Math.Sin(2 * Math.PI * mFreq * t) * 0.35);
                }

                // Bassline: warm triangle + sub-sine
                double bPhase = (t * bFreq) % 1.0;
                double bassVal = (Math.Abs(bPhase * 4 - 2) - 1) * 0.55 + (Math.Sin(2 * Math.PI * bFreq * t) * 0.35);

                double combined = (melVal * 0.38 + bassVal * 0.42) * env;
                pcm[sampleIdx] = (short)Math.Clamp(combined * 16000, -32000, 32000);
            }
        }

        return CreateWavHeader(pcm);
    }

    private static byte[] GenerateHardDropSound()
    {
        int samples = (int)(SampleRate * 0.14f);
        short[] pcm = new short[samples];
        for (int i = 0; i < samples; i++)
        {
            double t = (double)i / SampleRate;
            double frac = (double)i / samples;
            // Sweep from 150 Hz down to 35 Hz
            double freq = 150.0 * (1.0 - frac * 0.75);
            double envelope = Math.Pow(1.0 - frac, 2.0);
            double tone = Math.Sin(2 * Math.PI * freq * t);
            // Add subtle punch distortion
            tone = Math.Tanh(tone * 1.8);
            pcm[i] = (short)(tone * envelope * 24000);
        }
        return CreateWavHeader(pcm);
    }

    private static byte[] GenerateTetrisFanfare()
    {
        float[] notes = { 523.25f, 659.25f, 783.99f, 1046.50f };
        float noteDur = 0.10f;
        int samples = (int)(SampleRate * noteDur * notes.Length);
        short[] pcm = new short[samples];

        for (int n = 0; n < notes.Length; n++)
        {
            float freq = notes[n];
            int start = (int)(n * noteDur * SampleRate);
            int count = (int)(noteDur * SampleRate);

            for (int i = 0; i < count; i++)
            {
                int idx = start + i;
                if (idx >= samples) break;
                double t = (double)i / SampleRate;
                double frac = (double)i / count;
                double env = 1.0 - (frac * 0.6);
                double s = (Math.Sin(2 * Math.PI * freq * t) * 0.6 + Math.Sin(4 * Math.PI * freq * t) * 0.3) * env;
                pcm[idx] = (short)(s * 22000);
            }
        }
        return CreateWavHeader(pcm);
    }

    private static byte[] GenerateLevelUpSound()
    {
        float[] notes = { 440f, 554.37f, 659.25f, 880f };
        float dur = 0.08f;
        int samples = (int)(SampleRate * dur * notes.Length);
        short[] pcm = new short[samples];

        for (int n = 0; n < notes.Length; n++)
        {
            float freq = notes[n];
            int start = (int)(n * dur * SampleRate);
            int count = (int)(dur * SampleRate);
            for (int i = 0; i < count; i++)
            {
                int idx = start + i;
                if (idx >= samples) break;
                double t = (double)i / SampleRate;
                double env = 1.0 - ((double)i / count * 0.5);
                double phase = (t * freq) % 1.0;
                double s = (Math.Abs(phase * 4 - 2) - 1) * env;
                pcm[idx] = (short)(s * 20000);
            }
        }
        return CreateWavHeader(pcm);
    }

    private static byte[] GenerateGameOverSound()
    {
        float[] notes = { 311.13f, 261.63f, 196.00f, 130.81f }; // Eb4 -> C4 -> G3 -> C3
        float dur = 0.22f;
        int samples = (int)(SampleRate * dur * notes.Length);
        short[] pcm = new short[samples];

        for (int n = 0; n < notes.Length; n++)
        {
            float freq = notes[n];
            int start = (int)(n * dur * SampleRate);
            int count = (int)(dur * SampleRate);
            for (int i = 0; i < count; i++)
            {
                int idx = start + i;
                if (idx >= samples) break;
                double t = (double)i / SampleRate;
                double frac = (double)i / count;
                double env = Math.Pow(1.0 - frac, 1.5);
                double s = Math.Sin(2 * Math.PI * freq * t) * env;
                pcm[idx] = (short)(s * 18000);
            }
        }
        return CreateWavHeader(pcm);
    }

    private static byte[] GenerateVictoryFanfare()
    {
        float[] notes = { 440f, 554.37f, 659.25f, 880f, 1108.73f };
        float dur = 0.12f;
        int samples = (int)(SampleRate * dur * notes.Length);
        short[] pcm = new short[samples];

        for (int n = 0; n < notes.Length; n++)
        {
            float freq = notes[n];
            int start = (int)(n * dur * SampleRate);
            int count = (int)(dur * SampleRate);
            for (int i = 0; i < count; i++)
            {
                int idx = start + i;
                if (idx >= samples) break;
                double t = (double)i / SampleRate;
                double env = 1.0 - ((double)i / count * 0.4);
                double s = Math.Sin(2 * Math.PI * freq * t) * env;
                pcm[idx] = (short)(s * 20000);
            }
        }
        return CreateWavHeader(pcm);
    }

    private static byte[] GenerateTone(float frequency, float durationSec, WaveType waveType, float amplitude, float attackSec, float releaseSec)
    {
        int samples = (int)(SampleRate * durationSec);
        short[] pcm = new short[samples];

        int attackSamples = (int)(SampleRate * attackSec);
        int releaseSamples = (int)(SampleRate * releaseSec);

        for (int i = 0; i < samples; i++)
        {
            double t = (double)i / SampleRate;
            double envelope = 1.0;
            if (i < attackSamples) envelope = (double)i / attackSamples;
            else if (i >= samples - releaseSamples) envelope = (double)(samples - i) / releaseSamples;

            double val = SampleWave(waveType, frequency, t);
            pcm[i] = (short)(val * envelope * amplitude * 26000);
        }

        return CreateWavHeader(pcm);
    }

    private static byte[] GenerateFrequencySweep(float startFreq, float endFreq, float durationSec, WaveType waveType, float amplitude)
    {
        int samples = (int)(SampleRate * durationSec);
        short[] pcm = new short[samples];

        double phase = 0;
        for (int i = 0; i < samples; i++)
        {
            double frac = (double)i / samples;
            double curFreq = startFreq + (endFreq - startFreq) * frac;
            phase += 2 * Math.PI * curFreq / SampleRate;

            double envelope = 1.0 - frac;
            double val = waveType switch
            {
                WaveType.Triangle => Math.Abs(((phase / (2 * Math.PI)) % 1.0) * 4 - 2) - 1,
                WaveType.Square => ((phase % (2 * Math.PI)) < Math.PI ? 1.0 : -1.0),
                _ => Math.Sin(phase)
            };

            pcm[i] = (short)(val * envelope * amplitude * 26000);
        }

        return CreateWavHeader(pcm);
    }

    private static byte[] GenerateChord(float[] frequencies, float durationSec, WaveType waveType, float amplitude)
    {
        int samples = (int)(SampleRate * durationSec);
        short[] pcm = new short[samples];

        for (int i = 0; i < samples; i++)
        {
            double t = (double)i / SampleRate;
            double frac = (double)i / samples;
            double envelope = Math.Pow(1.0 - frac, 1.2);

            double sum = 0;
            foreach (var f in frequencies)
            {
                sum += SampleWave(waveType, f, t);
            }
            sum /= frequencies.Length;

            pcm[i] = (short)(sum * envelope * amplitude * 26000);
        }

        return CreateWavHeader(pcm);
    }

    private static double SampleWave(WaveType waveType, float freq, double t)
    {
        return waveType switch
        {
            WaveType.Square => ((t * freq) % 1.0 < 0.5 ? 1.0 : -1.0),
            WaveType.Triangle => Math.Abs(((t * freq) % 1.0) * 4 - 2) - 1,
            _ => Math.Sin(2 * Math.PI * freq * t)
        };
    }

    private static byte[] CreateWavHeader(short[] pcmSamples)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        int byteCount = pcmSamples.Length * 2;
        // RIFF header
        bw.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
        bw.Write(36 + byteCount);
        bw.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
        // fmt chunk
        bw.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
        bw.Write(16);
        bw.Write((short)1); // PCM
        bw.Write((short)1); // Mono
        bw.Write(SampleRate);
        bw.Write(SampleRate * 2);
        bw.Write((short)2);
        bw.Write((short)16);
        // data chunk
        bw.Write(System.Text.Encoding.ASCII.GetBytes("data"));
        bw.Write(byteCount);

        for (int i = 0; i < pcmSamples.Length; i++)
        {
            bw.Write(pcmSamples[i]);
        }

        return ms.ToArray();
    }

    private enum WaveType
    {
        Sine,
        Square,
        Triangle
    }
}

