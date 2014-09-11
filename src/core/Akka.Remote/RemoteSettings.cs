﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Akka.Actor;
using Akka.Configuration;

namespace Akka.Remote
{
    public class RemoteSettings
    {
        public RemoteSettings(Config config)
        {
            Config = config;
            LogReceive = config.GetBoolean("akka.remote.log-received-messages");
            LogSend = config.GetBoolean("akka.remote.log-sent-messages");

            var bufferSizeLogKey = "akka.remote.log-buffer-size-exceeding";
            if (config.GetString(bufferSizeLogKey).ToLowerInvariant().Equals("off") ||
                config.GetString(bufferSizeLogKey).ToLowerInvariant().Equals("false"))
            {
                LogBufferSizeExceeding = Int32.MaxValue;
            }
            else
            {
                LogBufferSizeExceeding = config.GetInt(bufferSizeLogKey);
            }

            UntrustedMode = config.GetBoolean("akka.remote.untrusted-mode");
            TrustedSelectionPaths = new HashSet<string>(config.GetStringList("akka.remote.trusted-selection-paths"));
            RemoteLifecycleEventsLogLevel = config.GetString("akka.remote.log-remote-lifecycle-events") ?? "DEBUG";
            Dispatcher = config.GetString("akka.remote.use-dispatcher");
            if (RemoteLifecycleEventsLogLevel.Equals("on")) RemoteLifecycleEventsLogLevel = "DEBUG";
            FlushWait = config.GetMillisDuration("akka.remote.flush-wait-on-shutdown");
            ShutdownTimeout = config.GetMillisDuration("akka.remote.shutdown-timeout");
            TransportNames = config.GetStringList("akka.remote.enabled-transports");
            Transports = (from transportName in TransportNames
                let transportConfig = TransportConfigFor(transportName)
                select new TransportSettings(transportConfig)).ToArray();
            Adapters = ConfigToMap(config.GetConfig("akka.remote.adapters"));
            BackoffPeriod = config.GetMillisDuration("akka.remote.backoff-interval");
            RetryGateClosedFor = config.GetMillisDuration("akka.remote.retry-gate-closed-for", TimeSpan.Zero);
            UsePassiveConnections = config.GetBoolean("akka.remote.use-passive-connections");
            SysMsgBufferSize = config.GetInt("akka.remote.system-message-buffer-size");
            SysResendTimeout = config.GetMillisDuration("akka.remote.resend-interval");
            InitialSysMsgDeliveryTimeout = config.GetMillisDuration("akka.remote.initial-system-message-delivery-timeout");
            SysMsgAckTimeout = config.GetMillisDuration("akka.remote.system-message-ack-piggyback-timeout");
            QuarantineDuration = config.GetMillisDuration("akka.remote.prune-quarantine-marker-after");
            StartupTimeout = config.GetMillisDuration("akka.remote.startup-timeout");
            CommandAckTimeout = config.GetMillisDuration("akka.remote.command-ack-timeout");

            WatchFailureDetectorConfig = config.GetConfig("akka.remote.watch-failure-detector");
            WatchFailureDetectorImplementationClass = WatchFailureDetectorConfig.GetString("implementation-class");
            WatchHeartBeatInterval = WatchFailureDetectorConfig.GetMillisDuration("heartbeat-interval");
            WatchUnreachableReaperInterval = WatchFailureDetectorConfig.GetMillisDuration("unreachable-nodes-reaper-interval");
            WatchHeartbeatExpectedResponseAfter = WatchFailureDetectorConfig.GetMillisDuration("expected-response-after");
        }

        /// <summary>
        /// Used for augmenting outbound messages with the Akka scheme
        /// </summary>
        public static readonly string AkkaScheme = "akka";

        public Config Config { get; private set; }

        public HashSet<string> TrustedSelectionPaths { get; set; }

        public bool UntrustedMode { get; set; }

        public bool LogSend { get; set; }

        public bool LogReceive { get; set; }

        public int LogBufferSizeExceeding { get; set; }

        public string RemoteLifecycleEventsLogLevel { get; set; }

        public string Dispatcher { get; set; }

        public TimeSpan ShutdownTimeout { get; set; }

        public TimeSpan FlushWait { get; set; }

        public IList<string> TransportNames { get; set; }

        public IDictionary<string, string> Adapters { get; set; }

        public TransportSettings[] Transports { get; set; }
        public TimeSpan BackoffPeriod { get; set; }
        public TimeSpan RetryGateClosedFor { get; set; }
        public bool UsePassiveConnections { get; set; }
        public int SysMsgBufferSize { get; set; }
        public TimeSpan SysResendTimeout { get; set; }
        public TimeSpan InitialSysMsgDeliveryTimeout { get; set; }
        public TimeSpan SysMsgAckTimeout { get; set; }
        public TimeSpan? QuarantineDuration { get; set; }
        public TimeSpan StartupTimeout { get; set; }
        public TimeSpan CommandAckTimeout { get; set; }

        public Config WatchFailureDetectorConfig { get; set; }
        public string WatchFailureDetectorImplementationClass { get; set; }
        public TimeSpan WatchHeartBeatInterval { get; set; }
        public TimeSpan WatchUnreachableReaperInterval { get; set; }
        public TimeSpan WatchHeartbeatExpectedResponseAfter { get; set; }

        private Config TransportConfigFor(string transportName)
        {
            return Config.GetConfig(transportName);
        }

        public Props ConfigureDispatcher(Props props)
        {
            return String.IsNullOrEmpty(Dispatcher) ? props : props.WithDispatcher(Dispatcher);
        }

        public class TransportSettings
        {
            public TransportSettings(Config config)
            {
                TransportClass = config.GetString("transport-class");
                Adapters = config.GetStringList("applied-adapters").Reverse().ToList();
                Config = config;
            }

            public Config Config { get; set; }

            public IList<string> Adapters { get; set; }

            public string TransportClass { get; set; }
        }

        private static IDictionary<string, string> ConfigToMap(Config cfg)
        {
            if(cfg.IsEmpty) return new Dictionary<string, string>();
            return cfg.Root.GetObject().Unwrapped.ToDictionary(k => k.Key, v => v.Value != null? v.Value.ToString():null);
        }
    }
}