namespace TelegramPoster.Application.Services.Shared;

public static class TimePostingSharedService
{
    public static List<DateTime> GetTimeForPosting(int mediaCount,
                                            Dictionary<DayOfWeek, List<TimeOnly>> scheduleMedia,
                                            List<DateTime> allTimingMessages)
    {
        var currentDateValue = DateTime.Now;

        var currentDayOfWeek = currentDateValue.DayOfWeek;
        var currentTime = currentDateValue.TimeOfDay;

        var dateTimes = new List<DateTime>();
        int index = 0;

        while (index < mediaCount)
        {
            if (scheduleMedia.TryGetValue(currentDayOfWeek, out List<TimeOnly>? timesForToday))
            {
                timesForToday.Sort();
                foreach (var time in timesForToday)
                {
                    var timeSpan = time.ToTimeSpan();
                    var potentialNewDateTime = currentDateValue.Date + timeSpan;

                    if (timeSpan > currentTime && !allTimingMessages.Contains(potentialNewDateTime))
                    {
                        dateTimes.Add(potentialNewDateTime);
                        index++;
                        if (index >= mediaCount)
                            break;
                    }
                }
            }

            currentDateValue = currentDateValue.AddDays(1);
            currentDayOfWeek = currentDateValue.DayOfWeek;
            currentTime = TimeSpan.Zero;
        }

        return dateTimes;
    }
}
