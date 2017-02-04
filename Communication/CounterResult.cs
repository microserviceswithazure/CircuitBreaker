namespace Communication
{
    using System;

    public class CounterResult
    {
        public int Value { get; set; }
        public DateTime ReportTime { get; set; }
        public string CircuitState { get; set; }
        public double ResponseTimeInSeconds { get; set; }
    }
}