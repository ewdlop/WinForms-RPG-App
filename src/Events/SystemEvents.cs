using System;
using System.Collections.Generic;

namespace WinFormsApp1.Events
{
    /// <summary>
    /// Event for requesting data synchronization between managers
    /// </summary>
    public class DataSyncRequestEvent : GameEvent
    {
        public string SourceManager { get; set; }
        public string TargetManager { get; set; }
        public string DataType { get; set; }
        public object Data { get; set; }
        public string SyncId { get; set; }

        public override int Priority => 12;

        public DataSyncRequestEvent(string sourceManager, string targetManager, string dataType, object data, string syncId = null)
        {
            SourceManager = sourceManager;
            TargetManager = targetManager;
            DataType = dataType;
            Data = data;
            SyncId = syncId ?? Guid.NewGuid().ToString();
            Source = sourceManager;
        }
    }

    /// <summary>
    /// Event for responding to data synchronization requests
    /// </summary>
    public class DataSyncResponseEvent : GameEvent
    {
        public string SyncId { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public object ResponseData { get; set; }

        public DataSyncResponseEvent(string syncId, bool success, string errorMessage = "", object responseData = null)
        {
            SyncId = syncId;
            Success = success;
            ErrorMessage = errorMessage;
            ResponseData = responseData;
            Source = "SystemManager";
        }
    }

    /// <summary>
    /// Event for manager state changes
    /// </summary>
    public class ManagerStateChangedEvent : GameEvent
    {
        public string ManagerName { get; set; }
        public ManagerState OldState { get; set; }
        public ManagerState NewState { get; set; }
        public string Reason { get; set; }
        public Dictionary<string, object> StateData { get; set; }

        public override int Priority => 11;

        public ManagerStateChangedEvent(string managerName, ManagerState oldState, ManagerState newState, string reason = "", Dictionary<string, object> stateData = null)
        {
            ManagerName = managerName;
            OldState = oldState;
            NewState = newState;
            Reason = reason;
            StateData = stateData ?? new Dictionary<string, object>();
            Source = managerName;
        }
    }

    /// <summary>
    /// Manager states
    /// </summary>
    public enum ManagerState
    {
        Uninitialized,
        Initializing,
        Ready,
        Busy,
        Error,
        Shutting_Down,
        Shutdown
    }

    /// <summary>
    /// Event for system-wide operations
    /// </summary>
    public class SystemOperationEvent : GameEvent
    {
        public SystemOperation Operation { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public string OperationId { get; set; }
        public List<string> TargetManagers { get; set; }

        public override int Priority => 13;

        public SystemOperationEvent(SystemOperation operation, Dictionary<string, object> parameters = null, List<string> targetManagers = null, string operationId = null)
        {
            Operation = operation;
            Parameters = parameters ?? new Dictionary<string, object>();
            TargetManagers = targetManagers ?? new List<string>();
            OperationId = operationId ?? Guid.NewGuid().ToString();
            Source = "SystemManager";
        }
    }

    /// <summary>
    /// System operations
    /// </summary>
    public enum SystemOperation
    {
        Initialize,
        Shutdown,
        Reset,
        Backup,
        Restore,
        Validate,
        Synchronize,
        Maintenance,
        Performance_Check,
        Memory_Cleanup
    }

    /// <summary>
    /// Event for system operation responses
    /// </summary>
    public class SystemOperationResponseEvent : GameEvent
    {
        public string OperationId { get; set; }
        public string RespondingManager { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object> Results { get; set; }
        public TimeSpan ExecutionTime { get; set; }

        public SystemOperationResponseEvent(string operationId, string respondingManager, bool success, string message = "", Dictionary<string, object> results = null, TimeSpan executionTime = default)
        {
            OperationId = operationId;
            RespondingManager = respondingManager;
            Success = success;
            Message = message;
            Results = results ?? new Dictionary<string, object>();
            ExecutionTime = executionTime;
            Source = respondingManager;
        }
    }

