using Astralbrew.Celesta.Runtime;
using Astralbrew.CelestaSyntaxTreeCompiler.Assembler;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace Astralbrew.CelestaSyntaxTreeCompiler
{
    internal class Program
    {
        //static readonly Interpreter Interpreter = new Interpreter(RuntimeContext.DefaultRuntimeContext);
        static readonly Interpreter Interpreter = new Interpreter(CompilerContext.TestCompiler);

        static void Run(string input)
        {
            var ev = Interpreter.Evaluate(input) as AssembledCode;
            ev.Items.ForEach(Console.WriteLine);
            File.WriteAllBytes("result.out", ev.Binary);

            //Console.WriteLine(Interpreter.Evaluate(input) ?? "<NULL>");
            return;

            try
            {
                Console.WriteLine(Interpreter.Evaluate(input) ?? "<NULL>");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);                
            }
        }

        static void RunInteractiveInterpreter()
        {
            Console.WriteLine("Astral Celesta Interpreter\n\n");

            while (true)
            {
                Console.Write(">> ");                
                Run(Console.ReadLine());
                Console.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            /*while(true)
            {
                try
                {
                    Console.Write(">> ");
                    var iname = Console.ReadLine();
                    var opcode = Opcodes.GetOpcode(iname);                
                    Console.WriteLine("Opcode = " + opcode.ToString("X2"));
                    Console.WriteLine("Instr  = " + Opcodes.GetInstructionName(opcode));
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }                      
            
            Console.ReadLine();*/            

            if (args.Length == 0)
            {
                RunInteractiveInterpreter();
            }
            if (args.Length == 1)
            {
                Run($"load(\"{Regex.Escape(args[0])}\")");
            }
        }
    }
}