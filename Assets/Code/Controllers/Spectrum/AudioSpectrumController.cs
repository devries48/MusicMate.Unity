using UnityEngine;

// https://www.youtube.com/watch?v=mHk3ZiKNH48
public class AudioSpectrumController : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;

    readonly float[] _samples = new float[512];
    readonly float[] _freqBand = new float[8];
    readonly float[] _bandBuffer = new float[8];
    readonly float[] _bufferDecrease = new float[8];
    readonly float[] _freqBandHighest = new float[8];

    public static float[] m_AudioBand = new float[8];
    public static float[] m_AudioBandBuffer = new float[8];

    void Update()
    {
        if (!_audioSource.isPlaying) return;

        GetSpectrumData();
        CreateFrequencyBands();
        CreateBandBuffer();
        CreateAudioBands();
    }

    void GetSpectrumData() => _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);

    /// <summary>
    /// Create 8 frequency bands from the averages of the 512 samples.
    /// 43 hertz per sample (22050/512)
    /// 0: 2 samples = 86 hertz
    /// 1: 4 samples = 172 hertz      87-258
    /// 2: 8 samples = 344 hertz      259-602
    /// 3: 16 samples = 688 hertz     603-1290
    /// 4: 32 samples = 1376 hertz    1291-2666
    /// 5: 64 samples = 2752 hertz    2667-5418
    /// 6: 128 samples = 5504 hertz   5419-10922
    /// 7: 256 samples = 11008 hertz  10923-21930
    /// Total 510 samples -> Last 2 samples are added to the last bar.
    /// </summary>
    void CreateFrequencyBands()
    {
        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            int sampleCount = (int)Mathf.Pow(2, i) * 2;
            float average = 0;

            if (i == 7) sampleCount += 2;

            for (int j = 0; j < sampleCount; j++)
            {
                average += _samples[count] * (count + 1);
                count++;
            }

            average /= count;
            _freqBand[i] = average * 10;
        }
    }

    void CreateBandBuffer()
    {
        for (int i = 0; i < 8; i++)
        {
            if (_freqBand[i] > _bandBuffer[i])
            {
                _bandBuffer[i] = _freqBand[i];
                _bufferDecrease[i] = 0.0005f;
            }

            if (_freqBand[i] < _bandBuffer[i])
            {
                _bandBuffer[i] -= _bufferDecrease[i];
                _bufferDecrease[i] *= 1.2f;
            }
        }
    }

    void CreateAudioBands()
    {
        bool isMax = true;

        for (int i = 0; i < 8; i++)
        {
            if (_freqBand[i] > _freqBandHighest[i])
                _freqBandHighest[i] = _freqBand[i];

            if (_freqBandHighest[i] != 0)
            {
                m_AudioBand[i] = _freqBand[i] / _freqBandHighest[i];
                m_AudioBandBuffer[i] = _bandBuffer[i] / _freqBandHighest[i];
            }
            if (m_AudioBand[i] != 1f)
                isMax = false;
        }

        if (isMax)
        {
            for (int i = 0; i < 8; i++)
            {
                m_AudioBand[i] = 0;
                m_AudioBandBuffer[i] = 0;
            }
        }

    }
}