    /// <summary>
    /// Event for performance metrics
    /// </summary>
    public class PerformanceMetricEvent : GameEvent
    {
        public string MetricName { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public DateTime Timestamp { get; set; }
        public string Category { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        public PerformanceMetricEvent(string metricName, double value, string unit = "", string category = "General", Dictionary<string, object> metadata = null)
        {
            MetricName = metricName;
            Value = value;
            Unit = unit;
            Category = category;
            Timestamp = DateTime.UtcNow;
            Metadata = metadata ?? new Dictionary<string, object>();
            Source = "PerformanceMonitor";
        }
    }

    /// <summary>
    /// Event for error reporting across the system
    /// </summary>
    public class SystemErrorEvent : GameEvent
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }
        public ErrorSeverity Severity { get; set; }
        public string Component { get; set; }
        public Dictionary<string, object> Context { get; set; }
        public bool IsRecoverable { get; set; }

        public override int Priority => 14; // High priority for errors

        public SystemErrorEvent(string errorCode, string errorMessage, Exception exception = null, ErrorSeverity severity = ErrorSeverity.Error, string component = "", Dictionary<string, object> context = null, bool isRecoverable = true)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            Exception = exception;
            Severity = severity;
            Component = component;
            Context = context ?? new Dictionary<string, object>();
            IsRecoverable = isRecoverable;
            Source = component;
        }
    }

    /// <summary>
    /// Error severity levels
    /// </summary>
    public enum ErrorSeverity
    {
        Info,
        Warning,
        Error,
        Critical,
        Fatal
    }

    /// <summary>
    /// Event for resource management
    /// </summary>
    public class ResourceEvent : GameEvent
    {
        public ResourceOperation Operation { get; set; }
        public string ResourceType { get; set; }
        public string ResourceId { get; set; }
        public object ResourceData { get; set; }
        public long ResourceSize { get; set; }
        public string RequestingManager { get; set; }

        public ResourceEvent(ResourceOperation operation, string resourceType, string resourceId, object resourceData = null, long resourceSize = 0, string requestingManager = "")
        {
            Operation = operation;
            ResourceType = resourceType;
            ResourceId = resourceId;
            ResourceData = resourceData;
            ResourceSize = resourceSize;
            RequestingManager = requestingManager;
            Source = "ResourceManager";
        }
    }

    /// <summary>
    /// Resource operations
    /// </summary>
    public enum ResourceOperation
    {
        Allocate,
        Deallocate,
        Load,
        Unload,
        Cache,
        Evict,
        Validate,
        Cleanup
    }

    /// <summary>
    /// Event for configuration changes
    /// </summary>
    public class ConfigurationChangedEvent : GameEvent
    {
        public string ConfigurationKey { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
        public string Category { get; set; }
        public bool RequiresRestart { get; set; }
        public List<string> AffectedManagers { get; set; }

        public override int Priority => 10;

        public ConfigurationChangedEvent(string configurationKey, object oldValue, object newValue, string category = "General", bool requiresRestart = false, List<string> affectedManagers = null)
        {
            ConfigurationKey = configurationKey;
            OldValue = oldValue;
            NewValue = newValue;
            Category = category;
            RequiresRestart = requiresRestart;
            AffectedManagers = affectedManagers ?? new List<string>();
            Source = "ConfigurationManager";
        }
    }

    /// <summary>
    /// Event for audit logging
    /// </summary>
    public class AuditEvent : GameEvent
    {
        public string Action { get; set; }
        public string Actor { get; set; }
        public string Target { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> Details { get; set; }
        public AuditLevel Level { get; set; }

        public AuditEvent(string action, string actor, string target, AuditLevel level = AuditLevel.Info, Dictionary<string, object> details = null)
        {
            Action = action;
            Actor = actor;
            Target = target;
            Level = level;
            Timestamp = DateTime.UtcNow;
            Details = details ?? new Dictionary<string, object>();
            Source = "AuditManager";
        }
    }

    /// <summary>
    /// Audit levels
    /// </summary>
    public enum AuditLevel
    {
        Debug,
        Info,
        Warning,
        Security,
        Critical
    }
} 