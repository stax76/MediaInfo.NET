
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

public class PowerShell
{
    public static string InitCode { get; set; }

    public static Collection<PSObject> Invoke(string code)
    {
        return Invoke(code, null, null);
    }

    public static Collection<PSObject> Invoke(string code, string varName, object varValue)
    {
        using (var runspace = RunspaceFactory.CreateRunspace())
        {
            runspace.ApartmentState = System.Threading.ApartmentState.STA;
            runspace.Open();

            using (var pipeline = runspace.CreatePipeline())
            {
                if (!string.IsNullOrEmpty(InitCode))
                    pipeline.Commands.AddScript(InitCode);

                pipeline.Commands.AddScript(code);

                if (!string.IsNullOrEmpty(varName))
                    runspace.SessionStateProxy.SetVariable(varName, varValue);

                return pipeline.Invoke();
            }
        }
    }

    public static string InvokeAndConvert(string code)
    {
        return InvokeAndConvert(code, null, null);
    }

    public static string InvokeAndConvert(string code, string varName, object varValue)
    {
        var objects = Invoke(code, varName, varValue);

        if (objects == null)
            return "";

        List<string> lines = new List<string>();

        foreach (var obj in objects)
        {
            if (obj != null)
                lines.Add(obj.ToString());
        }

        return string.Join(Environment.NewLine, lines);
    }
}
