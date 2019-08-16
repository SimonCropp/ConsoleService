using System;
using System.Reflection;
using ConsoleService;

static class OverrideValidator
{
    public static void Validate(ProgramService service)
    {
        var type = service.GetType();
        var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        var onStart = type.GetMethod("OnStart", bindingFlags, null, new[] {typeof(string[])}, null);
        EnsureNotOverriden(onStart);
        var onStop = type.GetMethod("OnStop", bindingFlags);
        EnsureNotOverriden(onStop);
    }

    static void EnsureNotOverriden(MethodInfo method)
    {
        if (method.DeclaringType != typeof(ProgramService))
        {
            throw new Exception("Do not override OnStart or OnStop. Instead place that code in OnStartAsync and OnStopAsync");
        }
    }
}