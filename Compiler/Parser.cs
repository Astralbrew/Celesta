using Astralbrew.Celesta.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Astralbrew.Celesta.Compiler
{
    public partial class Parser
    {
        public List<(string Key, string[] Pattern, string[] TreeBuild)> Rules = new List<(string, string[], string[])>();

        private static readonly string[] Keywords = Constants.LanguageDefinition.Keywords;

        public Parser()
        {
            AddPatternRule("@CODE", "@SCOPE");

            /*AddPatternRule("@SCOPE", "@INSTR ; @SCOPE", "~scope %1 %2");
            AddPatternRule("@SCOPE", "@INSTR ;");
            AddPatternRule("@SCOPE", "@INSTR");*/

            //AddPatternRule("@SCOPE", "@INSTR ; ?? @INSTR", "^*SCOPE");
            //AddPatternRule("@SCOPE", "@INSTR ; ?? @INSTR ;", "^*SCOPE");            
            AddPatternRule("@SCOPE", "@INSTR", "^*SCOPE");       

            AddPatternRule("@LIST", "@E, @LIST", "~list %1 %2");
            AddPatternRule("@LIST", "@E", "~list %1");

            AddPatternRule("@INSTR", "@C");
            AddPatternRule("@C", "@WHILE");
            AddPatternRule("@C", "@IF");
            AddPatternRule("@C", "@DECL");
            AddPatternRule("@C", "@ASSIGN");
            AddPatternRule("@C", "@E");

            AddPatternRule("@WHILE", "while @E do @SCOPE endwhile|end", "~while %1 %2");

            AddPatternRule("@IF", "if @E then @SCOPE else @SCOPE endif|end", "~if %1 %2 %3");
            AddPatternRule("@IF", "if @E then @SCOPE endif|end", "~if %1 %2");            

            AddPatternRule("@DECL", "SYMBOL SYMBOL = @E", "~decl %1 %2 %3");
            AddPatternRule("@DECL", "SYMBOL SYMBOL", "~decl %1 %2");

            AddPatternRule("@FUN", "SYMBOL ( )", "~fun %1");
            AddPatternRule("@FUN", "SYMBOL ( @LIST )", "~fun %1 %2");

            AddPatternRule("@ASSIGN", "SYMBOL = @E", "~assign %1 %2");

            var operatorsRuleBuilder = new OperatorsRuleBuilder();

            operatorsRuleBuilder.AddLayer("or");
            operatorsRuleBuilder.AddLayer("and");            
            operatorsRuleBuilder.AddLayer("==", "!=");
            operatorsRuleBuilder.AddLayer("<", "<=", ">", ">=");
            operatorsRuleBuilder.AddLayer("+", "-");
            operatorsRuleBuilder.AddLayer("*", "/", "%");

            operatorsRuleBuilder.GetRules("@E", "@T").ForEach(r =>
                AddPatternRule(r.Key, string.Join(" ", r.Pattern), string.Join(" ", r.TreeBuild)));            


            AddPatternRule("@T", "+ @T", "+u %1");
            AddPatternRule("@T", "- @T", "-u %1");
            AddPatternRule("@T", "@IF");
            AddPatternRule("@T", "@ASSIGN");
            AddPatternRule("@T", "@FUN");
            AddPatternRule("@T", "LITERAL");
            AddPatternRule("@T", "SYMBOL");
            AddPatternRule("@T", "( @E )");                     
            
            /*foreach(var r in Rules)
            {
                Console.WriteLine($"{r.Key} : {string.Join(" ", r.Pattern)}");
            }*/
        }
        
        internal void AddPatternRule(string Key, string Pattern, string TreeBuild = "")
        {            
            Rules.Add((Key, Pattern.Split(' '), TreeBuild.Split(' ').Where(s => s != "").ToArray()));           
        }

        public ParseTreeNode Parse(string input)
        {
            Cache.Clear();
            int pos = 0;
            var tokens = input.SplitToTokens();
            if (tokens.Count == 0)
                return new ParseTreeNode("~scope");
            var tree = Parse(0, tokens, ref pos);
            if (pos != tokens.Count) 
                throw new ParseException("Parse error");

            /*Console.WriteLine(tree?.ToString() ?? "<NULL>");

            var tree2 = tree.Flatten();

            Console.WriteLine(tree2?.ToString() ?? "<NULL>");*/

            return tree;
        }    

        Dictionary<string, (ParseTreeNode Node, int Pos)> Cache = new Dictionary<string, (ParseTreeNode Node, int Pos)>();

        private ParseTreeNode Parse(int ruleId, List<string> tokens, ref int pos, int stack = 0)
        {
            var cacheKey = $"{Rules[ruleId].Key}!{pos}";
            if (Cache.ContainsKey(cacheKey)) 
            {
                pos = Cache[cacheKey].Pos;
                return Cache[cacheKey].Node;
            }

            if (stack == Rules.Count * Rules.Count) return null;

            string[] pattern = Rules[ruleId].Pattern;
            string[] treeBuild = Rules[ruleId].TreeBuild;

            //Console.WriteLine(string.Join(" ", tokens));
            int t = pos;
            /*Console.WriteLine(string.Join(" ", tokens.Select((x, i) =>
            {
                string w = new string(' ', x.Length - 1);
                if (i == t) return '^' + w;
                return ' ' + w;
            })));*/
            //Console.WriteLine($"({stack}) Trying {Rules[ruleId].Key} : {string.Join(" ", pattern)}");

            List<string> guesses = new List<string>();
            List<ParseTreeNode> children = new List<ParseTreeNode>();
            int tmppos = pos;


            var fixedSplit = pattern.TakeWhile(s => s != "??");
            var reptSplit = pattern.SkipWhile(s => s != "??").Skip(1);

            //Console.WriteLine($"Fixed = {string.Join(" ", fixedSplit)}");
            //Console.WriteLine($"Reptt = {string.Join(" ", reptSplit)}");

            var pattern2 = fixedSplit.ToList();
            ParseTreeNode solution = null;

            for(int i=0;i<pattern2.Count;i++)
            {
                //Console.WriteLine(string.Join(" ", pattern2));
                if (tmppos >= tokens.Count) return solution;
                string rule = pattern2[i];
                //Console.WriteLine($"\tPattern = {rule}");
                //Console.WriteLine($"\tToken = {tokens[tmppos]}");                
                if (rule == "LITERAL") 
                {
                    if (tmppos >= tokens.Count || !IsLiteral(tokens[tmppos])) 
                    {
                        ////Console.WriteLine("--Fail.");
                        return solution;
                    }
                    children.Add(new ParseTreeNode(tokens[tmppos]));
                    tmppos++;
                }
                else if(rule=="SYMBOL")
                {
                    if (tmppos >= tokens.Count || !IsSymbol(tokens[tmppos]))
                    {
                        ////Console.WriteLine("--Fail.");
                        return solution;
                    }
                    children.Add(new ParseTreeNode(tokens[tmppos]));
                    tmppos++;
                }
                else if (rule[0]!='@')
                {                    
                    if (tmppos >= tokens.Count) 
                    {
                        ////Console.WriteLine("--Fail.");                       
                        return solution;
                    }
                    //Console.WriteLine(String.Join(" ", rule.Split('|')));
                    var match = rule.Split('|').Where(r => r == tokens[tmppos]);
                    //Console.WriteLine($"Matches = {match.Count()}");
                    if(match.Count()==0)
                    {
                        ////Console.WriteLine("--Fail.");
                        return solution;
                    }
                    guesses.Add(match.First());

                    tmppos++;
                }
                else
                {
                    bool ok = false;
                    foreach (var subrule in Rules.Select((r, p) => (r, p))
                        .Where(x => x.r.Key == rule && (x.r.Key != Rules[ruleId].Key || (i > 0 || (x.r.Key == Rules[ruleId].Key && x.p != ruleId))))
                        .Select(r => r.p)) 
                    {
                        var subtree = Parse(subrule, tokens, ref tmppos, stack + 1);
                        if (subtree != null)
                        {
                            children.Add(subtree);
                            ok = true;
                            break;
                        }
                    }
                    if (!ok)
                    {
                        ////Console.WriteLine("--Fail.");
                        return solution;
                    }
                }

                if (i == pattern2.Count - 1) 
                {
                    pos = tmppos;
                    solution = BuildParseTreeNode(treeBuild, children, guesses, pos, cacheKey);
                    if(reptSplit.Count()>0)
                    {
                        //Console.WriteLine("Extending");
                        pattern2.AddRange(reptSplit);
                    }
                }
            }
            
            pos = tmppos;

            return solution;
        }        

        ParseTreeNode BuildParseTreeNode(string[] treeBuild, List<ParseTreeNode> children, List<string> guesses, int pos, string cacheKey)        
        {
            if (treeBuild.Length == 0)
            {
                Cache[cacheKey] = (children[0], pos);
                return children[0];
            }

            if (treeBuild.Length == 1 && treeBuild[0] == "^L*")
            {
                var result = children[0];
                for (int i = 0; i < guesses.Count; i++)
                {
                    result = new ParseTreeNode(guesses[i], new ParseTreeNode[] { result, children[i + 1] });
                }
                return result;
            }

            if(treeBuild.Length==1 && treeBuild[0]== "^*SCOPE")
            {  
                return new ParseTreeNode("~scope", children.ToArray());
            }

            string label = treeBuild[0];
            if (label[0] == '^' && guesses.Count > 0)
            {
                label = guesses[int.Parse(label.Substring(1)) - 1];
            }

            var resChildren = treeBuild.Skip(1).Select(tb =>
            {
                int id = int.Parse(tb.Substring(1)) - 1;
                return children[id];
            }).ToArray();


            var treenode = new ParseTreeNode(label, resChildren);
            Cache[cacheKey] = (treenode, pos);
            ////Console.WriteLine("OK");
            return treenode;
        }

        public static bool IsLiteral(string input)
        {
            return double.TryParse(input, out double _) || input.MatchesCStyleString();
        }

        public static bool IsSymbol(string input)
        {
            return !Keywords.Contains(input) && Regex.IsMatch(input, @"^[a-zA-Z_][a-zA-Z0-9]*$");
        }      
    }
}
