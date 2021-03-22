namespace script
{
    public class TimeUtil
    {
        public static string TimeFormatInHrsMinSec(int timeLeft)
        {
            GetTimeDetails(timeLeft, out var day, out var hour, out var min, out var sec);
            return $"{hour.ToString():00}:{min.ToString():00}:{sec.ToString():00}";
        }

        public static void GetTimeDetails(int time, out int day, out int hour, out int min, out int sec)
        {
            min = time / 60; //总分钟数
            hour = min / 60; //总小时数
            day = hour / 24; //总天数

            sec = time % 60; //剩余秒数
            min %= 60; //剩余分钟数
            hour %= 24; //剩余小时数
        }

        public static int GetTimeSec(int hours)
        {
            return hours * 3600;
        }
    }
}