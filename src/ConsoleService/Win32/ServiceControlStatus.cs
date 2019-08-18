static class ServiceControlStatus
{
    internal const int STATE_CONTINUE_PENDING = 0x00000005;
    internal const int STATE_PAUSED = 0x00000007;
    internal const int STATE_PAUSE_PENDING = 0x00000006;
    internal const int STATE_RUNNING = 0x00000004;
    internal const int STATE_START_PENDING = 0x00000002;
    internal const int STATE_STOPPED = 0x00000001;
    internal const int STATE_STOP_PENDING = 0x00000003;
    internal const int ERROR_EXCEPTION_IN_SERVICE = 0x00000428;
}