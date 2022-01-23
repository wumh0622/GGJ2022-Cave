using UnityEngine;

namespace FreqCon
{
    public class FreqControl : MonoBehaviour
    {
        protected Transform m_FreqObj;

        public float mLimitAberrationShaker = 0;
        public float mLimitDistortionShaker = 0;
        //屬性
        public int m_band = 0; //屬於哪裡
        private AudioPeer mAudioPeer = null;

        public void SetAudioPeer(AudioPeer iAudioPeer)
        {
            mAudioPeer = iAudioPeer;
        }

        #region 初始化
        public virtual void SetValue(int _band)
        {
            m_band = 0;
            SwitchThis(true);
            if (0 <= _band && _band < 512)
                m_band = _band;
        }
        #endregion

        private void OnDestroy()
        {
            SwitchThis(false);
        }

        #region 加入控制區開關
        protected virtual void SwitchThis(bool _open)
        {
            if (_open)
                AudioPeer.AddFreqCon(this);
            else
                AudioPeer.RemoveFreqCon(this);
        }
        #endregion

        #region 設定值
        public virtual void SetFreqValue(float _value)
        {
            if (mLimitAberrationShaker != 0 && _value >= mLimitAberrationShaker)
            {
                mAudioPeer.AberrationShaker();
            }
            else if (mLimitDistortionShaker != 0 && _value >= mLimitDistortionShaker)
            {
                mAudioPeer.DistortionShaker();
            }
        }
        #endregion
    }
}