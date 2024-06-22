namespace Geosite
{
    public class LoadingBar
    {
        public int Count;

        private readonly ProgressBar _bar;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bar">ProgressBar 类型对象</param>
        public LoadingBar(ProgressBar bar)
        {
            _bar = bar;
            _bar.BeginInvoke(() =>
                {
                    _bar.MarqueeAnimationSpeed = 0;
                    _bar.Refresh();
                }
            );
        }

        /// <summary>
        /// 开启或关闭等待效果
        /// </summary>
        /// <param name="onOff">true（默认）=开启；false=仅关闭前次开启；null=彻底关闭</param>
        public void Run(bool? onOff = true)
        {
            if (onOff == true)
            {
                Count++;
                if (Count == 1)
                    try
                    {
                        _bar.BeginInvoke(() =>
                            {
                                _bar.MarqueeAnimationSpeed = 1;
                                _bar.Refresh();
                            }
                        );
                    }
                    catch
                    {
                        //
                    }
            }
            else
            {
                if (onOff == false)
                {
                    Count--;
                    if (Count < 0)
                        Count = 0;
                }
                else
                    Count = 0;
                try
                {
                    if (Count == 0)
                        _bar.BeginInvoke(() =>
                            {
                                _bar.MarqueeAnimationSpeed = 0;
                                _bar.Refresh();
                            }
                        );
                }
                catch
                {
                    //
                }
            }
        }
    }
}
