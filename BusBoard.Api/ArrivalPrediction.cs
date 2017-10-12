using System;
public class ArrivalPrediction
{
    public string Id { get; set; }
    public string StationName { get; set; }
    public int TimeToStation { get; set; }

    public string GetArrivalTime()
    {
        TimeSpan time = TimeSpan.FromSeconds(TimeToStation);
        String stringTime = time.ToString(@"\:mm\:ss");
        return stringTime;
    }
}


