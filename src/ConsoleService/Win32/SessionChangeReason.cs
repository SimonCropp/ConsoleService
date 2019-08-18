namespace ConsoleService
{
    public enum SessionChangeReason
    {
        /// <devdoc>
        ///    <para>A session was connected to the console session. </para>
        /// </devdoc>
        ConsoleConnect = SessionStateChange2.WTS_CONSOLE_CONNECT,

        /// <devdoc>
        ///    <para>A session was disconnected from the console session. </para>
        /// </devdoc>
        ConsoleDisconnect = SessionStateChange2.WTS_CONSOLE_DISCONNECT,

        /// <devdoc>
        ///    <para>A session was connected to the remote session. </para>
        /// </devdoc>
        RemoteConnect = SessionStateChange2.WTS_REMOTE_CONNECT,

        /// <devdoc>
        ///    <para>A session was disconnected from the remote session. </para>
        /// </devdoc>
        RemoteDisconnect = SessionStateChange2.WTS_REMOTE_DISCONNECT,

        /// <devdoc>
        ///    <para>A user has logged on to the session. </para>
        /// </devdoc>
        SessionLogon = SessionStateChange2.WTS_SESSION_LOGON,

        /// <devdoc>
        ///    <para>A user has logged off the session. </para>
        /// </devdoc>
        SessionLogoff = SessionStateChange2.WTS_SESSION_LOGOFF,

        /// <devdoc>
        ///    <para>A session has been locked. </para>
        /// </devdoc>
        SessionLock = SessionStateChange2.WTS_SESSION_LOCK,

        /// <devdoc>
        ///    <para>A session has been unlocked. </para>
        /// </devdoc>
        SessionUnlock = SessionStateChange2.WTS_SESSION_UNLOCK,

        /// <devdoc>
        ///    <para>A session has changed its remote controlled status. </para>
        /// </devdoc>
        SessionRemoteControl = SessionStateChange2.WTS_SESSION_REMOTE_CONTROL
    }
}