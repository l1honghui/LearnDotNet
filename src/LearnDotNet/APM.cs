
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;

namespace Common
{
    class APM
    {
        public static void PrintSystemInfo()
        {
            var proc = Process.GetCurrentProcess();
            var mem = proc.WorkingSet64;
            var cpu = proc.TotalProcessorTime;

            //foreach (var aProc in Process.GetProcesses())
            //    Console.WriteLine("Proc {0,30}  CPU {1,-20:n} msec", aProc.ProcessName, cpu.TotalMilliseconds/Environment.ProcessorCount);

            Console.WriteLine("My process {0} used working set {1:n3} Mb of working set and CPU {2:n} msec",
                proc.ProcessName, mem / 1024.0 / 1024.0,
                cpu.TotalMilliseconds / Environment.ProcessorCount);
        }
    }

    public class PerformanceCounterListener : EventListener
    {
        private static HashSet<string> _keys = new HashSet<string>
        {
            "Count",
            "Min",
            "Max",
            "Mean",
            "Increment",
            "DisplayUnits"
        };

        private static DateTimeOffset? _lastSampleTime;

        private Action<string> _writeLogAction;

        public PerformanceCounterListener(Action<string> writeLogAction)
        {
            _writeLogAction = writeLogAction;
        }

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            base.OnEventSourceCreated(eventSource);
            if (eventSource.Name == "System.Runtime")
            {
                EnableEvents(eventSource, EventLevel.Critical, EventKeywords.All, new Dictionary<string, string>
                {
                    ["EventCounterIntervalSec"] = "60"
                });
            }
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (_lastSampleTime.HasValue && DateTimeOffset.UtcNow - _lastSampleTime.Value > TimeSpan.FromSeconds(1.0))
            {
                Console.WriteLine();
            }

            _lastSampleTime = DateTimeOffset.UtcNow;
            IDictionary<string, object> dictionary = (IDictionary<string, object>)eventData.Payload![0];
            object arg = dictionary["Name"];
            IEnumerable<string> source = from it in dictionary
                                         where _keys.Contains(it.Key)
                                         select $"{it.Key} = {it.Value}";
            string text = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd hh:mm::ss");
            string obj = string.Format("{0,-32}: {1}", arg, string.Join("; ", source.ToArray()));
            _writeLogAction(obj);
        }
    }

}

