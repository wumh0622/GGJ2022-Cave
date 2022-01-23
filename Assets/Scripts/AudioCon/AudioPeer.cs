using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FreqCon
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPeer : MonoBehaviour
    {
        [Header("平均8")]
        [SerializeField] FreqControl[] m_AverageFreqs = new FreqControl[8];
        public MoreMountains.FeedbacksForThirdParty.MMChromaticAberrationShaker mAberrationShaker = null;
        public MoreMountains.FeedbacksForThirdParty.MMLensDistortionShaker mDistortionShaker = null;

        public float mAberrationShakerDelay = 0;
        public float mDistortionShakerDelay = 0;
        private float mCurAberrationShakerDelay = 0;
        private float mCurDistortionShakerDelay = 0;

        public static List<FreqControl> m_ConFreqValue = new List<FreqControl>();
        private FreqControl CacheFreqCon = null;
        private static int freqMax;

        private AudioSource myAudioSource;
        private float[] samples = new float[512];

        public FFTWindow m_FFT;

        //平均頻率方式
        private float[] freqBand = new float[8];
        private float[] bandBuffer = new float[8];
        private float[] bufferDecrease = new float[8];

        public float freqBandHigh = 0;
        private float[] freqBandHighest = new float[8];
        private float[] audioBand = new float[8];
        public float[] audioBandBuffer = new float[8];
        //振福        
        private float m_Amplitude = 0, m_AmplitudeBuffer = 0, m_AmplitudeHighest = 0.001f;
        private float cacheAmplitude, cacheAmplitudeBuffer;
        
        private int cacheIndex;

        private void Awake()
        {
            BornAverageFreqs();
        }
        private void Start()
        {
            myAudioSource = GetComponent<AudioSource>();
            for (int i = 0; i < 8; i++)
            {
                freqBandHighest[i] = freqBandHigh;
            }
        }
        private void Update()
        {
            GetSpectrumAudioSource();
            Hertz();
            Action();
            if (mCurAberrationShakerDelay > 0)
            {
                mCurAberrationShakerDelay -= Time.deltaTime;
            }
            if (mCurDistortionShakerDelay > 0)
            {
                mCurDistortionShakerDelay -= Time.deltaTime;
            }
        }

        #region 新增和移除  頻率控制
        public static void AddFreqCon(FreqControl _freqCon)
        {
            m_ConFreqValue.Add(_freqCon);
            freqMax = m_ConFreqValue.Count;
        }
        public static void RemoveFreqCon(FreqControl _freqCon)
        {
            if (m_ConFreqValue.Contains(_freqCon))
                m_ConFreqValue.Remove(_freqCon);
            freqMax = m_ConFreqValue.Count;
        }
        #endregion

        void Action()
        {
            for (int i = 0; i < freqMax; i++)
            {
                CacheFreqCon = m_ConFreqValue[i];
                cacheIndex = CacheFreqCon.m_band;

                CacheFreqCon.SetFreqValue(samples[cacheIndex]);
                if (cacheIndex >= 8)
                    cacheIndex = Mathf.RoundToInt(cacheIndex * 0.0136986f);
            }
        }

        #region 頻率
        /*  220 / 512 = 43
            20-60
            60-250
            250-500
            500-2000
            2000-4000
            4000-6000
            6000-20000

           0-2 = 86
           1-4 = 172
           2-8 = 344
           3-16 = 688
           4-32 = 1376
           5-64 = 2752
           6-128 = 5504  (以上全部510)
           7-256 = 11008*/
        void Hertz()
        {
            int count = 0;
            cacheAmplitude = 0;
            cacheAmplitudeBuffer = 0;
            for (int pow = 0; pow < 8; pow++)//次方
            {
                float average = 0;
                int smapleCount = (int)Mathf.Pow(2, pow) * 2;
                if (pow == 7)
                    smapleCount += 2;
                for (int num = 0; num < smapleCount; num++) //加幾次
                {
                    average += samples[count] * (count + 1);
                    count++;
                }
                average /= count;
                freqBand[pow] = average * 10;

                //平緩區
                if (freqBand[pow] > bandBuffer[pow])
                {
                    bandBuffer[pow] = freqBand[pow];
                    bufferDecrease[pow] = 0.005f;
                }
                else if (freqBand[pow] < bandBuffer[pow])
                {
                    bandBuffer[pow] -= bufferDecrease[pow];
                    bufferDecrease[pow] *= 1.2f;
                }

                if (freqBand[pow] > freqBandHighest[pow])
                    freqBandHighest[pow] = freqBand[pow];

                audioBand[pow] = (freqBand[pow] / freqBandHighest[pow]);
                audioBandBuffer[pow] = (bandBuffer[pow] / freqBandHighest[pow]);

                //振幅
                cacheAmplitude += audioBand[pow];
                cacheAmplitudeBuffer += audioBandBuffer[pow];
            }
            if (cacheAmplitude > m_AmplitudeHighest)
                m_AmplitudeHighest = cacheAmplitude;

            m_Amplitude = cacheAmplitude / m_AmplitudeHighest;
            m_AmplitudeBuffer = cacheAmplitudeBuffer / m_AmplitudeHighest;
        }
        #endregion

        void BornAverageFreqs()
        {
            for (int i = 0; i < m_AverageFreqs.Length; i++)
            {
                m_AverageFreqs[i].SetValue(i);
                m_AverageFreqs[i].SetAudioPeer(this);
            }
        }

        void GetSpectrumAudioSource()
        {
            myAudioSource.GetSpectrumData(samples, 0, m_FFT);
        }

        public void AberrationShaker()
        {
            if (mCurAberrationShakerDelay <= 0)
            {
                mAberrationShaker.enabled = true;
                mCurAberrationShakerDelay = mAberrationShakerDelay;
            }
        }
        public void DistortionShaker()
        {
            if (mCurDistortionShakerDelay <= 0)
            {
                mDistortionShaker.enabled = true;
                mCurDistortionShakerDelay = mDistortionShakerDelay;
            }
        }
    }
}