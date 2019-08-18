using System;
using System.Runtime.InteropServices;
using ConsoleService;

[StructLayout(LayoutKind.Sequential)]
public struct SERVICE_TABLE_ENTRY
{
    public IntPtr name;
    public ServiceMainCallback callback;
}