using Astralbrew.Celesta.Compiler.AST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.CelestaSyntaxTreeCompiler
{
    internal static class Extensions
    {
        public static void Scan(this ISyntaxTreeNode stn, Action<ISyntaxTreeNode> action)
        {
            action(stn);

            if (stn is VariableNode || stn is ConstantNode)
                return;
            if (stn is AssignNode assign)
            {
                assign.LeftHandSide.Scan(action);
                assign.RightHandSide.Scan(action);
            }
            else if (stn is BlockNode block)
                foreach (var n in block.GetNodes())
                    n.Scan(action);
            else if (stn is ConditionalNode cond)
            {
                cond.Condition.Scan(action);
                cond.ThenBranch.Scan(action);
                cond.ElseBranch?.Scan(action);
            }
            else if (stn is FunctionNode func)
            {
                foreach (var a in func.GetArguments())
                    a.Scan(action);
            }
            else if (stn is LoopNode loop)
            {
                loop.RunningCondition.Scan(action);
                loop.LoopLogic.Scan(action);
            }
            else if (stn is OperatorNode op)
            {
                op.Param1.Scan(action);
                op.Param2?.Scan(action);
            }
            else if (stn is RepeatNode rept)
            {
                rept.NumberOfIterations.Scan(action);
                rept.LoopLogic.Scan(action);
            }
            else
                throw new ArgumentException($"Unknown syntax tree node type : {stn.GetType().Name}");
        }

        public static void Scan<ST>(this ISyntaxTreeNode stn, Action<ST> action) where ST : ISyntaxTreeNode
        {
            stn.Scan(n =>
            {
                if (n is ST t)
                    action(t);
            });
        }

        public static byte[] ToBytes(this uint[] uints)
        {
            byte[] result = new byte[uints.Length * sizeof(uint)];
            Buffer.BlockCopy(uints, 0, result, 0, result.Length);
            return result;
        }
    }
}
