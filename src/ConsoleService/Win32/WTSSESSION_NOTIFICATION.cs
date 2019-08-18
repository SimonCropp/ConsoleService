using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
class WTSSESSION_NOTIFICATION
{
    public int size;
    public int sessionId;
}