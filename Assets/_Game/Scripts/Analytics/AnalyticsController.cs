using System.Collections.Generic;
using _Game.Scripts.Analytics.Events;
using _Game.Scripts.DI;
using GeneralUtils.Processes;

namespace _Game.Scripts.Analytics {
    public class AnalyticsController : IAnalyticsLogger {
        private readonly IEnumerable<IAnalyticsService> _services;
        private readonly List<IServiceLogger> _loggers = new List<IServiceLogger>();

        [Inject]
        public AnalyticsController(IEnumerable<IAnalyticsService> services) {
            _services = services;
        }

        public Process Init() {
            var initProcess = new ParallelProcess();
            foreach (var service in _services) {
                var serviceInitProcess = new SerialProcess();
                serviceInitProcess.Add(service.Init());
                serviceInitProcess.Add(new SyncProcess(() => _loggers.Add(service.CreateLogger())));
                initProcess.Add(serviceInitProcess);
            }

            return initProcess;
        }

        public void Log(AnalyticsEvent analyticsEvent) {
            foreach (var logger in _loggers) {
                logger.Log(analyticsEvent);
            }
        }
    }
}