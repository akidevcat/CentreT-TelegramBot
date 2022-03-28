using System.Text;
using Appccelerate.StateMachine.AsyncMachine;
using Appccelerate.StateMachine.AsyncMachine.ActionHolders;
using Appccelerate.StateMachine.AsyncMachine.GuardHolders;
using Appccelerate.StateMachine.AsyncMachine.States;

namespace CentreT_TelegramBot.StateMachine.Reports;

public class DotStateMachineReportGenerator<TState, TEvent> : IStateMachineReport<TState, TEvent>
    where TState : IComparable
    where TEvent : IComparable
{

    private readonly TextWriter _textWriter;
    
    public DotStateMachineReportGenerator(TextWriter textWriter)
    {
        _textWriter = textWriter;
    }

    public void Report(string name, IEnumerable<IStateDefinition<TState, TEvent>> stateDefinitions, TState initialStateId)
    {
        var dot = new StringBuilder();

        dot.AppendLine($"digraph {name} {{");
        dot.AppendLine("node [shape=box3d,fontsize=\"12pt\",margin=\"0.2,0.1\"];");

        foreach (var state in stateDefinitions)
        {
            dot.Append(ConvertStateToDotString(state));
            dot.Append('\n');
        }   
        
        dot.Append('}');
        
        _textWriter.Write(dot);
    }

    private static StringBuilder ConvertStateToDotString(IStateDefinition<TState, TEvent> state)
    {
        var result = new StringBuilder();

        foreach (var keyValuePair in state.Transitions)
        {
            var e = keyValuePair.Key;
            foreach (var t in keyValuePair.Value)
            {
                if (t.Target == null)
                    continue;
                result.Append('\t');
                result.Append(GetStateName(t.Source));
                result.Append(" -> ");
                result.Append(GetStateName(t.Target));

                var tags = new List<(string, string)> {("label", e.ToString()!)};

                if (t.Guard != null)
                {
                    tags.Add(("style", "dashed"));
                }
                
                result.Append($"[{GetCombinedTags(tags)}]");
                result.AppendLine(";");
            }
        }
        
        return result;
    }

    private static string GetCombinedTags(IEnumerable<(string, string)> tags)
    {
        var result = "";
        var flFirst = true;
        foreach (var (tagName, tagValue) in tags)
        {
            if (!flFirst)
            {
                result += ",";
            }

            result += tagName + "=" + tagValue;
            
            flFirst = false;
        }
        return result;
    }

    private static string GetTransitionLabel(IEnumerable<IActionHolder> actions, IGuardHolder guard)
    {
        return "";
    }
    
    private static string GetActionsLabel(IEnumerable<IActionHolder> actions)
    {
        var result = "";
        foreach (var a in actions)
        {
            result += a.Describe().Replace(" ", "_") + ",";
        }

        return result;
    }
    
    private static string GetStateName(IStateDefinition<TState, TEvent> state)
    {
        return state.Id.ToString()!.Replace(" ", "_");
    }
    
}